using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand_Ctrl : MonoBehaviour
{
    public const float vyska_linek = -48.6f; // vz

    public HUD_ctrl Select_HUD;// menu okolo zvolene noty
    public HUD_ctrl End_HUD; // menu na tvorbu nove noty
    public GameObject paper;  // objekt ktery je parrentem vsemnotam a linkam
    public GameObject takt_top; // horni cislice taktu
    public GameObject takt_bot; // dolni cislice taktu

    public GameObject proto_nota;    // prototypy game objektu ktery jesou zkopirovany kdyz je potrebujeme
    public GameObject proto_linka;  
    public GameObject proto_klic; 
    public GameObject proto_end;
    public GameObject proto_pred_h;
    public GameObject proto_pred_b;
    public GameObject proto_takt;
    public GameObject proto_lig;
    public GameObject proto_lig_side;

    static GameObject decoy_linka;//kopije prototypu abychom nemusely kopirovat vsechny prototypy  pouzito pri nacitani
    static GameObject decoy_nota;
    static HUD_ctrl decoy_HUD;
    static GameObject decoy_paper;
    static GameObject decoy_takt;
    static GameObject decoy_pred;

    public Holder[] hands; // holder pro kazdou ruku  ale druha ruka je vypnuta takze vice mene na nic

    int selected_hand = 0;  // vybrna ruka

    public int predznamenani = 0; //0 = cdur kladny jsou krizky, zaporny jsou becka
    public int takt = 16; // delka taktu

    public string Give_String()// transformuje vsechny data do stringu
    {
        string dat = "";
        for (int i = hands.GetLength(0) - 1; i >= 0; i--)
        {
            hands[i].vybrany = hands[i].Prvni;
            while (hands[i].vybrany != null)
            {
                dat = dat + hands[i].vybrany.Give_String();
                hands[i].vybrany = hands[i].vybrany.Next;
            }
            dat = "H," + hands[i].klic + ";" + dat;
        }

        dat = hands.GetLength(0) + "," + predznamenani + "," + takt + ";" + dat;

        return dat;
    }

    public Hand_Ctrl Give_data(string input, out bool[] errors)
    {
        //hands count not loaded * hands count invalid * no hands loaded * invalid string * N/P when no hand tag * invalid datatype * subtype destring error * more hands then hand count * less hands then hand count
        errors = new bool[9] { false, false, false, false, false, false, false, false, false };
        Reset();
        char[] filter = new char[1] { ';' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        filter[0] = ',';
        string[] localdata = data[0].Split(filter, StringSplitOptions.RemoveEmptyEntries);
        Int32 output = new Int32();
        if (int.TryParse(localdata[0], out output))
        {
            if (output > 0)
            {
                int tmp_hand_count = output;

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

                Create(tmp_hand_count);

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
                                hands[index].first.Update_gfx(hands[index]);
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

                        case "A"://not in use
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
        for (int i = 0; i < hands.GetLength(0); i++)
        {
            Recalc(hands[i].Prvni);
        }
        return this;
    }

    public Nota Add_Nota(Holder hand, Znak target = null, float adapt = -1)// vytvori novu notu dane ruce(hand), za danou notou (nebo za posledni notou pokud target je null) 
    {
        if (hand == null)// nemelo by se to stat  ale pro pripad ze hand je null tak program nespadne
        {
            hand = hands[0];
        }
        bool middle = true;// je true pokud se snazime dat do prostred not
        if (target == null)
        {
            target = hand.Posledni;
            middle = false;
        }
        GameObject GO = Instantiate(proto_nota, paper.transform, false);// vytvori grafiku
        GO.name = "nota " + hand.not + " " + hand.id;
        GO.SetActive(true);
        hand.not++;
        Nota made = GO.AddComponent<Nota>();
        made.Do_data();
        made.master = hand;
        made.Hand_id = hands.GetLength(0) - 1 + hand.id;
        if (adapt < 0)//priradi delku
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
            made.Adapt(adapt, true);
        }
        if (target != null)// prida nove vytvorenou notu do odkazoveho seznamu
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
        made.Update_delka();// udela upravy  jako napr. taktove cary a posu HUDu
        hand.vybrany = made;
        Select_hand(hand);
        Select_HUD.Adjust_HUD(made);
        return made;
    }

    public Pomlka Add_Pomlka(Holder hand, Znak target = null, float adapt = -1) // to same jako predchozi ale vytavrime pomlku
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
        if (adapt < 0)
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
            made.Adapt(adapt, true);
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
        made.Update_delka();
        hand.vybrany = made;
        Select_HUD.Adjust_HUD(made);
        Select_hand(hand);
        return made;
    }

    private void Select_hand(int i)// zvoli ruku o danem id
    {
        selected_hand = i;// pouzivato jenom reset takze to je v pohode
    }

    private void Select_hand(Holder hold)// vybere dany holder pouzivano k najiti holderu z mastra znaku
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

    public void Relay_signal(int cmd_id)//prijma signal od HUDu
    {
        switch (cmd_id)
        {
            case 1:
                hands[selected_hand].vybrany.Nota_Up(); // moznosti 1 az 4 nejsou pouzity
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
            case 7://metoda change je pouzita ale ne volana odtud
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
            case 11:
                hands[selected_hand].vybrany.Do_lig();
                break;
            case 12:
                hands[selected_hand].vybrany.Do_lig(true);
                break;
            default:
                Debug.Log("unstandard signal");
                break;
        }
    }

    private void Shift_prev(Znak target = null)// vymeni notu s tou pred ni
    {
        bool lig = false;
        if (target.Lig_prev != null)
        {
            lig = true;
        }
        if (target == null)
        {
            target = hands[selected_hand].vybrany;
        }
        Znak swap = target.Prev;
        if (swap != null)
        {
            swap.Next = target.Next;// prehodi objekty v retezci
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
        Recalc(target);// prepocita pozice takty a hud
        Do_Takty(target.master);
        Select_HUD.Adjust_HUD(target);
        if (lig)
        {
            target.Do_lig(true);
        }
    }

    private void Shift_next(Znak target = null)// stejny jako predchozi fce ale vymenujeme s predchozim znakem
    {
        bool lig = false;
        if (target.Lig_next != null)
        {
            lig = true;
        }
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
        Recalc(swap);
        Do_Takty(target.master);
        Select_HUD.Adjust_HUD(target);
        if (lig)
        {
            target.Do_lig();
        }
    }

    private void Remove(Znak target = null)//odstranime vybranou notu
    {
        if (target == null)
        {
            target = hands[selected_hand].vybrany;
        }
        Holder hold = target.master;
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
                hold.Posledni = target.Prev;
            }
            hold.vybrany = target.Prev;
        }
        else if (target.Next != null)
        {
            Recalc(target.Next);
            target.Next.Prev = null;
            hold.Prvni = target.Next;
            hold.vybrany = target.Next;
        }
        else
        {
            hold.Prvni = null;
            hold.Posledni = null;
            Add_Nota(hold);
        }
        Destroy(target.gameObject);
        if (hold.Prvni == null)// pokud jsme smazali posledni notu na radku takvytorime novou jinak se kod zblazni
        {
            Add_Nota(hold);
        }
        Do_Takty(hold);
        Select_HUD.Adjust_HUD(hold.Posledni);
        End_HUD.Adjust_HUD(hold.Posledni);
    }

    public Znak Change(Znak target = null)// za meni notu za pomlku a obracene
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
        made.Load(target);//zavola vsechny dulezite fce
        made.Calc_Pos();
        made.Update_gfx();
        Select_HUD.Adjust_HUD(made);
        End_HUD.Adjust_HUD(made.master.Posledni);
        Destroy(target);
        return made;
    }

    public void Reset(int[] data)//smaze vsechny noty a linky a nacte z pole data
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

    public void Reset()// smaze vsechny linky a noty
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

    public void Create(int[] data)// vyvori novy projekt s jenim radkem a notou pro kazdou ruku
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
            takt_top.GetComponent<Text>().text = (tmp / 2).ToString();
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
    public void Create(int hand_count)// zjednodusena verze predchozi methody pouzita pri nacitani
    {
        hands = new Holder[hand_count];
        int tmp = takt / 2;
        if (tmp % 2 == 1)
        {
            takt_bot.GetComponent<Text>().text = "8";
            takt_top.GetComponent<Text>().text = tmp.ToString();
        }
        else
        {
            takt_bot.GetComponent<Text>().text = "4";
            takt_top.GetComponent<Text>().text = (tmp / 2).ToString();
        }
        for (int i = 0; i < hand_count; i++)
        {
            hands[i] = new Holder();
            hands[i].id = i;
            Add_line(hands[i]);
            hands[i].selected.takty = new GameObject[Znak.takts_per_line];
        }
    }

    void Add_line(Holder hold)// prida novou linku vcetne klic zakonceni a predznamenani
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
        if (proto_nota != null)
        {
            Znak.ref_point = proto_nota.transform.position;//pri nacitani Hand_ctrl nexistule dost dlouho aby udelal v tomhle zmatek
            decoy_linka = proto_linka;
            decoy_nota = proto_nota;
            decoy_HUD = Select_HUD;
            decoy_paper = paper;
            decoy_takt = takt_top;
            decoy_pred = proto_pred_b;
        }
    }
    void Update() { }

    public void Load_decoys()// nacte prototypy z decoyu
    {
        Znak.CTRL = this;
        Select_HUD = decoy_HUD;
        End_HUD = decoy_HUD;
        paper = decoy_paper;
        takt_top = decoy_takt;
        takt_bot = decoy_takt;
        proto_pred_h = decoy_pred;
        proto_pred_b = decoy_pred;

        proto_nota = decoy_nota;
        proto_linka = decoy_linka;
        proto_klic = decoy_linka;
        proto_end = decoy_linka;
        proto_takt = decoy_linka;
        proto_lig = decoy_linka;
        proto_lig_side = decoy_linka;
    }

    public void Recalc(Znak target)// prepocita pozici kazde noty pocinaje tou zadanou
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
            Do_Takty(hold);
            End_HUD.Adjust_HUD(hold.Posledni);
        }

    }

    public Znak get_selected() // vybere danou notu
    {
        return hands[selected_hand].vybrany;
    }

    public void Do_Takty(Holder hold) // spocita takty
    {
        int mod = Znak.takts_per_line + 1;
        float Pos_t = hold.Posledni.Pos_x;  // vzdalenost od prevni noty v taktech
        if (Pos_t > hold.last_x)
        {
            for (int i = (int)Math.Floor(hold.last_x) + 1; i < (int)Math.Floor(Pos_t) + 1; i++)
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
                        hold.selected.takty[(modulo) - 1].transform.position = new Vector3(proto_nota.transform.position.x + (modulo) * Znak.takt_width + modulo * Znak.cara_width - 30f, hold.selected.transform.position.y);
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
        else if (Pos_t < hold.last_x)
        {
            for (int i = (int)Math.Floor(hold.last_x); i > (int)Math.Floor(Pos_t) - 1; i--)
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
        hold.last_x = Pos_t;
    }

    public GameObject get_lig(int state, Znak target)
    {
        GameObject output;
        if (state == 0)
        {
            output = Instantiate(proto_lig, target.transform, false);
        }
        else
        {
            output = Instantiate(proto_lig_side, target.transform, false);
            if (state == 2)
            {
                output.GetComponent<Image>().sprite = Znak.Gfx.Lig_left;
            }
        }
        return output;
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

