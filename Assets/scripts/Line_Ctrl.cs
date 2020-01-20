using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Ctrl : MonoBehaviour
{
    Line_Ctrl next;
    Line_Ctrl prev;

    int id = -1;

    public Line_Ctrl Next { get => next; set => next = value; }
    public Line_Ctrl Prev { get => prev; set => prev = value; }

    public int Id 
    {
        get
        {
            return id;
        }
        set
        {
            if (id == -1)
            {
                id = value;
            }
        }
    }

    void Start() { }
    void Update() { }
}
