using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grafix_bank : MonoBehaviour//drzi odkazi na sprity pouzivane jinymi clasamy aby tama nebyla tuna kodu
{
    public Sprite Prefix_krizek;
    public Sprite Prefix_becko;
    public Sprite Prefix_neutral;

    public Sprite Nota_cela;
    public Sprite Nota_pull;
    public Sprite Nota_ctvrt;
    public Sprite Nota_prapor;
    public Sprite Nota_carka;

    public Sprite P_cela;
    public Sprite P_pull;
    public Sprite P_ctvrt;
    public Sprite P_osmina;
    public Sprite P_sestnactina;

    public Sprite B_klic;
    public Sprite H_klic;
    public Sprite Lig_left;
   
    void Start()
    {
        Znak.Gfx = this;
    }
    
    void Update(){ }
}
