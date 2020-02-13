using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Change_ctrl : MonoBehaviour
{
    public Hand_Ctrl CTRL; // dulezite pro pristup k prototypum

    Znak target_note; // nota kerou modifikujeme

    public Button[] vyska_buttons; // tlacitka na modifikaci vysky potrebujemeje abychom je mohly vypnou kdyz pracujeme s pomlkou
    public InputField Vyska_txt;// textova pola vstup i vystup
    public InputField Delka_txt;
    public InputField postfix_txt;
    public Predznamenani pred;// predznamenani v change menu kopije z mastru cilove noty
    public GameObject klic;// klic to same jako predznamenani

    public Toggle pomlka; // prepinani nezi notou a pomlkou
    public Toggle nota;
    bool swaping = false; // dulezite pro manipulaci s toggly

    public Toggle[] prefix; // pole objekt prefixu

    public Nota_ProtoType prototype;  // prototyp multifunkcni noty

    public void Mod_vyska(int Change)// zmeni vysku o dane monozstvi    spousteno tlacitky v menu
    {
        prototype.Vyska = prototype.Vyska + Change;
        Vyska_txt.text = prototype.Vyska.ToString();
        Control_prefix();
    }

    public void Input_vyska(string Target_vyska) // zmeni vysku na uzivatelem napsanou hodnotu volano z Vyska_txt
    {
        prototype.Vyska = int.Parse(Target_vyska);
        Control_prefix();
    }

    public void Mod_delka(int Change)// zmeni vysku o dane monozstvi    spousteno tlacitky v menu
    {
        prototype.Delka = prototype.Delka + Change;
        Delka_txt.text = Math.Pow(2, prototype.Delka).ToString();
    }

    public void Input_delka(string Target_delka)// zmeni vysku na uzivatelem napsanou hodnotu volano z Delka_txt
    {
        if (int.TryParse(Target_delka, out int output))
        {
            prototype.Delka = (int)Math.Floor(Math.Log(output,2));
        }
        Delka_txt.text = Math.Pow(2, prototype.Delka).ToString();
    }

    public void Input_state(bool is_nota)
    {
        if (!swaping)
        {
            swaping = true;// modifikujeme isOn hodnotu togglu ktere spousteji tuto funkci takze se musime ujistit ze se vykona je jednou
            bool state;
            if (is_nota)
            {
                state = nota.isOn; 
            }
            else
            {
                state = !pomlka.isOn;
            }
            nota.isOn = state;
            pomlka.isOn = !state;
            prototype.Nota = state;
            for (int i = 0; i < vyska_buttons.GetLength(0); i++)
            {
                vyska_buttons[i].interactable = state;
            }
            Vyska_txt.interactable = state;
            swaping = false;
            Control_prefix();
        }
    }

    public void Input_prefix(int index)// zmeni prefix v zavislosti na vstupu  aktivnizadny nebo pouze jeden ze vstupu
    {
        if (!swaping)
        {
            swaping = true;
            bool state = prefix[index].isOn;
            if (state)
            {
                if (prototype.Prefix != 0)
                {
                    prefix[prototype.Prefix - 1].isOn = false; 
                }
                prototype.Prefix = index + 1;
            }
            else
            {
                prototype.Prefix = 0;
            }
            swaping = false;
        }
    }

    public void Set_data(Znak target)// zkopiruje data ciloveho znaku a upracy UI
    {
        if (!swaping)
        {
            swaping = true;
            target_note = target;
            Vector3 pos = pred.transform.position;
            Destroy(pred.gameObject);
            if (target_note.master.klic == 0)
            {
                pred = Instantiate(CTRL.proto_pred_h, transform, false).GetComponent<Predznamenani>();
                klic.GetComponent<Image>().sprite = Znak.Gfx.H_klic;
            }
            else
            {
                pred = Instantiate(CTRL.proto_pred_b, transform, false).GetComponent<Predznamenani>();
                klic.GetComponent<Image>().sprite = Znak.Gfx.B_klic;
            }
            pred.Activate(CTRL.predznamenani);
            int[] output = prototype.Copy(target);
            Vyska_txt.text = output[0].ToString();
            Delka_txt.text = Math.Pow(2, output[1]).ToString();
            postfix_txt.text = output[3].ToString();
            if (output[2] == 1)
            {
                nota.isOn = true;
                pomlka.isOn = false;
            }
            else
            {
                nota.isOn = false;
                pomlka.isOn = true;
            }
            swaping = false;
            Control_prefix();
            Input_prefix(prototype.Prefix);
        }
    }

    public void Send_data() // posle data a upravy cilovy znak
    {
        if (target_note.is_nota() != prototype.Nota)
        {
            target_note = Znak.CTRL.Change(target_note);
        }
        target_note.Paste(prototype.Send());
    }

    public void Control_prefix() // zabrani nekterym prefixum aby byli zvoleny
    {
        if (!swaping)
        {
            swaping = true;
            if (prototype.Nota)
            {
                int result = pred.Is_moded(prototype.Vyska, target_note.master.klic);
                for (int i = 0; i < prefix.GetLength(0); i++)
                {
                    if (result == i)
                    {
                        prefix[i].isOn = false;
                        prefix[i].interactable = false;//krizek blokuje becko
                    }
                    else 
                    {
                        prefix[i].isOn = false;
                        prefix[i].interactable = true;//krizek blokuje becko
                    }
                }
            }
            else
            {
                for (int i = 0; i < prefix.GetLength(0); i++)
                {
                    prefix[i].isOn = false;
                    prefix[i].interactable = false;//pomlka nema prefix
                }
            } 
        }
        swaping = false;
    }

    public void Mod_postfix(int Change)  //modifikuje postfix o change volano tlacitky
    {
        prototype.Postfix = prototype.Postfix + Change;
        postfix_txt.text = prototype.Postfix.ToString();
    }

    public void Input_postfix(string Target_delka)// zadani postfixu od uzivatele
    {
        if (int.TryParse(Target_delka, out int output))
        {
            prototype.Postfix = output;
        }
        postfix_txt.text = prototype.Postfix.ToString();
    }
    
    void Start() { }
    void Update() { }
}
