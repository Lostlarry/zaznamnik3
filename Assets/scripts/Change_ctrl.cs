using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Change_ctrl : MonoBehaviour
{
    public Text Vyska_txt;
    public Text Delka_txt;

    public void Mod_vyska(int Change)
    {
        int vyska = int.Parse(Vyska_txt.text) + Change;

        Vyska_txt.text = vyska.ToString();
    }

    public void Set_vyska(int Target_vyska)
    {
        Vyska_txt.text = Target_vyska.ToString();
    }

    public void Input_vyska(string Target_vyska)
    {
    }

    public void Mod_delka(int Change)
    {
        int delka = int.Parse(Delka_txt.text) + Change;

        Delka_txt.text = delka.ToString();
    }

    public void Set_delka(int Target_delka)
    {
        Delka_txt.text = Target_delka.ToString();
    }

    public void Input_delka(string Target_delka)
    {
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
