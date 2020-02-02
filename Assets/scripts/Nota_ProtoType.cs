using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nota_ProtoType : MonoBehaviour
{
    public Change_ctrl master;

    public GameObject prefix_GO;
    public GameObject topfix_GO;
    public GameObject postfix_GO;

    public GameObject[] prapor_GOs;

    public Grafix_bank Gfx;

    bool nota = true;

    int delka = 0;
    int vyska = 0;

    int prefix = 0;
    int postfix = 0;
    int topfix = 0;

    Vector3 default_pos;

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
            delka = value;
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
            postfix = value;
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
        if (inputs[0] > 1)
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
        Update_gfx();
        return new int[3] { vyska, delka, output };
    }

    public void Update_gfx()
    {
        // vyska a prevraceni
        gameObject.transform.position = default_pos + new Vector3(0, vyska * Znak.nota_height);
        if (vyska > 1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f); 
            prefix_GO.transform.position = new Vector3(-abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
            prefix_GO.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            prefix_GO.transform.position = new Vector3(abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
            prefix_GO.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        //pomocne carky
        if(vyska < -3)
        {

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
        if (delka < -1)
        {
            gameObject.GetComponent<Image>().sprite = Gfx.Nota_ctvrt;
            for (int i = -2; i < delka; i--)
            {
                prapor_GOs[abs(i) - 2].SetActive(true);
            }
        }
        else if (delka == -1)
        {
            gameObject.GetComponent<Image>().sprite = Gfx.Nota_pull;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = Gfx.Nota_cela;
        }
        for (int i = delka; abs(i) - 2 < prapor_GOs.GetLength(0); i--)
        {
            prapor_GOs[abs(i) - 2].SetActive(false);
        }
        //postfix
        if (postfix != 0)
        {
            postfix_GO.SetActive(true);
        }
        else
        {
            postfix_GO.SetActive(false);
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

    // Start is called before the first frame update
    void Start()
    {
        default_pos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float abs(float input)
    {
        if (input < 0)
        {
            return -input;
        }
        else
        {
            return input;
        }
    }

    int abs(int input)
    {
        if (input < 0)
        {
            return -input;
        }
        else
        {
            return input;
        }
    }
}
