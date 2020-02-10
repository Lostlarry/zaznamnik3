using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand_Ctrl : MonoBehaviour
{
    public const float vyska_linek = -48.6f;

    public HUD_ctrl Select_HUD;
    public HUD_ctrl End_HUD;
    public GameObject paper;
    public GameObject takt_top;
    public GameObject takt_bot;

    public GameObject proto_nota;
    public GameObject proto_linka;
    public GameObject proto_klic;
    public GameObject proto_end;
    public GameObject proto_pred_h;
    public GameObject proto_pred_b;
    public GameObject proto_takt;

    Holder[] hands;

    int selected_hand = 0;

    public int predznamenani = 0; //0 = cdur kladny jsou krizky, zaporny jsou becka
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

        dat = hands.GetLength(0)+ "," + predznamenani + "," + takt + ";" + dat;

        return dat;
    }

    public Hand_Ctrl Give_data(string input, out bool[] errors)
    {
        //hands count not loaded * hands count invalid * no hands loaded * invalid string * N/P when no hand tag * invalid datatype * subtype destring error * more hands then hand count * less hands then hand count
        errors = new bool[9] {false, false, false, false, false, false, false, false, false};
        Reset();
        char[] filter = new char[1] { ';'};
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
                    predznamenani = output;
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
                            errors[6] = (Add_Nota(hands[index]).FromString(data[i]) || errors[6]);
                            break;

                        case "P":
                            if (index == -1)
                            {
                                errors[4] = true;
                                break;
                            }
                            errors[6] = (Add_Pomlka(hands[index]).FromString(data[i]) || errors[6]);
                            break;

                        case "A":
                            if (index == -1)
                            {
                                errors[4] = true;
                                break;
                            }
                            //errors[6] = (Add_Pomlka(hands[index]).FromString(data[i]) || errors[6]);
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
        return this;
    }

    public Nota Add_Nota(Holder hand, Znak target = null, int input_delka = -1)
    {
        if (hand == null)
        {
            hand = hands[0];
        }
        bool middle = true;
        if (target == null)
        {
            target = hand.Posledni;
            middle = false;
        }
        GameObject GO = Instantiate(proto_nota, paper.transform, false);
        GO.name = "nota " + hand.not + " " + hand.id;
        GO.SetActive(true);
        hand.not++;
        Nota made = GO.AddComponent<Nota>();
        made.Do_data();
        made.master = hand;
        made.Hand_id = hands.GetLength(0) - 1 + hand.id;
        if (input_delka == -1)
        {
            int tmp_delka = 1;
            for (int i = 4; i > -1; i--)
            {
                if (Math.Pow(2, i) <= takt)
                {
                    tmp_delka = i;
                    i = -1;
                }
            }
            made.Delka = tmp_delka;
        }
        else
        {
            made.Delka = input_delka;
        }
        if (target != null)
        {
            if (middle && target.Next != null)
            {
                made.Next = target.Next;
                Recalc(made.Next);
            }
            else
            {
                hand.Posledni = made;
            }
            target.Next = made;
            made.Prev = target;
            made.Bump_pos();
        }
        else
        {
            hand.Prvni = made;
            hand.Posledni = made;
            made.Calc_Pos();
        }
        hand.vybrany = made;
        Select_hand(hand);
        Select_HUD.Adjust_HUD(made);
        return made;
    }

    public Pomlka Add_Pomlka(Holder hand, Znak target = null, int input_delka = -1)
    {
        if (hand == null)
        {
            hand = hands[0];
        }
        bool middle = true;
        if (target == null)
        {
            target = hand.Posledni;
            middle = false;
        }
        GameObject GO = Instantiate(proto_nota, paper.transform, false);
        GO.name = "note " + hand.not + " " + hand.id;
        hand.not++;
        Pomlka made = GO.AddComponent<Pomlka>();
        made.Do_data();
        made.master = hand;
        made.Hand_id = hands.GetLength(0) - 1 + hand.id;
        if (input_delka == -1)
        {
            int tmp_delka = 1;
            for (int i = 4; i > -1; i--)
            {
                if (Math.Pow(2, i) <= takt)
                {
                    tmp_delka = i;
                    i = -1;
                }
            }
            made.Delka = tmp_delka;
        }
        else
        {
            made.Delka = input_delka;
        }
        if (target != null)
        {
            if (middle && target.Next != null)
            {
                made.Next = target.Next;
                Recalc(made.Next);
            }
            else
            {
                hand.Posledni = made;
            }
            target.Next = made;
            made.Prev = target;
            made.Bump_pos();
        }
        else
        {
            hand.Prvni = made;
            hand.Posledni = made;
            made.Calc_Pos();
        }
        hand.vybrany = made;
        Select_HUD.Adjust_HUD(made);
        Select_hand(hand);
        return made;
    }

    private void Select_hand(int i)
    {
        selected_hand = i;// used by reset only so its fine
    }

    private void Select_hand(Holder hold)
    {
        for (int i = 0; i < hands.GetLength(0); i++)
        {
            if (hands[i] == hold)
            {
                selected_hand = i;
                End_HUD.Adjust_HUD(hold.Posledni);
            }
        }
    }

    public void Select_note(GameObject input)
    {
        Znak select = input.GetComponent<Znak>();
        select.master.vybrany = select;
        Select_hand(select.master);
        Select_HUD.Adjust_HUD(select);

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
                Recalc(hands[selected_hand].vybrany);
                break;
            case 4:
                hands[selected_hand].vybrany.Nota_Short();
                Recalc(hands[selected_hand].vybrany);
                break;
            case 5:
                Shift_next();
                break;
            case 6:
                Shift_prev();
                break;
            case 7://not in use
                Change();
                break;
            case 8:
                Remove();
                break;
            case 9:
                Add_Nota(hands[selected_hand]);
                break;
            case 10:
                Add_line(hands[selected_hand]);
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
            target.Swap_Pos(swap);
        }
        Select_HUD.Adjust_HUD(target);
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
            target.Swap_Pos(swap);
        }
        Select_HUD.Adjust_HUD(target);

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
        made.Load(target);
        Destroy(target);
    }

    public void Reset(int[] data)
    {
        Select_hand(0);
        predznamenani = 0;
        takt = 1;

        if (hands != null)
        {
            for (int i = 0; i < hands.GetLength(0); i++)
            {
                if (hands[i].last != null)
                {
                    Line_Ctrl LC_select = hands[i].last;
                    while (LC_select.Prev != null)
                    {
                        LC_select = LC_select.Prev;
                        Destroy(LC_select.Next.klic);
                        Destroy(LC_select.Next.end);
                        Destroy(LC_select.Next.pred);
                        Destroy(LC_select.Next.gameObject);
                    }
                    Destroy(LC_select.klic);
                    Destroy(LC_select.end);
                    Destroy(LC_select.pred);
                    Destroy(LC_select.gameObject);
                }

                if (hands[i].Posledni != null)
                {
                    hands[i].vybrany = hands[i].Posledni;
                    while (hands[i].vybrany.Prev != null)
                    {
                        hands[i].vybrany = hands[i].vybrany.Prev;
                        Destroy(hands[i].vybrany.Next.gameObject);
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
                    Line_Ctrl LC_select = hands[i].last;
                    while (LC_select.Prev != null)
                    {
                        LC_select = LC_select.Prev;
                        Destroy(LC_select.Next.klic);
                        Destroy(LC_select.Next.end);
                        Destroy(LC_select.Next.pred);
                        Destroy(LC_select.Next.gameObject);
                    }
                    Destroy(LC_select.klic);
                    Destroy(LC_select.end);
                    Destroy(LC_select.pred);
                    Destroy(LC_select.gameObject);
                }

                if (hands[i].Posledni != null)
                {
                    hands[i].vybrany = hands[i].Posledni;
                    while (hands[i].vybrany.Prev != null)
                    {
                        hands[i].vybrany = hands[i].vybrany.Prev;
                        Destroy(hands[i].vybrany.Next.gameObject);
                    }
                    Destroy(hands[i].vybrany.gameObject);
                }
            } 
        }
    }

    public void Create(int[] data)
    {
        hands = new Holder[data[0]];
        predznamenani = data[1];
        takt = data[2];
        int tmp = takt / 2;
        if (tmp % 2 == 1)
        {
            takt_bot.GetComponent<Text>().text = "8";
            takt_top.GetComponent<Text>().text = tmp.ToString();
        }
        else
        {
            takt_bot.GetComponent<Text>().text = "4";
            takt_top.GetComponent<Text>().text = (tmp/2).ToString();

        }
        for (int i = 0; i < data[0]; i++)
        {
            hands[i] = new Holder();
            hands[i].klic = data[i + 3];
            hands[i].id = i;
            Add_line(hands[i]);
            Add_Nota(hands[i]);
        }
    }

    void Add_line(Holder hold)
    {
        GameObject novy = Instantiate(proto_linka, paper.transform, false);
        novy.gameObject.name = "linka " + hold.linek + " " + hold.id;
        GameObject klic = Instantiate(proto_klic, paper.transform, false);
        klic.gameObject.name = "klic " + hold.linek + " " + hold.id;
        GameObject end = Instantiate(proto_end, paper.transform, false);
        end.gameObject.name = "end " + hold.linek + " " + hold.id;
        GameObject pred;
        if (hold.klic == 0)
        {
            pred = Instantiate(proto_pred_h, paper.transform, false);
            klic.GetComponent<Image>().sprite = Znak.Gfx.H_klic;
        }
        else
        {
            pred = Instantiate(proto_pred_b, paper.transform, false);
            klic.GetComponent<Image>().sprite = Znak.Gfx.B_klic;
        }
        pred.GetComponent<Predznamenani>().Activate(predznamenani);
        pred.gameObject.name = "pred " + hold.linek + " " + hold.id;
        int mod = hold.linek;
        if (hands.GetLength(0) > 1)
        {
            mod = hold.id + hold.linek * 2;
        }
        novy.transform.position = novy.transform.position + new Vector3(0, mod * vyska_linek, 0);
        klic.transform.position = klic.transform.position + new Vector3(0, mod * vyska_linek, 0);
        end.transform.position = end.transform.position + new Vector3(0, mod * vyska_linek, 0);
        pred.transform.position = pred.transform.position + new Vector3(0, mod * vyska_linek, 0);
        Line_Ctrl target = hold.last;
        Line_Ctrl LC = novy.GetComponent<Line_Ctrl>();
        LC.id = hold.linek;
        LC.klic = klic;
        LC.end = end;
        LC.pred = pred;
        LC.SetActive(true);
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
            hold.selected = LC;
        }
        hold.linek++;
    }

    void Start()
    {
        Znak.CTRL = this;
        Znak.ref_point = proto_nota.transform.position;
    }
    void Update(){}

    public void Recalc(Znak target)
    {
        if (target != null)
        {
            Holder hold = target.master;
            while (target != null)
            {
                target.Bump_pos();
                target.Update_delka();
                target = target.Next;
            }
            End_HUD.Adjust_HUD(hold.Posledni);
        }
        
    }

    public Znak get_selected()
    {
        return hands[selected_hand].vybrany;
    }

    public void Do_Takty(Holder hold)
    {
        int mod = Znak.takts_per_line + 1;
        float Dist_t = hold.Posledni.Dist_x / takt;  // vzdalenost od prevni noty v taktech
        if (Dist_t > hold.last_x)
        {
            for (int i = (int)Math.Floor(hold.last_x)+ 1; i < (int)Math.Floor(Dist_t) + 1; i++)
            {
                int modulo = i % mod;
                if (modulo != 0)
                {
                    if (hold.selected.takty[(modulo) - 1] != null)
                    {
                        hold.selected.takty[(modulo) - 1].SetActive(true);
                    }
                    else
                    {
                        hold.selected.takty[(modulo) - 1] = Instantiate(proto_takt, hold.selected.transform, false);
                        hold.selected.takty[(modulo) - 1].SetActive(true);
                        hold.selected.takty[(modulo) - 1].transform.position =new Vector3(proto_nota.transform.position.x + (modulo) * Znak.takt_width + modulo * Znak.cara_width - 40f, hold.selected.transform.position.y);
                    }
                }
                else
                {
                    if (i != 0)
                    {
                        Line_Ctrl LC_select = hold.first;
                        for (int x = 0; x < hold.Posledni.Pos_y; x++)
                        {
                            if (LC_select.Next == null)
                            {
                                Add_line(hold);
                            }
                            LC_select = LC_select.Next;
                        }
                        hold.selected = hold.selected.Next;
                    }
                }
            }
        }
        else if (Dist_t < hold.last_x)
        {
            for (int i = (int)Math.Floor(hold.last_x); i > (int)Math.Floor(Dist_t) -1; i--)
            {
                int modulo = i % mod;
                if (modulo != 0)
                {
                    if (hold.selected.takty[(modulo) - 1] != null)
                    {
                        hold.selected.takty[modulo - 1].SetActive(false);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        hold.selected = hold.first;
                    }
                    else
                    {
                        hold.selected.SetActive(false);
                        hold.selected = hold.selected.Prev;
                    }
                }
            }
        }
        hold.last_x = Dist_t;
    }
}

public class Holder
{
    public int id;
    public int not = 0;
    public int linek = 0;

    public Znak vybrany;
    public Znak Prvni;
    public Znak Posledni;

    public Line_Ctrl selected; // hold last active
    public Line_Ctrl first;
    public Line_Ctrl last;

    public float last_x = 0;
    public int klic = 1;//(0 = houslovy, 1 = bassovy)
    
}

