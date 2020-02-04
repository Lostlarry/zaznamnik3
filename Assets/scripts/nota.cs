using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Znak : MonoBehaviour
{
    public static Grafix_bank Gfx_src;
    public static Hand_Ctrl CTRL;

    public const int notes_per_line = 16;
    public const float takt_width = 26.4f;
    public const float nota_height = 3.7f;

    protected int delka = 0; // exponent 2 vzdy zaporne (2 na -2 je 1/4, atd.)

    public Holder master;

    protected Znak prev;
    protected Znak next;

    protected float pos_x = 0;
    protected int pos_y = 0;

    public Znak Prev { get => prev; set => prev = value; }
    public Znak Next { get => next; set => next = value; }
    public float Pos_x { get => pos_x; set => pos_x = value; }
    public int Pos_y { get => pos_y; set => pos_y = value; }
    public virtual int Postfix { get { return 0; } set { } }

    public void Load(Znak Z)
    {
        delka = Z.delka;
    }

    public virtual void Calc_Pos(bool update = false)
    {
        if (update)
        {
            Transfer_pos(prev);
            Bump_pos();
        }
        gameObject.transform.position = gameObject.transform.position + new Vector3((pos_x * takt_width), pos_y * Hand_Ctrl.vyska_linek, 0);
    }

    public void Swap_Pos(Znak target)
    {
        float tmp_x = target.pos_x;
        int tmp_y = target.pos_y;
        target.pos_x = pos_x;
        target.pos_y = pos_y;
        pos_x = tmp_x;
        pos_y = tmp_y;
        Calc_Pos();
        target.Calc_Pos();
    }


    public virtual bool is_nota()
    {
        return false;
    }

    public virtual void Nota_Up(int i = 1)
    {
        return;
    }

    public virtual void Nota_Down(int i = 1)
    {
        return;
    }

    public virtual void Nota_Long(int i = 1)
    {
        delka = delka + 1;
        if (delka > 0)
        {

        }
        Update_gfx();
    }

    public virtual void Nota_Short(int i = 1)
    {
        if (i > -4)// mensi nez sestnanctiny neberem
        {
            delka = delka - 1;
            Update_gfx(); 
        }
    }

    public override string ToString()
    {
        return "Z,"+delka+";";
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
                delka = output;
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
        delka = 0;
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
        pos_x = target.pos_x;
        pos_y = target.pos_y;
    }

    public void Transfer_pos(Znak target)
    {
        pos_x = target.pos_x;
        pos_y = target.pos_y;
    }

    public bool Bump_pos()
    { 
        bool output = false;
        float mod = 2^prev.delka;
        mod = mod + 0.5f * prev.Postfix;
        pos_x = pos_x + mod;
        if (pos_x > notes_per_line)
        {
            pos_x = pos_x - notes_per_line;
            pos_y++;
            output = true;
        }
        Calc_Pos();
        return output;
    }

    public virtual int[] Copy()
    {
        return new int[2] {0, delka};
    }

    public virtual void Paste(int[] input)
    {
        if (input[0] == 0)
        {
            delka = input[1];
        }
    }

    protected virtual void Update_gfx()
    {
        
    }

    void Start() { }
    void Update() { }
}

public class Nota : Znak
{

    int vyska = 0; // relativne ku spodni radce

    int prefix = 0;// 0= nic 1 = krizky 2 = becka 3 = cista 
    int postfix = 0;// 0 = nic 1 az 3 tecky(prida pulku delky)
    int topfix = 0;// 0 = nic 1 

    public GameObject prapor;

    public override int Postfix { get => postfix; set => postfix = value; }

    public override void Calc_Pos(bool update = false)
    {
        if (update)
        {
            Transfer_pos(prev);
            Bump_pos();
        }
        gameObject.transform.position = gameObject.transform.position + new Vector3(pos_x * takt_width, pos_y * Hand_Ctrl.vyska_linek + vyska * nota_height, 0);
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
        return "N,"+delka+","+vyska+","+prefix+","+postfix+";";
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
                delka = output;
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
        return new int[6] {2, delka, postfix, prefix, topfix, vyska};
    }

    public override void Paste(int[] input)
    {
        if (input[0] == 2)
        {
            delka = input[1];
            postfix = input[2];
            prefix = input[3];
            topfix = input[4];
            vyska = input[5];
        }
    }

    protected override void Update_gfx()
    {
        // vyska a prevraceni
        gameObject.transform.position = ref_point + new Vector3(0, vyska * Znak.nota_height);
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
        if (delka < -1)
        {
            gameObject.GetComponent<Image>().sprite = Gfx.Nota_ctvrt;
            if (delka < -2)
            {
                prapor_GOs[0].SetActive(true);
                if (delka == -4)
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
            if (delka == -1)
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
    }
}

class Pomlka : Znak
{

    int postfix = 0;// 0 = nic 1 az 3 tecky(prida pulku delky)

    public override string ToString()
    {
        return "N," + delka + "," + postfix + ";";
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
                delka = output;
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
        return new int[3] {1, delka, postfix};
    }

    public override void Paste(int[] input)
    {
        if (input[0] == 1)
        {
            delka = input[1];
            postfix = input[2];
        }
    }

    protected override void Do_lig()
    {

    }
}

class Acord : Znak
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
        return "A," + delka + "," + output + ";";
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
                delka = output;
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
        return new int[3] { 3, delka, topfix};
    }
    public override void Paste(int[] input)
    {
        if (input[0] == 3)
        {
            delka = input[1];
            topfix = input[2];
        }
    }
}

