using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Change_ctrl : MonoBehaviour
{
    public Hand_Ctrl CTRL;

    Znak target_note;

    public Button[] vyska_buttons;
    public InputField Vyska_txt;
    public InputField Delka_txt;
    public InputField postfix_txt;
    public Predznamenani pred;
    public GameObject klic;

    public Toggle pomlka;
    public Toggle nota;
    bool swaping = false;

    public Toggle[] prefix;

    public Nota_ProtoType prototype;

    public void Mod_vyska(int Change)
    {
        prototype.Vyska = prototype.Vyska + Change;
        Vyska_txt.text = prototype.Vyska.ToString();
        Control_prefix();
    }

    public void Input_vyska(string Target_vyska)
    {
        prototype.Vyska = int.Parse(Target_vyska);
        Control_prefix();
    }

    public void Mod_delka(int Change)
    {
        prototype.Delka = prototype.Delka + Change;
        Delka_txt.text = Math.Pow(2, prototype.Delka).ToString();
    }

    public void Input_delka(string Target_delka)
    {
        if (int.TryParse(Target_delka, out int output))
        {
            prototype.Delka = (int)Math.Floor(Math.Sqrt(output));
        }
        Delka_txt.text = Math.Pow(2, prototype.Delka).ToString();
    }

    public void Input_state(bool is_nota)
    {
        if (!swaping)
        {
            swaping = true;// we are modifing isOn value of the toggles which will triger this fucntion
            nota.isOn = is_nota;
            pomlka.isOn = !is_nota;
            prototype.Nota = is_nota;
            for (int i = 0; i < vyska_buttons.GetLength(0); i++)
            {
                vyska_buttons[i].interactable = is_nota;
            }
            Vyska_txt.interactable = is_nota;
            swaping = false;
            Control_prefix();
        }
    }

    public void Input_prefix(int index)
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

    public void Set_data(Znak target)
    {
        if (!swaping)
        {
            swaping = true;
            target_note = target;
            Vector3 pos = pred.transform.position;
            Destroy(pred.gameObject);
            Destroy(pred);
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

    public void Send_data()
    {
        if (target_note.is_nota() != prototype.Nota)
        {
            Znak.CTRL.Change(target_note);
        }
        target_note.Paste(prototype.Send());
    }

    public void Control_prefix()
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

    public void Mod_postfix(int Change)
    {
        prototype.Postfix = prototype.Postfix + Change;
        postfix_txt.text = prototype.Postfix.ToString();
    }

    public void Input_postfix(string Target_delka)
    {
        if (int.TryParse(Target_delka, out int output))
        {
            prototype.Postfix = output;
        }
        postfix_txt.text = prototype.Postfix.ToString();
    }



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
