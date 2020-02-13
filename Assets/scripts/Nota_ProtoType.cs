using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nota_ProtoType : MonoBehaviour
{
    const float modx = -75f;
    const float mody = 73.4f;

    public Change_ctrl master;

    public GameObject prefix_GO;
    public GameObject topfix_GO;
    public GameObject[] postfix_GO;

    public GameObject carka_licha;
    public GameObject carka_suda;
    GameObject[] carky;

    public GameObject[] prapor_GOs;

    public Grafix_bank Gfx;

    bool nota = true;

    int delka = 4;
    int vyska = 0;

    int prefix = 0;
    int postfix = 0;
    int topfix = 0;

    public bool Nota
    {
        get
        {
            return nota;
        }
        set
        {
            nota = value;
            Update_gfx();
        }
    }
    public int Delka
    {
        get
        {
            return delka;
        }
        set
        {
            if (value > -1)
            {
                if (value < 4)
                {
                    delka = value;  
                }
                else
                {
                    delka = 4;
                }
            }
            else
            {
                delka = 0;
            }
            Update_gfx();
        }
    }
    public int Vyska
    {
        get
        {
            return vyska;
        }
        set
        {
            vyska = value;
            master.Control_prefix();
            Update_gfx();
        }
    }
    public int Prefix
    {
        get
        {
            return prefix;
        }
        set
        {
            prefix = value;
            Update_gfx();
        }
    }
    public int Postfix
    {
        get
        {
            return postfix;
        }
        set
        {
            if (value > 3)
            {
                postfix = 3;
            }
            else if (value < 0)
            {
                postfix = 0;
            }
            else
            {
                postfix = value;
            }
            Update_gfx();
        }
    }
    public int Topfix
    {
        get
        {
            return topfix;
        }
        set
        {
            topfix = value;
            Update_gfx();
        }
    }

    public int[] Copy(Znak target)
    {
        int output = 0;
        int[] inputs = target.Copy();
        if (inputs[0] > 1)//ACORD SWAP do something about this
        {
            nota = true;
        }
        else
        {
            nota = false;
        }
        delka = inputs[1];
        if (inputs[0] > 0)
        {
            postfix = inputs[2];
            if (nota)
            {
                output = 1;
                prefix = inputs[3];
                topfix = inputs[4];
                vyska = inputs[5];
            }
        }

        carky = new GameObject[1];
        carky[0] = Instantiate(carka_licha, gameObject.transform);
        carky[0].SetActive(false);

        Update_gfx();
        return new int[4] { vyska, delka, output, postfix };
    }

    public int[] Send()
    {
        int[] output = new int[6] { 1, delka, postfix, prefix, topfix, vyska };
        if (nota)
        {
            output[0] = 2;//ACORD SWAP change this to 3
        }
        return output;
    }

    public void Update_gfx()
    {
        Vector3 master_scale = gameObject.transform.parent.parent.position;
        Vector3 ref_point = new Vector3(master_scale.x + modx, master_scale.y + mody);
        if (nota)
        {
            // vyska a prevraceni
            gameObject.transform.position = ref_point + new Vector3(0, vyska * Znak.nota_height);
            if (vyska > 1)
            {
                for (int i = 0; i < prapor_GOs.GetLength(0); i++)
                {
                    prapor_GOs[i].transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                prefix_GO.transform.position = new Vector3(-Math.Abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
                prefix_GO.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            }
            else
            {
                gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                prefix_GO.transform.position = new Vector3(Math.Abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
                prefix_GO.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                for (int i = 0; i < prapor_GOs.GetLength(0); i++)
                {
                    prapor_GOs[i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            //pomocne carky
            if (vyska < -3)
            {
                GameObject select;
                if (vyska % 2  == 0)
                {
                    select = carka_suda;
                    carka_suda.SetActive(true);
                    carka_licha.SetActive(false);
                }
                else
                {
                    select = carka_licha;
                    float dist = carka_suda.transform.position.y - carka_licha.transform.position.y;
                    carka_licha.transform.position = new Vector3(carka_suda.transform.position.x, carka_suda.transform.position.y + Math.Abs(dist), carka_suda.transform.position.z);
                    carka_licha.SetActive(true);
                    carka_suda.SetActive(false);
                }
                int targ_carek = (Math.Abs(vyska) - 2) / 2 - 1;
                for (int i = 0; i < carky.GetLength(0); i++)
                {
                    Destroy(carky[i]);
                }
                carky = new GameObject[targ_carek];
                for (int i = 0; i < targ_carek; i++)
                {
                    carky[i] = Instantiate(select, gameObject.transform, true);
                    carky[i].transform.position = new Vector3(select.transform.position.x, select.transform.position.y + 2 * Znak.nota_height * (i+1), select.transform.position.z);
                }
            }
            else if (vyska > 7)
            {
                GameObject select;
                if (vyska % 2 == 0)
                {
                    select = carka_suda;
                    carka_suda.SetActive(true);
                    carka_licha.SetActive(false);
                }
                else
                {
                    select = carka_licha;
                    float dist = carka_suda.transform.position.y - carka_licha.transform.position.y;
                    carka_licha.transform.position = new Vector3(carka_suda.transform.position.x, carka_suda.transform.position.y - Math.Abs(dist), carka_suda.transform.position.z);
                    carka_licha.SetActive(true);
                    carka_suda.SetActive(false);
                }
                int targ_carek = (Math.Abs(vyska) - 6) / 2 - 1;
                for (int i = 0; i < carky.GetLength(0); i++)
                {
                    Destroy(carky[i]);
                }
                carky = new GameObject[targ_carek];
                for (int i = 0; i < targ_carek; i++)
                {
                    carky[i] = Instantiate(select, gameObject.transform, true);
                    carky[i].transform.position = new Vector3(select.transform.position.x, select.transform.position.y - 2 * Znak.nota_height * (i+1), select.transform.position.z);
                }
            }
            else
            {
                carka_licha.SetActive(false);
                carka_suda.SetActive(false);
                for (int i = 0; i < carky.GetLength(0); i++)
                {
                    carky[i].SetActive(false);
                }
            }
            // prefix becka krizky
            switch (prefix)
            {
                case 1:
                    prefix_GO.SetActive(true);
                    prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_krizek;
                    break;
                case 2:
                    prefix_GO.SetActive(true);
                    prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_becko;
                    break;
                case 3:
                    prefix_GO.SetActive(true);
                    prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_neutral;
                    break;
                default:
                    prefix_GO.SetActive(false);
                    break;
            }
            //delka not
            if (Delka < 3)
            {
                gameObject.GetComponent<Image>().sprite = Gfx.Nota_ctvrt;
                if (Delka < 2) // prapory
                {
                    prapor_GOs[0].SetActive(true);
                    if (Delka == 0)
                    {
                        prapor_GOs[1].SetActive(true);
                    }
                    else
                    {
                        prapor_GOs[1].SetActive(false);
                    }
                }
                else
                {
                    prapor_GOs[1].SetActive(false);
                    prapor_GOs[0].SetActive(false);
                }
            }
            else
            {
                if (Delka == 3)
                {
                    gameObject.GetComponent<Image>().sprite = Gfx.Nota_pull;
                }
                else
                {
                    gameObject.GetComponent<Image>().sprite = Gfx.Nota_cela;
                }
                for (int i = 0; i < prapor_GOs.GetLength(0); i++)//vypne prapory
                {
                    prapor_GOs[i].SetActive(false);
                }
            }
            //topfix
            if (topfix != 0)
            {
                topfix_GO.SetActive(true);
            }
            else
            {
                topfix_GO.SetActive(false);
            }
        }
        else
        {
            //vyska
            gameObject.transform.position = ref_point;
            //delka pomka
            switch (delka)
            {
                case 4:
                    gameObject.GetComponent<Image>().sprite = Gfx.P_cela;
                    break;
                case 3:
                    gameObject.GetComponent<Image>().sprite = Gfx.P_pull;
                    break;
                case 2:
                    gameObject.GetComponent<Image>().sprite = Gfx.P_ctvrt;
                    break;
                case 1:
                    gameObject.GetComponent<Image>().sprite = Gfx.P_osmina;
                    break;
                case 0:
                    gameObject.GetComponent<Image>().sprite = Gfx.P_sestnactina;
                    break;
                default:
                    break;
            }

            //prefix
            prefix_GO.SetActive(false);
            //prapor
            for (int i = 0; i < prapor_GOs.GetLength(0); i++)
            {
                prapor_GOs[i].SetActive(false);
            }
            //topfix
            topfix_GO.SetActive(false);
        }
        //postfix
        for (int i = 0; i < postfix_GO.GetLength(0); i++)
        {
            if (postfix > i)
            {
                postfix_GO[i].SetActive(true);

            }
            else
            {
                postfix_GO[i].SetActive(false);
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
