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
    }

    public void Input_vyska(string Target_vyska)
    {
        prototype.Vyska = int.Parse(Target_vyska);
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
                vyska_buttons[i].enabled = is_nota;
            }
            Vyska_txt.enabled = is_nota;
            swaping = false;
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
        }
    }

    public void Send_data()
    {
        target_note.Paste(prototype.Send());
    }

    public void Control_prefix()
    {
        if (!swaping)
        {
            swaping = true;
            if (nota)
            {
                int result = pred.Is_moded(prototype.Vyska, target_note.master.klic);
                if (result == 1)
                {
                    prefix[1].enabled = false;//krizek blokuje becko
                    prefix[1].isOn = false;
                }
                else if (result == 2)
                {
                    prefix[0].enabled = false;//becko blokuje krizek
                    prefix[0].isOn = false;
                }
                else
                {
                    prefix[2].enabled = false;//zadny predznamanani blokuje odrazku
                    prefix[2].isOn = false;
                }
            }
            else
            {
                for (int i = 0; i < prefix.GetLength(0); i++)
                {
                    prefix[i].enabled = false;//becko blokuje krizek
                    prefix[i].isOn = false;
                }
            } 
        }
        swaping = false;
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
