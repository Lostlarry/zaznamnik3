using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class New_Ctrl : MonoBehaviour
{
    Transform[] kids;
    void Start()
    {
        Init();
    }
    void Update() { }

    void Init()
    {
        int length = gameObject.transform.childCount;
        kids = new Transform[length];
        for (int i = 0; i < length; i++)
        {
            kids[i] = gameObject.transform.GetChild(i);
        }
    }

    public int[] Getdata()
    {
        int hand_amt = 1;
        if (kids[0].GetComponent<Toggle>().isOn)
        {
            hand_amt++;
        }
        int[] data = new int[hand_amt + 3];
        data[0] = hand_amt;
        data[1] = kids[1].GetComponent<Dropdown>().value - 7;
        data[2] = (kids[2].GetComponent<Dropdown>().value + 3) * 2;
        for (int i = 0; i < hand_amt; i++)
        {
            data[i+3] = kids[i+3].GetComponent<Dropdown>().value;
        }
        return data;
    }
}
