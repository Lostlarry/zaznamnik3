using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nota_ProtoType : MonoBehaviour
{
    public GameObject prefix_GO;
    public GameObject topfix_GO;
    public GameObject postfix_GO;

    public Grafix_bank Gfx;

    bool nota;

    int delka;
    int vyska;

    int prefix;
    int postfix;
    int topfix;

    Vector3 default_pos;

    public bool Nota { get => nota; set => nota = value; }
    public int Delka { get => delka; set => delka = value; }
    public int Vyska { get => vyska; set => vyska = value; }
    public int Prefix { get => prefix; set => prefix = value; }
    public int Postfix { get => postfix; set => postfix = value; }
    public int Topfix { get => topfix; set => topfix = value; }

    public int[] Copy(Znak target)
    {
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
                prefix = inputs[3];
                topfix = inputs[4];
                vyska = inputs[5];
            }
        }
        Update_gfx();
        return new int[2] { vyska, delka };
    }

    public void Update_gfx()
    {
        gameObject.transform.position = default_pos + new Vector3(0, vyska * Znak.nota_height);
        if (vyska > 1)
        {
            gameObject.transform.Rotate(0f, 0f, 180f);
            prefix_GO.transform.position = new Vector3(-abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
            prefix_GO.transform.Rotate(0f, 0f, 180f);
        }
        else
        {
            gameObject.transform.Rotate(0f, 0f, 0f);
            prefix_GO.transform.position = new Vector3(abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
            prefix_GO.transform.Rotate(0f, 0f, 0f);
        }


        switch (prefix)
        {
            case 1:
                prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_krizek;
                break;
            case 2:
                prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_becko;
                break;
            case 3:
                prefix_GO.GetComponent<Image>().sprite = Gfx.Prefix_neutral;
                break;
            default:
                break;
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
}
