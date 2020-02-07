﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Znak : MonoBehaviour
{
    public static Grafix_bank Gfx;
    public static Hand_Ctrl CTRL;
    public static Vector3 ref_point;

    public static int takts_per_line = 5;// nevissi id takze taktu je o jedna vic
    public static float takt_width = 79.2f;
    public static float takt_delka = 1f;
    public const float nota_height = 3.7f;

    private int delka = 0; // exponent 2 vzdy zaporne (2 na -2 je 1/4, atd.)
    int postfix = 0;// 0 = nic 1 az 3 = mnozsti tecek delka s teckama = (delka * (2^-pocet tecek))

    public Holder master;

    protected Znak prev;
    protected Znak next;

    protected float pos_x = 0;
    protected float dist_x = 0;
    protected int pos_y = 0;
    protected int hand_id = 0;

    public Znak Prev { get => prev; set => prev = value; }
    public Znak Next { get => next; set => next = value; }

    public float Pos_x
    {
        get
        {
            return pos_x;
        }
        set
        {
            if (pos_x != value)
            {
                pos_x = value;
                CTRL.Do_Takty(master);
            }
            else
            {
                pos_x = value;
            }
        }
    }

    public int Pos_y { get => pos_y; set => pos_y = value; }
    public virtual int Postfix { get { return 0; } set { } }

    public int Hand_id { get => hand_id; set => hand_id = value; }
    public float Dist_x { get => dist_x; set => dist_x = value; }
    protected int Delka
    {
        get
        {
            return delka;
        }
        set
        {
            delka = value;
            if (dist_x % 1 + Math.Pow(2, delka) * (2 - Math.Pow(2, -postfix)) > takt_delka)
            {
                Adapt(1 - dist_x % takt_delka);
                Do_lig();
            }
            else if (Delka < -4)
            {
                Delka = -4;
            }
            Update_gfx();
        }
    }

    public void Adapt(float input)
    {
        float rem_input = input;
        int tmp_delka = 1;
        int targ_post = 0;
        for (int i = 0; i < 5; i++)
        {
            if (Math.Pow(2, -i) > input)
            {
                if (tmp_delka > 0)
                {
                    rem_input = rem_input - (float)Math.Pow(2, -i);
                    tmp_delka = -i;
                }
                else
                {
                    rem_input = rem_input - (float)Math.Pow(2, -i) * tmp_delka;
                    targ_post++;
                }
            }
            else if (tmp_delka < 0)
            {
                i = 5;
            }
        }
        bool shifted = false;
        if (rem_input > 0)
        {
            if (is_nota())
            {
                CTRL.Add_Nota(master, this).Adapt(input);
            }
            else
            {
                CTRL.Add_Pomlka(master, this).Adapt(input);
            }
            shifted = true;
        }
        Znak target = this;
        if (shifted)
        {
            target = next;
        }
        if (is_nota())
        {
            CTRL.Add_Nota(master, target).Adapt((float)(Math.Pow(2, delka) * (2 - Math.Pow(2, -postfix) - input)));
        }
        else
        {
            CTRL.Add_Pomlka(master, target).Adapt((float)(Math.Pow(2, delka) * (2 - Math.Pow(2, -postfix) - input)));
        }
        delka = tmp_delka;
        postfix = targ_post;
        Update_gfx();
    }

    public void Load(Znak Z)
    {
        Delka = Z.Delka;
    }

    public virtual void Calc_Pos(bool update = false)
    {
        if (update)
        {
            Transfer_pos(prev);
            Bump_pos();
        }
        int mod = 0;
        if (hand_id > 0)
        {
            mod = hand_id - 1 + pos_y;
        }
        gameObject.transform.position = ref_point + new Vector3((Pos_x * takt_width), (pos_y + mod) * Hand_Ctrl.vyska_linek, 0);
    }

    public void Swap_Pos(Znak target)
    {
        float tmp_x = target.Pos_x;
        int tmp_y = target.pos_y;
        float tmp_dist = target.dist_x;
        target.dist_x = dist_x;
        target.pos_y = pos_y;
        target.Pos_x = Pos_x;
        dist_x = tmp_dist;
        pos_y = tmp_y;
        Pos_x = tmp_x;
        Calc_Pos();
        target.Calc_Pos();
    }


    public virtual bool is_nota()
    {
        return false;
    }

    public virtual void Nota_Up(int i = 1)
    {
        Update_gfx();
    }

    public virtual void Nota_Down(int i = 1)
    {
        Update_gfx();
    }

    public virtual void Nota_Long(int i = 1)
    {
        Update_gfx();
    }

    public virtual void Nota_Short(int i = 1)
    {
        if (i > -4)// mensi nez sestnanctiny neberem
        {
            Delka = Delka - 1;
        }
        Update_gfx();
    }

    public override string ToString()
    {
        return "Z,"+Delka+";";
    }

    public virtual bool FromString(string input)
    {
        bool error = false;
        char[] filter = new char[1] { ',' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        if (data[0] == "Z")
        {
            Int32 output = new Int32();
            if (int.TryParse(data[1], out output))
            {
                Delka = output;
            }
            else
            {
                error = true;
            }
        }
        else
        {
            error = true;
            Debug.Log("attepted to destring non Znak string");
        }
        return error;
    }

    protected virtual void Do_lig()
    {
        Delka = 0;
    }

    public void Transfer(Znak target)
    {
        Prev = target.Prev;
        Next = target.Next;
        if (target.Prev != null)
        {
            target.Prev.Next = this;
        }
        if (target.Next != null)
        {
            target.Next.Prev = this;
        }
        master = target.master;
        hand_id = target.hand_id;
        dist_x = target.dist_x;
        pos_y = target.pos_y;
        Pos_x = target.Pos_x;
    }

    public void Transfer_pos(Znak target)
    {
        dist_x = target.dist_x;
        pos_y = target.pos_y;
        Pos_x = target.Pos_x;
    }

    public bool Bump_pos()
    { 
        bool output = false;
        double mod = Math.Pow(2, prev.Delka) * (2 - Math.Pow(2, -prev.postfix));
        dist_x = prev.dist_x + (float)mod;
        pos_x = prev.pos_x + (float)mod;
        if (Pos_x > takts_per_line)
        {
            pos_x = 0;
            pos_y++;
            output = true;
        }
        CTRL.Do_Takty(master);
        Calc_Pos();
        return output;
    }

    public virtual int[] Copy()
    {
        return new int[2] {0, Delka};
    }

    public virtual void Paste(int[] input)
    {
        if (input[0] == 0)
        {
            Delka = input[1];
            Calc_Pos();
            Update_gfx();
            CTRL.Recalc(next);
        }
    }

    protected virtual void Update_gfx() { }
    public virtual void Do_data() { }

    void Start() { }
    void Update() { }
}

public class Nota : Znak
{
    
    GameObject prefix_GO;
    GameObject topfix_GO;
    GameObject[] postfix_GO;

    GameObject carka_licha;
    GameObject carka_suda;
    GameObject[] carky;

    GameObject[] prapor_GOs;
    
    int vyska = 0; // relativne ku spodni radce

    int prefix = 0;// 0= nic 1 = krizky 2 = becka 3 = cista 
    int postfix = 0;// 0 = nic 1 az 3 = mnozsti tecek (delka * 1,5)
    int topfix = 0;// 0 = nic 1 

    public override int Postfix { get => postfix; set => postfix = value; }

    public override void Calc_Pos(bool update = false)
    {
        if (update)
        {
            Transfer_pos(prev);
            Bump_pos();
        }
        int mod = 0;
        if (hand_id > 0)
        {
            mod = hand_id - 1 + pos_y;
        }
        gameObject.transform.position = ref_point + new Vector3(Pos_x * takt_width, (pos_y + mod) * Hand_Ctrl.vyska_linek + vyska * nota_height, 0);
        //prevraceni
        if (vyska > 1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            prefix_GO.transform.position = new Vector3(-Math.Abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
            prefix_GO.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            prefix_GO.transform.position = new Vector3(Math.Abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
            prefix_GO.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public override void Nota_Up(int i = 1)
    {
        vyska = i + vyska;
        Calc_Pos();
    }

    public override void Nota_Down(int i = 1)
    {
        vyska = i - vyska;
        Calc_Pos();
    }

    public override string ToString()
    {
        return "N,"+Delka+","+vyska+","+prefix+","+postfix+";";
    }

    public override bool is_nota()
    {
        return true;
    }

    public override bool FromString(string input)
    {
        bool error = false;
        char[] filter = new char[1] { ',' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        if (data[0] == "N")
        {
            Int32 output = new Int32();
            if (int.TryParse(data[1], out output))
            {
                Delka = output;
            }
            else
            {
                error = true;
            }

            if (int.TryParse(data[2], out output))
            {
                vyska = output;
            }
            else
            {
                error = true;
            }

            if (int.TryParse(data[3], out output))
            {
                prefix = output;
            }
            else
            {
                error = true;
            }

            if (int.TryParse(data[4], out output))
            {
                postfix = output;
            }
            else
            {
                error = true;
            }
        }
        else
        {
            error = true;
            Debug.Log("attepted to destring non Nota string");
        }
        return error;
    }
    public override int[] Copy()
    {
        return new int[6] {2, Delka, postfix, prefix, topfix, vyska};
    }

    public override void Paste(int[] input)
    {
        if (input[0] == 2)
        {
            Delka = input[1];
            postfix = input[2];
            prefix = input[3];
            topfix = input[4];
            vyska = input[5];
            Calc_Pos();
            Update_gfx();
            CTRL.Recalc(next);
        }
    }

    protected override void Update_gfx()
    {
        //pomocne carky
        if (vyska < -3)
        {
            GameObject select;
            if (vyska % 2 == 0)
            {
                select = carka_suda;
                carka_suda.SetActive(true);
                carka_licha.SetActive(false);
            }
            else
            {
                select = carka_licha;
                float dist = carka_suda.transform.position.y - carka_licha.transform.position.y;
                carka_licha.transform.position = new Vector3(carka_suda.transform.position.x, carka_suda.transform.position.y + Math.Abs(dist), carka_suda.transform.position.z);
                carka_licha.SetActive(true);
                carka_suda.SetActive(false);
            }
            int targ_carek = (Math.Abs(vyska) - 2) / 2 - 1;
            for (int i = 0; i < carky.GetLength(0); i++)
            {
                Destroy(carky[i]);
            }
            carky = new GameObject[targ_carek];
            for (int i = 0; i < targ_carek; i++)
            {
                carky[i] = Instantiate(select, gameObject.transform, true);
                carky[i].transform.position = new Vector3(select.transform.position.x, select.transform.position.y + 2 * Znak.nota_height * (i + 1), select.transform.position.z);
            }
        }
        else if (vyska > 7)
        {
            GameObject select;
            if (vyska % 2 == 0)
            {
                select = carka_suda;
                carka_suda.SetActive(true);
                carka_licha.SetActive(false);
            }
            else
            {
                select = carka_licha;
                float dist = carka_suda.transform.position.y - carka_licha.transform.position.y;
                carka_licha.transform.position = new Vector3(carka_suda.transform.position.x, carka_suda.transform.position.y - Math.Abs(dist), carka_suda.transform.position.z);
                carka_licha.SetActive(true);
                carka_suda.SetActive(false);
            }
            int targ_carek = (Math.Abs(vyska) - 6) / 2 - 1;
            for (int i = 0; i < carky.GetLength(0); i++)
            {
                Destroy(carky[i]);
            }
            carky = new GameObject[targ_carek];
            for (int i = 0; i < targ_carek; i++)
            {
                carky[i] = Instantiate(select, gameObject.transform, true);
                carky[i].transform.position = new Vector3(select.transform.position.x, select.transform.position.y - 2 * Znak.nota_height * (i + 1), select.transform.position.z);
            }
        }
        else
        {
            carka_licha.SetActive(false);
            carka_suda.SetActive(false);
            for (int i = 0; i < carky.GetLength(0); i++)
            {
                carky[i].SetActive(false);
            }
        }
        // prefix becka krizky
        switch (prefix)
        {
            case 1:
                prefix_GO.SetActive(true);
                prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_krizek;
                break;
            case 2:
                prefix_GO.SetActive(true);
                prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_becko;
                break;
            case 3:
                prefix_GO.SetActive(true);
                prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_neutral;
                break;
            default:
                prefix_GO.SetActive(false);
                break;
        }
        //delka not
        if (Delka < -1)
        {
            gameObject.GetComponent<Image>().sprite = Gfx.Nota_ctvrt;
            if (Delka < -2)
            {
                prapor_GOs[0].SetActive(true);
                if (Delka == -4)
                {
                    prapor_GOs[1].SetActive(true);
                }
                else
                {
                    prapor_GOs[1].SetActive(false);
                }
            }
            else
            {
                prapor_GOs[1].SetActive(false);
                prapor_GOs[0].SetActive(false);
            }
        }
        else
        {
            if (Delka == -1)
            {
                gameObject.GetComponent<Image>().sprite = Gfx.Nota_pull;
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = Gfx.Nota_cela;
            }
            for (int i = 0; i < prapor_GOs.GetLength(0); i++)
            {
                prapor_GOs[i].SetActive(false);
            }
        }
        //topfix
        if (topfix != 0)
        {
            topfix_GO.SetActive(true);
        }
        else
        {
            topfix_GO.SetActive(false);
        }
        //postfix
        for (int i = 0; i < postfix_GO.GetLength(0); i++)
        {
            if (postfix > i)
            {
                postfix_GO[i].SetActive(true);

            }
            else
            {
                postfix_GO[i].SetActive(false);
            } 
        }
    }

    public override void Do_data()
    {
        prapor_GOs = new GameObject[2];
        postfix_GO = new GameObject[3];
        prefix_GO = gameObject.transform.GetChild(1).gameObject;
        postfix_GO[0] = gameObject.transform.GetChild(2).gameObject;
        postfix_GO[1] = gameObject.transform.GetChild(8).gameObject;
        postfix_GO[2] = gameObject.transform.GetChild(9).gameObject;
        topfix_GO = gameObject.transform.GetChild(3).gameObject;
        prapor_GOs[0] = gameObject.transform.GetChild(4).gameObject;
        prapor_GOs[1] = gameObject.transform.GetChild(5).gameObject;
        carka_licha = gameObject.transform.GetChild(6).gameObject;
        carka_suda = gameObject.transform.GetChild(7).gameObject;
        carky = new GameObject[1];
        carky[0] = Instantiate(carka_licha, gameObject.transform);
        carky[0].SetActive(false);
        Update_gfx();
    }
}

public class Pomlka : Znak
{
    GameObject postfix_GO;
    int postfix = 0;// 0 = nic 1 = tecka(delka * 1,5)

    public override string ToString()
    {
        return "P," + Delka + "," + postfix + ";";
    }

    public override bool is_nota()
    {
        return false;
    }

    public override bool FromString(string input)
    {
        bool error = false;
        char[] filter = new char[1] { ',' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        if (data[0] == "P")
        {
            Int32 output = new Int32();
            if (int.TryParse(data[1], out output))
            {
                Delka = output;
            }
            else
            {
                error = true;
            }
            
            if (int.TryParse(data[2], out output))
            {
                postfix = output;
            }
            else
            {
                error = true;
            }
        }
        else
        {
            error = true;
            Debug.Log("attepted to destring non Pomlka string");
        }
        return error;
    }

    public override int[] Copy()
    {
        return new int[3] {1, Delka, postfix};
    }

    public override void Paste(int[] input)
    {
        if (input[0] == 1)
        {
            Delka = input[1];
            postfix = input[2];
            Calc_Pos();
            Update_gfx();
            CTRL.Recalc(next);
        }
    }

    protected override void Do_lig()
    {
        
    }

    protected override void Update_gfx()
    {
        //delka pomka
        switch (Delka)
        {
            case 0:
                gameObject.GetComponent<Image>().sprite = Gfx.P_cela;
                break;
            case -1:
                gameObject.GetComponent<Image>().sprite = Gfx.P_pull;
                break;
            case -2:
                gameObject.GetComponent<Image>().sprite = Gfx.P_ctvrt;
                break;
            case -3:
                gameObject.GetComponent<Image>().sprite = Gfx.P_osmina;
                break;
            case -4:
                gameObject.GetComponent<Image>().sprite = Gfx.P_sestnactina;
                break;
            default:
                break;
        }

        //postfix
        if (postfix != 0)
        {
            postfix_GO.SetActive(true);
        }
        else
        {
            postfix_GO.SetActive(false);
        }
    }

    public override void Do_data()
    {
        postfix_GO = gameObject.transform.GetChild(2).gameObject;
        Update_gfx();
    }
}

class Acord : Znak//not in use
{
    Nota start;
    
    int topfix = 0;// 0 = nic 1 

    public override void Nota_Up(int i = 1)
    {
    }

    public override void Nota_Down(int i = 1)
    {
    }

    public override string ToString()
    {
        string output = "";
        Nota selected = start;
        while (selected != null)
        {
            output = output + selected.ToString();
        }
        return "A," + Delka + "," + output + ";";
    }

    public override bool is_nota()
    {
        return true;//maybe
    }

    public override bool FromString(string input)
    {
        bool error = false;
        char[] filter = new char[1] { ',' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        if (data[0] == "A")
        {
            Int32 output = new Int32();
            if (int.TryParse(data[1], out output))
            {
                Delka = output;
            }
            else
            {
                error = true;
            }
        }
        else
        {
            error = true;
            Debug.Log("attepted to destring non Acord string");
        }
        return error;
    }
    public override int[] Copy()
    {
        return new int[3] { 3, Delka, topfix};
    }
    public override void Paste(int[] input)
    {
        if (input[0] == 3)
        {
            Delka = input[1];
            topfix = input[2];
        }
    }
}

