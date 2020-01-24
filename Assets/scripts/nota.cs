using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Znak : MonoBehaviour
{
    protected int delka = 1;

    public Holder master;

    protected Znak prev;
    protected Znak next;

    public Znak Prev { get => prev; set => prev = value; }
    public Znak Next { get => next; set => next = value; }

    public void Load(Znak Z)
    {
        delka = Z.delka;
    }

    public virtual void Calc_Pos(int line_id, int nota_id)
    {
        gameObject.transform.up = gameObject.transform.up - new Vector3(0, line_id*4.4f);
        gameObject.transform.right = gameObject.transform.right + new Vector3(0, nota_id * 4.4f); ;
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
    }

    public virtual void Nota_Short(int i = 1)
    {
        delka = delka - 1;
    }

    public override string ToString()
    {
        return "Z,"+delka+";";
    }

    public virtual bool FromString(string input, Holder hold)
    {
        master = hold;
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

    

    void Start() { }
    void Update() { }
}
public class Nota : Znak
{
    int vyska = 0; // relativne ku spodni radce

    int prefix = 0;// 0= nic 1 = krizky 2 = becka 3 = cista 
    int postfix = 0;// 0 = nic 1 az 3 tecky(prida pulku delky)

    public override void Calc_Pos(int line_id, int nota_id)
    {
        gameObject.transform.up = gameObject.transform.up;
        gameObject.transform.right = gameObject.transform.right;
    }

    public override void Nota_Up(int i = 1)
    {
        vyska = i + vyska;
    }

    public override void Nota_Down(int i = 1)
    {
        vyska = i - vyska;
    }

    public override string ToString()
    {
        return "N,"+delka+","+vyska+","+prefix+","+postfix+";";
    }

    public override bool is_nota()
    {
        return true;
    }

    public override bool FromString(string input, Holder hold)
    {
        master = hold;
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
}

class Pomlka : Znak
{

    int postfix = 0;// 0 = nic 1 az 3 tecky(prida pulku delky)

    public Pomlka(int delka = 1)
    {
        this.delka = delka;
    }

    public override string ToString()
    {
        return "N," + delka + "," + postfix + ";";
    }

    public override bool is_nota()
    {
        return false;
    }

    public override bool FromString(string input, Holder hold)
    {
        master = hold;
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
}
