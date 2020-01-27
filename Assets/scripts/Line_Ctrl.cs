using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Ctrl : MonoBehaviour
{
    Line_Ctrl next;
    Line_Ctrl prev;

    public GameObject klic;
    public GameObject end;
    public GameObject pred;

    public Line_Ctrl Next { get => next; set => next = value; }
    public Line_Ctrl Prev { get => prev; set => prev = value; }

    void Start() { }
    void Update() { }
}
