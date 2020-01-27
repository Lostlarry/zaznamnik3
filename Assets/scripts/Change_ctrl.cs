using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Change_ctrl : MonoBehaviour
{
    public Hand_Ctrl CTRL;

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

    public void Input_state(int index)
    {
        if (!swaping)
        {
            swaping = true;
            if (index == 0)
            {
                nota.isOn = !nota.isOn;
            }
            else
            {
                pomlka.isOn = !pomlka.isOn;
            }
            prototype.Nota = !prototype.Nota;
            swaping = false;
        }
    }

    public void Input_prefix(int index)
    {
        if (!swaping)
        {
            swaping = true;
            bool state = prefix[index].isOn;
            for (int i = 0; i < prefix.GetLength(0); i++)
            {
                if (i != index)
                {
                    prefix[i].isOn = false;
                }
            }
            if (state)
            {
                prototype.Prefix = 0;
            }
            else
            {
                prototype.Prefix = index + 1;
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

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
