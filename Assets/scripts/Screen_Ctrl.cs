using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen_Ctrl : MonoBehaviour
{
    public GameObject workplace;// canvas kde je samotny editor
    public GameObject mainmenu;// canvas s hlavnim menu

    public GameObject new_menu;// menu pro tvorbu noveho projektu
    public GameObject change_menu;// menu na modifikaci not
    public GameObject Errorlog;// log na errory
    public Save_Load saver; // clasa na ukladani/nacitani dat

    public Hand_Ctrl hand;// clasa kontrolujici manipulaci s notamy
    GameObject current; //canvas (tedy workplace nebo mainmenu) ktery je prave aktivni

    public void Swap_canvas(int target)//target je dvouciferne cislo ve kterem prvni cislice udava cilove nastaveni a druha cislice specifikuje dalsi operace pred samotnym prepnutim
    {
        current.SetActive(false);
        Errorlog.SetActive(false);
        switch (target)
        {
            case 1://prepinani na workplace, nepouzite
                current = workplace;
                break;
            case 11://otevre menu na tvoreni noveho projektu
                new_menu.transform.SetParent(current.transform, false);// toto mone lze otevrit z hlavniho manu i z projektu
                new_menu.SetActive(true);
                break;
            case 12://vystup z newmenu      resetuje hand control a vytvori novy projekt podle specifikaci
                int[] data = new_menu.GetComponent<New_Ctrl>().Getdata();
                gameObject.GetComponent<Hand_Ctrl>().Reset(data);
                current = workplace;
                new_menu.SetActive(false);
                break;
            case 13://nacte ze souboru
                Hand_Ctrl original = gameObject.GetComponent<Hand_Ctrl>();
                Hand_Ctrl temp = gameObject.AddComponent<Hand_Ctrl>();// vytvori docasny Hand_ctrl objekt
                temp.Load_decoys();
                temp.Give_data(saver.getString(), out bool[] errors);// ktery se pokusi nacist dony soubor
                errors = Log_Error(errors);//vytvori error log a rozradi errory
                temp.Reset();// a pote je odstranen
                Destroy(temp);
                Znak.CTRL = original;
                if (!errors[0])//soubor je nactitelny
                {
                    original.Give_data(saver.getString(), out errors);// nacte data
                    current = workplace;
                    if (errors[1])// ukaze eroor log pokud jsou v souboru chyby
                    {
                        Errorlog.transform.SetParent(current.transform);
                        Errorlog.SetActive(true);
                    }
                }
                else// soubor s kritickymi errory
                {
                    Errorlog.transform.SetParent(current.transform);
                    Errorlog.SetActive(true);
                }
                break;
            case 2:// navrat do hlavniho menu
                current = mainmenu;
                break;
            case 4://otevre menu na modifikaci not
                change_menu.GetComponent<Change_ctrl>().Set_data(hand.get_selected());
                change_menu.SetActive(true);
                break;
            default:
                Debug.Log("unexpected value");
                break;
        }
        current.SetActive(true);
    }


    public void log_save(int state)// vrati stav nacitani/ukladani (nerika nic o stavu soubor) volano ze save_load clasy
    {
        string output = "Log:/n";
        switch (state)
        {
            case 1:
                output = output + "No Savefile found!";
                break;
            case 2:
                output = output + "File loaded";
                break;
            case 3:
                output = output + "File saved";
                break;
            default:
                break;
        }
        Errorlog.GetComponentInChildren<Text>().text = output;
        Errorlog.transform.SetParent(current.transform);
        Errorlog.SetActive(true);
    }

    //hands count not loaded * hands count invalid * no hands loaded * invalid string * N/P when no hand tag * invalid datatype * subtype destring error * more hands then hand count * less hands then hand count
    bool[] Log_Error(bool[] errors)
    {
        string output = "Error Log:/n";//napise popis erroru do logu(UI) 
        bool is_critical = false;// pokud je true tak soubor nebudeme nacitat
        bool is_error = false;// pokeuj je true tak je neco spatne se souborem ale neni to nic duleziteho a ztracena data jsou nahrazena defaultnimy hodnotami
        if (errors[0])
        {
            output = output + "File load: No hands count loaded!/n";//pocet rukou mebyl deklarovan
            is_critical = true;
            is_error = true;
        }
        if (errors[1])
        {
            output = output + "File load: invalid hands count!/n";//
            is_critical = true;
            is_error = true;
        }
        if (errors[2])
        {
            output = output + "File load: no hands loaded!/n";//zadne ruce nebyly nacteny
            is_critical = true;
            is_error = true;
        }
        if (errors[3])
        {
            output = output + "File load: invalid type tag./n";// 
            is_error = true;
        }
        if (errors[4])
        {
            output = output + "File load: loading into no hand./n";// nacita Znak kdyz nebyla nactena ruka
            is_error = true;
        }
        if (errors[5])
        {
            output = output + "File load: failed to parse intager string./n";
            is_error = true;
        }
        if (errors[6])
        {
            output = output + "File load: note destring failure./n";
            is_error = true;
        }
        if (errors[7])
        {
            output = output + "File load: attepted to load more hands than then decalred by hand count./n";
            is_error = true;
        }
        if (errors[8])
        {
            output = output + "File load: loaded less hands then decalred by hand count./n";
            is_error = true;
        }

        Errorlog.GetComponentInChildren<Text>().text = output;
        return new bool[2]{ is_critical, is_error };
    }

    void Start()
    {
        current = mainmenu;
        workplace.SetActive(false);
        saver.SCR_ctrl = this;
    }
    void Update(){}
}
