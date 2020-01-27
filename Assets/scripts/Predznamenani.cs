using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predznamenani : MonoBehaviour
{
    public GameObject[] Krizky;
    public GameObject[] Becka;

    public void Activate(int amount)
    {
        if (amount > 0)
        {
            for (int i = 0; i + 1 < amount; i++)
            {
                Krizky[i].SetActive(true);
            }
            for (int i = amount; i < Krizky.GetLength(0); i++)
            {
                Krizky[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < Krizky.GetLength(0); i++)
            {
                Krizky[i].SetActive(false);
            }
        }
        if (amount < 0)
        {
            amount = amount * -1;
            for (int i = 0; i + 1 < amount; i++)
            {
                Becka[i].SetActive(true);
            }
            for (int i = amount; i < Becka.GetLength(0); i++)
            {
                Becka[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < Becka.GetLength(0); i++)
            {
                Becka[i].SetActive(false);
            }
        }
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
