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

    public Toggle pomlka;
    public Toggle nota;
    bool swaping = false;

    public Toggle[] prefix;

    public Nota_ProtoType prototype;

    public void Mod_vyska(int Change)
    {
        int vyska = prototype.Vyska + Change;
        prototype.Vyska = vyska;
        Vyska_txt.text = prototype.Vyska.ToString();
    }

    public void Input_vyska(string Target_vyska)
    {
        prototype.Vyska = int.Parse(Target_vyska);
    }

    public void Mod_delka(int Change)
    {
        int delka = prototype.Delka + Change;
        prototype.Delka = delka;
        Delka_txt.text = "1/" + Math.Pow(2, -delka).ToString();
    }

    public void Input_delka(string Target_delka)
    {
        char[] filter = new char[1] { '/' };
        string[] data = Target_delka.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        int output;
        if (int.TryParse(data[1],out output))
        {
            prototype.Delka = -Double_to_int(Math.Sqrt(output));
            Delka_txt.text = "1/" + Math.Pow(2, -prototype.Delka).ToString();
        }
        else
        {
            Delka_txt.text = "1/" + Math.Pow(2, -prototype.Delka).ToString();
        }
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
                Vyska_txt.enabled = is_nota;
            }
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
            pred.Activate(CTRL.predznamenani);
            int[] output = prototype.Copy(target);
            Vyska_txt.text = output[0].ToString();
            Delka_txt.text = output[1].ToString();
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
                int result = pred.Is_moded(prototype.Vyska);
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

    int Double_to_int(double input)
    {
        int output = 0;
        int mod = 1;
        if (input < 0)
        {
            mod = -1;
        }
        while(output+1 < input)
        {
            output = output + mod;
        }
        return output;
    }
}
