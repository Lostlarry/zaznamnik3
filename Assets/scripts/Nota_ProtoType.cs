using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nota_ProtoType : MonoBehaviour
{
    public GameObject prefix_GO;
    public GameObject topfix_GO;
    public GameObject postfix_GO;

    bool nota;

    int delka;
    int vyska;

    int prefix;
    int postfix;
    int topfix;

    public void Copy(Znak target)
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

    }

    public void Update_gfx()
    {

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
