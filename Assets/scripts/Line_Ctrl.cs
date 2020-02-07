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
    public GameObject[] takty;

    public int id = 0;

    public Line_Ctrl Next { get => next; set => next = value; }
    public Line_Ctrl Prev { get => prev; set => prev = value; }

    void Start()
    {
        takty = new GameObject[Znak.takts_per_line];
    }
    void Update() { }

    public void SetActive(bool state)
    {
        klic.SetActive(state);
        end.SetActive(state);
        pred.SetActive(state);
        gameObject.SetActive(state);
        for (int i = 0; i < takty.GetLength(0); i++)
        {
            if (takty[i] != null)
            {
                takty[i].SetActive(state); 
            }
        }
    }
}
