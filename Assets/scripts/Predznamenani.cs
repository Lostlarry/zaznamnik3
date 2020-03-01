using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predznamenani : MonoBehaviour
{
    public GameObject[] Krizky;
    public GameObject[] Becka;

    int odchylka = 0;

    public void Activate(int amount)
    {
        odchylka = amount;
        if (amount > 0)
        {
            for (int i = 0; i < Krizky.GetLength(0); i++)
            {
                if (i < amount)
                {
                    Krizky[i].SetActive(true);
                }
                else
                {
                    Krizky[i].SetActive(false);
                }
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
            for (int i = 0; i < Becka.GetLength(0); i++)
            {
                if (i < amount)
                {
                    Becka[i].SetActive(true);
                }
                else
                {
                    Becka[i].SetActive(false);
                }
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

    public int Is_moded(int vyska, int klic)
    {
        if (klic == 1)
        {
            vyska = vyska - 5;
        }
        int mod_vyska = make_positive(vyska) % 7;
        if (odchylka > (mod_vyska + 1) * 2 % 7)
        {
            return 2;//krizek blokuje becko
        }
        else if(-odchylka > make_positive((2 - mod_vyska) * 2) % 7)
        {
            return 1;//becko blokuje krizek
        }
        return 3;//nothing blokuje odrazku
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int make_positive(int i)
    {
        while (i < 0)
        {
            i = i + 7;
        }
        return i;
    }
}
