using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Change_ctrl : MonoBehaviour
{
    public Hand_Ctrl CTRL;

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
        int vyska = int.Parse(Vyska_txt.text) + Change;
        prototype.Vyska = vyska;
        Vyska_txt.text = vyska.ToString();
    }

    public void Input_vyska(string Target_vyska)
    {
        prototype.Vyska = int.Parse(Target_vyska);
    }

    public void Mod_delka(int Change)
    {
        int delka = int.Parse(Delka_txt.text) + Change;
        prototype.Delka = delka;
        Delka_txt.text = delka.ToString();
    }

    public void Input_delka(string Target_delka)
    {
        prototype.Delka = int.Parse(Target_delka);
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
                prototype.Prefix = index + 1;
                prefix[prototype.Prefix].isOn = false;
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
    }

    public void Control_prefix()
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


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
