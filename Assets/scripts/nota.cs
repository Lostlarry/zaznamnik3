using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Znak : MonoBehaviour
{
    public static Grafix_bank Gfx;
    public static Hand_Ctrl CTRL;
    public static Vector3 ref_point;
    
    public const float nota_height = 3.7f;

    protected int delka = 4; // exponent 2 vzdy zaporne (2 na -2 je 1/4, atd.)
    protected int postfix = 0;// 0 = nic 1 az 3 = mnozsti tecek delka s teckama = (delka * (2^-pocet tecek))

    public Holder master;

    private Line_Ctrl linka;
    protected Takt takt;
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
        }
    }

    public int Pos_y
    {
        get
        {
            return pos_y;
        }
        set
        {
            Linka = CTRL.get_linka(master ,value);
            pos_y = value;
        }
    }
    public virtual int Postfix { get { return 0; } set { } }

    public int Hand_id { get => hand_id; set => hand_id = value; }
    public float Dist_x { get => dist_x; set => dist_x = value; }
    public int Delka
    {
        get
        {
            return delka;
        }
        set
        {
            delka = value;
            Update_delka();
            if (Delka < 0)
            {
                Delka = 0;
            }
            Update_gfx();
        }
    }

    public Takt Takt { get => takt; set => takt = value; }
    public Line_Ctrl Linka
    {
        get => linka;
        set => linka = value;
    }

    public void Update_delka()
    {
        if ((dist_x % CTRL.takt + Math.Pow(2, delka) * (2 - Math.Pow(2, -postfix))) > CTRL.takt)
        {
            Adapt(CTRL.takt - dist_x % CTRL.takt);
            Do_lig();
        }
    }

    public void Adapt(float input, bool adding = true)
    {
        if (input <= 0)
        {
            return;
        }
        float rem_input = input;
        int tmp_delka = -1;
        int targ_post = 0;
        for (int i = 4; i > -1; i--)
        {
            if (Math.Pow(2, i) <= rem_input)
            {
                if (tmp_delka < 0)
                {
                    rem_input = rem_input - (float)Math.Pow(2, i);
                    tmp_delka = i;
                }
                else
                {
                    rem_input = rem_input - ((float)Math.Pow(2, i)/16) * tmp_delka;
                    targ_post++;
                }
            }
            else if (tmp_delka > 0)
            {
                i = 0;
            }
        }
        int exdelka = delka;
        int expost = postfix;
        delka = tmp_delka;
        postfix = targ_post;
        bool shifted = false;
        if (rem_input > 0)
        {
            if (is_nota())
            {
                CTRL.Add_Nota(master, this, 1).Adapt(rem_input, false);
            }
            else
            {
                CTRL.Add_Pomlka(master, this, 1).Adapt(rem_input, false);
            }
            shifted = true;
        }
        if (adding)
        {
            Znak target = this;
            if (shifted)
            {
                target = next;
            }
            if (is_nota())
            {
                CTRL.Add_Nota(master, target, 1, (float)(Math.Pow(2, exdelka) * (2 - Math.Pow(2, -expost)) - input));
            }
            else
            {
                CTRL.Add_Pomlka(master, target, 1, (float)(Math.Pow(2, exdelka) * (2 - Math.Pow(2, -expost)) - input));
            }
        }
        Update_gfx();
    }

    public void Load(Znak Z)
    {
        Delka = Z.Delka;
    }

    public virtual void Calc_Pos()
    {
        int mod_y = 0;
        if (hand_id > 0)
        {
            mod_y = hand_id - 1 + Pos_y;
        }
        gameObject.transform.position = ref_point + new Vector3((Pos_x * Linka.nota_lenght), (Pos_y + mod_y) * Hand_Ctrl.vyska_linek, 0);
    }

    public void Swap_Pos(Znak target)
    {
        float tmp_x = target.Pos_x;
        int tmp_y = target.Pos_y;
        float tmp_dist = target.dist_x;
        target.dist_x = dist_x;
        target.Pos_y = Pos_y;
        target.Pos_x = Pos_x;
        dist_x = tmp_dist;
        Pos_y = tmp_y;
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
        if (i > 0)// mensi nez sestnanctiny neberem
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
        Pos_y = target.Pos_y;
        Pos_x = target.Pos_x;
    }

    public bool Bump_pos()
    {
        if (prev == null)
        {
            return false;
        }
        bool output = false;
        pos_x = prev.pos_x;
        if (prev.Delka == 4)
        {
            pos_x = pos_x + 1.5f;
        }
        else
        {
            pos_x++;
        }
        if (Linka != null)
        {
            Pos_y = Linka.id; 
        }
        else
        {
            pos_y = prev.pos_y;
            if (CTRL.get_linka(master, pos_y).full)
            {
                Pos_x = 0;
                pos_y++;
                linka = CTRL.get_linka(master, pos_y);
            }
        }
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
            CTRL.Do_Takty(master);
            Calc_Pos();
            Update_gfx();
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
    int topfix = 0;// 0 = nic 1 

    public override void Calc_Pos()
    {
        if (Linka == null)
        {
            return;
        }
        pos_y = Linka.id;
        int mod = 0;
        if (hand_id > 0)
        {
            mod = hand_id - 1 + Pos_y;
        }
        gameObject.transform.position = ref_point + new Vector3(Pos_x * Linka.nota_lenght, (Pos_y + mod) * Hand_Ctrl.vyska_linek + vyska * nota_height, 0);
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
            CTRL.Do_Takty(master);
            Calc_Pos();
            Update_gfx();
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
        if (Delka < 3)
        {
            gameObject.GetComponent<Image>().sprite = Gfx.Nota_ctvrt;
            if (Delka < 2)
            {
                prapor_GOs[0].SetActive(true);
                if (Delka == 0)
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
            if (Delka == 3)
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
    GameObject[] postfix_GO;

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
            CTRL.Do_Takty(master);
            Calc_Pos();
            Update_gfx();
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
            case 4:
                gameObject.GetComponent<Image>().sprite = Gfx.P_cela;
                break;
            case 3:
                gameObject.GetComponent<Image>().sprite = Gfx.P_pull;
                break;
            case 2:
                gameObject.GetComponent<Image>().sprite = Gfx.P_ctvrt;
                break;
            case 1:
                gameObject.GetComponent<Image>().sprite = Gfx.P_osmina;
                break;
            case 0:
                gameObject.GetComponent<Image>().sprite = Gfx.P_sestnactina;
                break;
            default:
                break;
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
        postfix_GO = new GameObject[3];
        postfix_GO[0] = gameObject.transform.GetChild(2).gameObject;
        postfix_GO[1] = gameObject.transform.GetChild(8).gameObject;
        postfix_GO[2] = gameObject.transform.GetChild(9).gameObject;
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
        Znak selected = start;
        while (selected != null)
        {
            output = output + selected.ToString();
            selected = selected.Next;
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

