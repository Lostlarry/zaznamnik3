using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Change_ctrl : MonoBehaviour
{
    public Text Vyska_txt;
    public Text Delka_txt;

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

    public void Set_data(Znak target)
    {
        int[] output = prototype.Copy(target);
        Vyska_txt.text = output[0].ToString();
        Delka_txt.text = output[1].ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
