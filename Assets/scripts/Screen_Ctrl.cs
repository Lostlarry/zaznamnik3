using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen_Ctrl : MonoBehaviour
{
    public GameObject workplace;
    public GameObject mainmenu;
    public GameObject playback;

    public GameObject new_menu;
    public GameObject Errorlog;
    public Save_Load saver;

    GameObject current;

    public void Swap_canvas(int target)//target is number two digit number first declaring target canvas and second declaring other procedures before the swap
    {

        current.SetActive(false);
        Errorlog.SetActive(false);
        switch (target)
        {
            case 1:
                current = workplace;
                current.SetActive(true);
                break;
            case 11:
                new_menu.transform.SetParent(current.transform);
                new_menu.SetActive(true);
                current.SetActive(true);
                break;
            case 12:
                int[] data = new_menu.GetComponent<New_Ctrl>().Getdata();
                gameObject.GetComponent<Hand_Ctrl>().Reset(data);
                current = workplace;
                new_menu.SetActive(false);
                current.SetActive(true);
                break;
            case 13:
                Hand_Ctrl original = gameObject.GetComponent<Hand_Ctrl>();
                Hand_Ctrl temp = new Hand_Ctrl(saver.getString(), out bool[] errors);
                errors = Log_Error(errors);
                if (!errors[0])//it didnt actualy load
                {
                    temp.transform.SetParent(gameObject.transform);
                    Destroy(original);
                    current = workplace;
                }
                if (errors[1])
                {
                    Errorlog.transform.SetParent(current.transform);
                    Errorlog.SetActive(true);
                }
                current.SetActive(true);
                break;
            case 2:
                current = mainmenu;
                current.SetActive(true);
                break;
            case 3:
                current = playback;
                current.SetActive(true);
                break;
            default:
                Debug.Log("unexpected value");
                current.SetActive(true);
                break;
        }
    }

    //hands count not loaded * hands count invalid * no hands loaded * invalid string * N/P when no hand tag * invalid datatype * subtype destring error * more hands then hand count * less hands then hand count
    bool[] Log_Error(bool[] errors)
    {
        string output = "Error Log:/n";
        bool is_critical = false;
        bool is_error = false;
        if (errors[0])
        {
            output = output + "File load: No hands count loaded!/n";
            is_critical = true;
            is_error = true;
        }
        if (errors[1])
        {
            output = output + "File load: invalid hands count!/n";
            is_critical = true;
            is_error = true;
        }
        if (errors[2])
        {
            output = output + "File load: no hands loaded!/n";
            is_critical = true;
            is_error = true;
        }
        if (errors[3])
        {
            output = output + "File load: invalid type tag./n";
            is_error = true;
        }
        if (errors[4])
        {
            output = output + "File load: loading into no hand./n";
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
    }
    void Update(){}
}
