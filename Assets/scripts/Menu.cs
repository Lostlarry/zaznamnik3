using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    Screen_Ctrl Ctrl;
    Save_Load S_L;

    void Start()
    {
        Ctrl = gameObject.GetComponent<Screen_Ctrl>();
        S_L = gameObject.GetComponent<Save_Load>();
    }
    void Update() { }

    public void Create()
    {
        Ctrl.Swap_canvas(11);
    }

    public void doCreate()
    {
        Ctrl.Swap_canvas(12);
    }

    public void cancelCreate()
    {
        Ctrl.Swap_canvas(4);
    }

    public void Load()
    {
        S_L.Load();
        Ctrl.Swap_canvas(13);
    }

    public void Save()
    {
        S_L.Save();
    }

    public void Play()
    {
        Ctrl.Swap_canvas(3);
    }

    public void unPlay()
    {
        Ctrl.Swap_canvas(1);
    }

    public void Main_Menu()
    {
        Ctrl.Swap_canvas(2);
    }
}
