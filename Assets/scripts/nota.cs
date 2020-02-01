using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected int pos_x = 0;
    protected int pos_y = 0;

    public Znak Prev { get => prev; set => prev = value; }
    public Znak Next { get => next; set => next = value; }
    public int Pos_x { get => pos_x; set => pos_x = value; }
    public int Pos_y { get => pos_y; set => pos_y = value; }

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
        gameObject.transform.position = gameObject.transform.position + new Vector3(pos_x * takt_width, pos_y * Hand_Ctrl.vyska_linek, 0);
    }

    public void Swap_Pos(Znak target)
    {
        int tmp_x = target.pos_x;
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
        char[] filter = new char[1];
        filter[0] = ',';
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
        pos_x = pos_x + 2^prev.delka;
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
        char[] filter = new char[1];
        filter[0] = ',';
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

    protected override void Update_gfx()
    {
        base.Update_gfx();
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
        char[] filter = new char[1];
        filter[0] = ',';
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

    protected override void Do_lig()
    {

    }
}

class Acord : Znak
{
    Nota start;

    int vyska = 0; // relativne ku spodni radce relativne k prvni note
    
    int topfix = 0;// 0 = nic 1 

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
        string output = "";
        Nota selected = start;
        while (selected != null)
        {
            output = output + selected.ToString();
        }
        return "A," + delka + "," + vyska + "," + output + ";";
    }

    public override bool is_nota()
    {
        return true;//maybe
    }

    public override bool FromString(string input)
    {
        bool error = false;
        char[] filter = new char[1];
        filter[0] = ',';
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

            if (int.TryParse(data[2], out output))
            {
                vyska = output;
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
        return new int[4] { 3, delka, topfix, vyska };
    }
}

