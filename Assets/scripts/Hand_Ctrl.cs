using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Hand_Ctrl : MonoBehaviour
{
    const int notes_per_line = 30;

    public HUD_ctrl HUD;
    public GameObject paper;
    public GameObject proto_nota;
    public GameObject proto_linka;
    
    Holder[] hands;

    int selected_hand = 0;

    int not = 0;
    int linek = 0;

    public int tonina = 0; //(0 = cdur)
    public int takt = 1;

    public override string ToString()
    {

        string dat = "";
        for (int i = hands.GetLength(0); i > 0; i--)
        {
            hands[i].vybrany = hands[i].Prvni;
            while (hands[i].vybrany != null)
            {
                dat = dat + hands[i].vybrany.ToString();
            }
            dat = "H," + hands[i].klic + ";" + dat; 
        }

        dat = hands.GetLength(0)+ "," + tonina + "," + takt + ";" + dat;

        return dat;
    }

    public Hand_Ctrl(string input, out bool[] errors)
    {
        //hands count not loaded * hands count invalid * no hands loaded * invalid string * N/P when no hand tag * invalid datatype * subtype destring error * more hands then hand count * less hands then hand count
        errors = new bool[9] {false, false, false, false, false, false, false, false, false};
        Reset();
        char[] filter = new char[1];
        filter[0] = ';';
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        filter[0] = ',';
        string[] localdata = data[0].Split(filter, StringSplitOptions.RemoveEmptyEntries);
        Int32 output = new Int32();
        if (int.TryParse(localdata[0], out output))
        {
            if (output > 0)
            {
                hands = new Holder[output];

                if (int.TryParse(localdata[1], out output))
                {
                    tonina = output;
                }
                else
                {
                    errors[5] = true;
                }

                if (int.TryParse(localdata[2], out output))
                {
                    takt = output;
                }
                else
                {
                    errors[5] = true;
                }

                int index = -1;
                for (int i = 1; i < data.GetLength(0); i++)
                {
                    localdata = data[i].Split(filter, StringSplitOptions.RemoveEmptyEntries);
                    switch (localdata[0])
                    {
                        case "H":
                            if (hands.GetLength(0) != index)
                            {
                                index++;

                                if (int.TryParse(localdata[1], out output))
                                {
                                    hands[index].klic = output;
                                }
                                else
                                {
                                    errors[5] = true;
                                }
                            }
                            else
                            {
                                errors[7] = true;
                            }
                            break;

                        case "N":
                            if (index == -1)
                            {
                                errors[4] = true;
                                break;
                            }
                            errors[6] = (Add_Nota().FromString(data[i], hands[index]) || errors[6]);
                            break;

                        case "P":
                            if (index == -1)
                            {
                                errors[4] = true;
                                break;
                            }
                            errors[6] = (Add_Pomlka().FromString(data[i], hands[index]) || errors[6]);
                            break;

                        default:
                            errors[3] = true;
                            Debug.Log("attepted to destring invalid string");
                            break;
                    }
                }
                if (index == -1)
                {
                    errors[2] = true;
                }
                else if (index + 1 < hands.GetLength(0))
                {
                    errors[8] = true;
                    Holder[] temp = new Holder[index + 1];
                    for (int i = 0; i < index + 1; i++)
                    {
                        temp[i] = hands[i];
                    }
                    hands = temp;
                }
            }
            else
            {
                errors[1] = true;
            }
        }
        else
        {
            errors[0] = true;
        }
    }

    Nota Add_Nota(int handid = 0, Znak target = null)
    {
        Holder hand;
        hand = hands[handid];
        Select_hand(handid);
        bool middle = true;
        if (target == null)
        {
            target = hand.Posledni;
            middle = false;
        }
        GameObject GO = Instantiate(proto_nota, paper.transform, false);
        GO.name = "note " + not;
        not++;
        Nota made = GO.AddComponent<Nota>();
        made.master = hand;
        if (target != null)
        {
            if (middle)
            {
                made.Next = target.Next;
                Recalc(made.Next);
            }
            else
            {
                made.master.Posledni = made;
            }
            target.Next = made;
            made.Prev = target; 
        }
        else
        {
            made.master.Prvni = made;
            made.master.Posledni = made;
        }
        hand.vybrany = made;
        HUD.Adjust_HUD(GO);
        return made;
    }

    Nota Add_Nota(Holder hand, Znak target = null)
    {
        Select_hand(hand);
        bool middle = true;
        if (target == null)
        {
            target = hand.Posledni;
            middle = false;
        }
        GameObject GO = Instantiate(proto_nota, paper.transform, false);
        GO.name = "note " + not;
        not++;
        Nota made = GO.AddComponent<Nota>();
        made.master = hand;
        if (target != null)
        {
            if (middle)
            {
                made.Next = target.Next;
                Recalc(made.Next);
            }
            else
            {
                made.master.Posledni = made;
            }
            target.Next = made;
            made.Prev = target;
        }
        else
        {
            made.master.Prvni = made;
            made.master.Posledni = made;
        }
        hand.vybrany = made;
        HUD.Adjust_HUD(GO);
        return made;
    }

    Pomlka Add_Pomlka(int handid = 0, Znak target = null)
    {
        Holder hand;
        hand = hands[handid];
        Select_hand(handid);
        bool middle = true;
        if (target == null)
        {
            target = hand.Posledni;
            middle = false;
        }
        GameObject GO = Instantiate(proto_nota, paper.transform, false);
        GO.name = "note " + not;
        not++;
        Pomlka made = GO.AddComponent<Pomlka>();
        made.master = hand;
        if (target != null)
        {
            if (middle)
            {
                made.Next = target.Next;
                Recalc(made.Next);
            }
            target.Next = made;
            made.Prev = target;
        }
        hand.vybrany = made;
        HUD.Adjust_HUD(GO, false);
        return made;
    }

    private void Select_hand(int i)
    {
        selected_hand = i;
    }

    private void Select_hand(Holder hold)
    {
        for (int i = 0; i < hands.GetLength(0); i++)
        {
            if (hands[i] == hold)
            {
                selected_hand = i;
            }
        }
    }

    public void Select_note(GameObject input)
    {
        Znak selected = input.GetComponent<Znak>();
        while (selected.Prev != null)
        {
            selected = selected.Prev;
        }
        for(int i = 0; i < hands.GetLength(0);i++)
        {
            if (selected == hands[i].Prvni)
            {
                hands[i].vybrany = input.GetComponent<Znak>();
                Select_hand(i);
            }
        }
    }

    public void Relay_signal(int cmd_id)
    {
        switch (cmd_id)
        {
            case 1:
                hands[selected_hand].vybrany.Nota_Up();
                break;
            case 2:
                hands[selected_hand].vybrany.Nota_Down();
                break;
            case 3:
                hands[selected_hand].vybrany.Nota_Long();
                break;
            case 4:
                hands[selected_hand].vybrany.Nota_Short();
                break;
            case 5:
                Shift_next();
                break;
            case 6:
                Shift_prev();
                break;
            case 7:
                Change();
                break;
            case 8:
                Remove();
                break;
            case 9:
                Add_Nota();
                break;
            default:
                Debug.Log("unstandard signal");
                break;
        }
    }

    private void Shift_prev(Znak target = null)
    {
        if (target == null)
        {
            target = hands[selected_hand].vybrany;
        }
        Znak swap = target.Prev;
        if (swap != null)
        {
            swap.Next = target.Next;
            target.Next = swap;
            target.Prev = swap.Prev;
            swap.Prev = target;
            if (target.Prev != null)
            {
                target.Prev.Next = target;
            }
            else
            {
                target.master.Prvni = target;
            }
            if (swap.Next != null)
            {
                swap.Next.Prev = swap;
            }
            else
            {
                target.master.Posledni = swap;
            }
        }
        Recalc(swap);
        HUD.Adjust_HUD(target.gameObject);
    }

    private void Shift_next(Znak target = null)
    {
        if (target == null)
        {
            target = hands[selected_hand].vybrany;
        }
        Znak swap = target.Next;
        if (swap != null)
        {
            swap.Prev = target.Prev;
            target.Prev = swap;
            target.Next = swap.Next;
            swap.Next = target;
            if (target.Next != null)
            {
                target.Next.Prev = target;
            }
            else
            {
                target.master.Posledni = target;
            }
            if (swap.Prev != null)
            {
                swap.Prev.Next = swap;
            }
            else
            {
                target.master.Prvni = swap;
            }
        }
        Recalc(target);
        HUD.Adjust_HUD(target.gameObject);

    }

    private void Remove(Znak target = null)
    {
        if (target == null)
        {
            target = hands[selected_hand].vybrany;
        }
        Znak swap = target.Next;
        if (target.Prev != null)
        {
            if (target.Next != null)
            {
                target.Next.Prev = target.Prev;
                target.Prev.Next = target.Next;
                Recalc(target.Next);
            }
            else
            {
                target.Prev.Next = null;
                target.master.Posledni = target.Prev;
            }
        }
        else if (target.Next != null)
        {
            Recalc(target.Next);
            target.Next.Prev = null;
            target.master.Prvni = target.Next;
        }
        else
        {
            target.master.Prvni = null;
            target.master.Posledni = null;
        }
       Destroy(target.gameObject);

    }

    private void Change(Znak target = null)
    {
        if (target == null)
        {
            target = hands[selected_hand].vybrany;
        }
        Znak made;
        if (target.is_nota())
        {
            made = target.gameObject.AddComponent<Pomlka>();
        }
        else
        {
            made = target.gameObject.AddComponent<Nota>();
        }
        made.Prev = target.Prev;
        made.Next = target.Next;
        if (target.Prev != null)
        {
            target.Prev.Next = made;
        }
        if (target.Next != null)
        {
            target.Next.Prev = made;
            Recalc(target.Next);
        }
        made.master = target.master;
        made.Load(target);
        Destroy(target);
    }

    public void Reset(int[] data)
    {
        Select_hand(0);
        not = 0;
        linek = 0;
        tonina = 0;
        takt = 1;

        if (hands != null)
        {
            for (int i = 0; i < hands.GetLength(0); i++)
            {
                if (hands[i].last != null)
                {
                    hands[i].selected = hands[i].last;
                    while (hands[i].selected.Prev != null)
                    {
                        hands[i].selected = hands[i].selected.Prev;
                        Destroy(hands[i].selected.Prev.gameObject);
                    }
                    Destroy(hands[i].selected.gameObject);
                }

                if (hands[i].Posledni != null)
                {
                    hands[i].vybrany = hands[i].Posledni;
                    while (hands[i].vybrany.Prev != null)
                    {
                        hands[i].vybrany = hands[i].vybrany.Prev;
                        Destroy(hands[i].vybrany.Prev.gameObject);
                    }
                    Destroy(hands[i].vybrany.gameObject);
                }
            }
        }
        Create(data);
    }

    public void Reset()
    {
        if (hands != null)
        {
            for (int i = 0; i < hands.GetLength(0); i++)
            {
                if (hands[i].last != null)
                {
                    hands[i].selected = hands[i].last;
                    while (hands[i].selected.Prev != null)
                    {
                        hands[i].selected = hands[i].selected.Prev;
                        Destroy(hands[i].selected.Prev.gameObject);
                    }
                    Destroy(hands[i].selected.gameObject);
                }

                if (hands[i].Posledni != null)
                {
                    hands[i].vybrany = hands[i].Posledni;
                    while (hands[i].vybrany.Prev != null)
                    {
                        hands[i].vybrany = hands[i].vybrany.Prev;
                        Destroy(hands[i].vybrany.Prev.gameObject);
                    }
                    Destroy(hands[i].vybrany.gameObject);
                }
            } 
        }
    }

    public void Create(int[] data)
    {
        hands = new Holder[data[0]];
        tonina = data[1];
        takt = data[2];
        for (int i = 0; i < data[0]; i++)
        {
            hands[i] = new Holder();
            Add_line(hands[i]);
            Add_Nota(hands[i]);
            Recalc(hands[i].Posledni);
        }
    }

    void Add_line(Holder hold)
    {
        GameObject novy = Instantiate(proto_linka, paper.transform, false);
        novy.gameObject.name = "linka " + linek;
        novy.SetActive(true);
        linek++;
        novy.transform.up = novy.transform.up * linek;
        Line_Ctrl target = hold.last;
        Line_Ctrl LC = novy.GetComponent<Line_Ctrl>();
        if (target != null)
        {
            target.Next = LC;
            LC.Prev = target;
            hold.last = LC;
        }
        else
        {
            hold.last = LC;
            hold.first = LC;
        }
    }

    void Start(){}
    void Update(){}

    void Recalc(Znak target)
    {
        Znak select = target.master.Prvni;
        int line_id = 0;
        int nota_id = 0;
        while (select != target)
        {
            nota_id++;
            if (nota_id > notes_per_line)
            {
                nota_id = 0;
                line_id++;
            }
            select = select.Next;
        }
        while (select != null)
        {
            select.Calc_Pos(line_id, nota_id);
            nota_id++;
            if (nota_id > notes_per_line)
            {
                nota_id = 0;
                line_id++;
            }
            select = select.Next;
        }
    }
}

public class Holder
{
    public Znak vybrany;
    public Znak Prvni;
    public Znak Posledni;

    public Line_Ctrl selected;
    public Line_Ctrl first;
    public Line_Ctrl last;

    public int klic = 1;//(1 = houslovy, 0 = bassovy)
    public Holder()
    {
    }
}

