using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Znak : MonoBehaviour
{
    public static Grafix_bank Gfx;// odkaz na uloziste spritu
    public static Hand_Ctrl CTRL;
    public static Vector3 ref_point;// pozice prvni noty (prvni nota na prvnim radku   tedy pos_x = 0, pos_y = 0) 

    public static int takts_per_line = 4;// nevissi id takze taktu je o jedna vic
    public static float takt_width = 79.2f;// vzdalenos prvni noty od konce taktu
    public static float cara_width = 8f; // sirka vyhrazena pro taktovou caru
    public const float nota_height = 3.7f; // vzdalenost noty o jedna vyssi podle osi x

    protected int delka = 4; // exponent 2 (2 na 4 je 16 (v sestnactnach) tj cely ctyr ctvrtovy takt, atd.)
    protected int postfix = 0;// 0 = nic 1 az 3 = mnozsti tecek delka s teckama = (delka * (2-2^-pocet tecek))   kazda tecka pridava pulku delky te predchozi (nebo noty v pripade prvni tecky) 

    public Holder master;

    protected Znak prev; // predchozi a dalsi Znak pro navigaci 
    protected Znak next; // ukazuje na Znaky pro stejnou ruku

    protected GameObject lig_next; // odkaz na ligaturu s dalsi notou
    protected GameObject lig_prev;
    protected GameObject[] postfix_GO; //pole odkazu na na posfix (tecky)

    protected float pos_x = 0; // Vzdalenost od prvniho znaku na radky v taktech
    protected int pos_y = 0; // id radku na kterem je tento znak
    protected int hand_id = 0; // dulezite por modifikace v dusledku vice rukou

    // gettery a seetery

    public Znak Prev { get => prev; set => prev = value; } 
    public Znak Next { get => next; set => next = value; }

    public float Pos_x
    {
        get
        {
            return pos_x;
        }
        set
        {
            if (pos_x != value) // minimalizuje pocet volani DOtakty funkace
            {
                pos_x = value;
                CTRL.Do_Takty(master);
            }
        }
    }

    public int Pos_y { get => pos_y; set => pos_y = value; }
    public int Postfix
    {
        get => postfix;
        set
        {
            if (value < 0)// zajisti ze nepocitame se silene detajlnimi daty
            {
                postfix = 0;
            }
            else if (value > 3)
            {
                postfix = 3;
            }
            else
            {
                postfix = value;
            }
        }
    }


    public int Hand_id { get => hand_id; set => hand_id = value; }
    public int Delka
    {
        get
        {
            return delka;
        }
        set
        {
            delka = value;
            if (Delka < 0)// zajisti ze nepocitame se silene detajlnimi daty
            {
                Delka = 0;
            }
            Update_gfx();
        }
    }

    public virtual GameObject Lig_next
    {
        get => lig_next;
        set { }
    }

    public virtual GameObject Lig_prev
    {
        get => lig_prev;
        set { }
    }

    public void Update_delka()
    {
        if ((pos_x % 1 * CTRL.takt + Math.Pow(2, delka) * (2 - Math.Pow(2, -postfix))) > CTRL.takt)// soucet delek not v tomto taktu + delka teto noty > delka jednoho taktu
        {
            Adapt(CTRL.takt - pos_x % 1 * CTRL.takt);
            Do_lig();
        }
    }

    public void Adapt(float input, bool made = false)// input = cilova delka teto noty, made = true pokud volano z teto funkce
    { 
        float rem_input = input;
        int tmp_delka = -1;
        int targ_post = 0;
        for (int i = 4; i > -1; i--)// pokusi se vytvorit co nejvetsi notu nizsi nez input
        {
            if (Math.Pow(2, i) <= rem_input)
            {
                if (tmp_delka < 0)
                {
                    rem_input = rem_input - (float)Math.Pow(2, i);
                    tmp_delka = i;
                }
                else
                {
                    rem_input = rem_input - (float)Math.Pow(2, -i) * (float)Math.Pow(2, tmp_delka);
                    targ_post++;
                }
            }
            else if (tmp_delka > 0)
            {
                i = 0;
            }
        }
        bool shifted = false;
        if (rem_input > 0)// pokud se nepodarilo vytvorit notu o delky input
        {
            if (is_nota())
            {
                CTRL.Add_Nota(master, this, rem_input);
            }
            else
            {
                CTRL.Add_Pomlka(master, this, rem_input);
            }
            shifted = true;
        }
        Znak target = this;
        int Ex_delka = delka;
        int Ex_postfix = postfix;
        if (made) //pokud byla nota vyvorena touto funkci tak jeji puvodni delka je irelevantni
        {
            Ex_delka = tmp_delka;
            Ex_postfix = targ_post;
        }
        delka = tmp_delka;
        postfix = targ_post;
        if (Math.Pow(2, Ex_delka) * (2 - Math.Pow(2, -Ex_postfix)) > input)// 
        {
            if (shifted)
            {
                target = next;
            }
            if (is_nota())
            {
                CTRL.Add_Nota(master, target, (float)(Math.Pow(2, Ex_delka) * (2 - Math.Pow(2, -Ex_postfix)) - input));
            }
            else
            {
                CTRL.Add_Pomlka(master, target, (float)(Math.Pow(2, Ex_delka) * (2 - Math.Pow(2, -Ex_postfix)) - input));
            }
        }
        Update_gfx();
    }

    public void Load(Znak Z)// nacte data z jineha znaku  pouzito pouze pri zmene noty na pomlku a obracene
    {
        pos_x = Z.pos_x;
        pos_y = Z.pos_y;
        delka = Z.delka;
        master = Z.master;
        next = Z.next;
        prev = Z.prev;
        if (prev != null)
        {
            prev.next = this; 
        }
        else
        {
            master.Prvni = this;
        }
        if (next != null)
        {
            next.prev = this; 
        }
        else
        {
            master.Posledni = this;
        }
        Do_data();
        Update_delka();
    }

    public virtual void Calc_Pos() //spocita pozici Grafiky
    {
        int mod = 0;
        if (hand_id > 0)
        {
            mod = hand_id - 1 + pos_y;
        }                                                         //delka not pred touto          //rezerva pro taktove cary
        gameObject.transform.position = ref_point + new Vector3((Pos_x * takt_width) + (float)Math.Floor(pos_x) * cara_width, (pos_y + mod) * Hand_Ctrl.vyska_linek, 0);
    }

    public void Swap_Pos(Znak target) // zameni pozice dvou Znaku
    {
        float tmp_x = target.Pos_x;
        int tmp_y = target.pos_y;
        target.pos_y = pos_y;
        target.Pos_x = Pos_x;
        pos_y = tmp_y;
        Pos_x = tmp_x;
        Calc_Pos();
        target.Calc_Pos();
    }


    public virtual bool is_nota()//vrati true pokud toto je nota
    {
        return false;
    }

    public virtual void Nota_Up(int i = 1)
    {
        Update_gfx();
    }

    public virtual void Nota_Down(int i = 1)
    {
        Update_gfx();
    }

    public virtual void Nota_Long(int i = 1)
    {

        Update_delka();
        Update_gfx();
    }

    public virtual void Nota_Short(int i = 1)
    {
        if (i > 0)// mensi nez sestnanctiny neberem
        {
            Delka = Delka - 1;
        }

        Update_delka();
        Update_gfx();
    }

    public virtual string Give_String()// vrati string ze ktereho nasledujici funkce vytovri objekt se stejnymi hodnotami  pouzito pri ukladani
    {
        return "Z,"+Delka+";";
    }

    public virtual bool FromString(string input) //nastavy data ze vstutpu   vrati true pokud jsou data ve spatnem formatu
    {
        bool error = false;
        char[] filter = new char[1] { ',' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        if (data[0] == "Z")
        {
            Int32 output = new Int32();
            if (int.TryParse(data[1], out output))
            {
                Delka = output;

                Update_delka();
            }
            else
            {
                error = true;
            }
        }
        else
        {
            error = true;
            Debug.Log("attepted to destring non Znak string");
        }
        return error;
    }

    public virtual void Do_lig(bool prev = false)// udela ligaturu s nalsedujici notou  delanic u vseho ostatniho
    {
    }

    public void Transfer(Znak target)// zkopiruje dulezita data z daneho Znaku
    {
        Prev = target.Prev;
        Next = target.Next;
        if (target.Prev != null)
        {
            target.Prev.Next = this;
        }
        if (target.Next != null)
        {
            target.Next.Prev = this;
        }
        master = target.master;
        hand_id = target.hand_id;
        pos_y = target.pos_y;
        Pos_x = target.Pos_x;
        Fix_lig();
        target.Fix_lig();
    }

    public bool Bump_pos()// vypocita svoji pozici v zavislosti na delace predchozi noty
    { 
        bool output = false;
        double mod = Math.Pow(2, prev.Delka) * (2 - Math.Pow(2, -prev.postfix));
        Pos_y = prev.pos_y;
        pos_x = prev.pos_x + (float)mod / CTRL.takt;
        if (Pos_x >= takts_per_line + 1)
        {
            pos_x = 0;
            pos_y++;
            output = true;
        }
        CTRL.Do_Takty(master);
        Calc_Pos();
        return output;
    }

    public virtual int[] Copy()// vrati data jako pole integru pouzite v change menu
    {
        return new int[2] {0, Delka};
    }

    public virtual void Paste(int[] input) // zpracuje data z change menu udeatuje grafiku  vystup z change menu
    {
        if (input[0] == 0)
        {
            Delka = input[1];

            Update_delka();
            Calc_Pos();
            Update_gfx();
            CTRL.Recalc(next);
        }
        Fix_lig();
    }

    protected void Fix_lig()//zkontroluje platnost ligatury
    {
        if (lig_prev != null)
        {
            Do_lig(true);
        }
        else
        {
            Do_lig();
        }
    }
    public virtual void Update_gfx() { }// matody ktery jsou overridly potomky
    public virtual void Do_data() { }

    void Start() { }// tyhle dve funkce jsou podnimkou MonoBehavoiur class (trid ktere jsou pripojene k objektu)
    void Update() { }
}

public class Nota : Znak
{
    
    GameObject prefix_GO;// game objekt reprezentujici prefix(krizek, becko nebo odrazka)
    GameObject topfix_GO;// melo by to byt pouzite na statcato ale neni implemetovano

    //pomocne carky pokud je nota mimo linkovani
    GameObject carka_licha; // carka jdouci pod notou    pouzita kdyz vyska je licha
    GameObject carka_suda; // carka jdouci notou    pouzita kdyz vyska je suda
    GameObject[] carky;// pole vesch pomocnych carek ktere leze pouzit (neni limitovano lze jednoduse opravit)

    GameObject[] prapor_GOs; // odakazi napole prapru jsou pouse dva(pro osminy a sestnactiny)
    
    int vyska = 0; // relativne ku 2. radce od spoda

    int prefix = 0;// 0= nic 1 = krizky 2 = becka 3 = cista 
    int topfix = 0;// nepouzito

    public override GameObject Lig_next
    {
        get => lig_next;
        set
        {
            lig_next = value;
        }
    }

    public override GameObject Lig_prev
    {
        get => lig_prev;
        set
        {
            lig_prev = value;
        }
    }

    public override void Calc_Pos()// vypocita pozici noty a prevratiji pokud je nad polovinou linkovani
    {
        int mod = 0;
        if (hand_id > 0)
        {
            mod = hand_id - 1 + pos_y;
        }
        gameObject.transform.position = ref_point + new Vector3(Pos_x * takt_width + (float)Math.Floor(pos_x) * cara_width, (pos_y + mod) * Hand_Ctrl.vyska_linek + vyska * nota_height, 0);
        //prevraceni
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);    // ovlinuje vypocty ostatnich otoceni tak to resetujeme
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
            prefix_GO.transform.position = new Vector3(Math.Abs(prefix_GO.transform.position.x), prefix_GO.transform.position.y, prefix_GO.transform.position.z);
            prefix_GO.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            for (int i = 0; i < prapor_GOs.GetLength(0); i++)
            {
                prapor_GOs[i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        Fix_lig();
    }

    public override void Nota_Up(int i = 1)// zvysi notu o i nepouzito
    {
        vyska = i + vyska;
        Calc_Pos();
    }

    public override void Nota_Down(int i = 1)
    {
        vyska = i - vyska;
        Calc_Pos();
    }

    public override string Give_String()// vreati string pro ukladani  
    {
        return "N,"+Delka+","+vyska+","+prefix+","+postfix+","+(lig_prev!=null)+";"; 
    }

    public override bool is_nota()// tohle je nota takze vrati true
    {
        return true;
    }

    public override bool FromString(string input)// ziska data ze stringu vrati true pokud byl format spatne
    {
        bool error = false;
        char[] filter = new char[1] { ',' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        if (data[0] == "N")
        {
            Int32 output = new Int32();
            if (int.TryParse(data[1], out output))
            {
                Delka = output;
                Update_delka();
            }
            else
            {
                error = true;
            }

            if (int.TryParse(data[2], out output))
            {
                vyska = output;
            }
            else
            {
                error = true;
            }

            if (int.TryParse(data[3], out output))
            {
                prefix = output;
            }
            else
            {
                error = true;
            }

            if (int.TryParse(data[4], out output))
            {
                postfix = output;
            }
            else
            {
                error = true;
            }
            bool is_lig;
            if (bool.TryParse(data[5], out is_lig))
            {
                if (is_lig)
                {
                    Do_lig(true);
                }
            }
            else
            {
                error = true;
            }
        }
        else
        {
            error = true;
            Debug.Log("attepted to destring non Nota string");
        }
        return error;
    }
    public override int[] Copy()// data posilana do change menu
    {
        return new int[6] {2, Delka, postfix, prefix, topfix, vyska};
    }

    public override void Paste(int[] input)// zpracovani dat z change menu
    {
        if (input[0] == 2)
        {
            Delka = input[1];
            postfix = input[2];
            prefix = input[3];
            topfix = input[4];
            vyska = input[5];
            Update_delka();
            Calc_Pos();
            Update_gfx();
            CTRL.Recalc(next);
        }
        Fix_lig();
    }

    public override void Update_gfx()// updatuje grafiku noty
    {
        //pomocne carky
        if (vyska < -3)
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
                carka_licha.transform.position = new Vector3(carka_suda.transform.position.x, carka_suda.transform.position.y + Math.Abs(dist), carka_suda.transform.position.z);
                carka_licha.SetActive(true);
                carka_suda.SetActive(false);
            }
            int targ_carek = (Math.Abs(vyska) - 2) / 2 - 1;// cilove mnozstvi pomocnych car  zmenseno o dva(vdalenonost referencni linky od te spodni) / 2 liky jsou kazda dva body vysky - 1 kvuli originalum
            for (int i = 0; i < carky.GetLength(0); i++)
            {
                Destroy(carky[i]);
            }
            carky = new GameObject[targ_carek];
            for (int i = 0; i < targ_carek; i++)
            {
                carky[i] = Instantiate(select, gameObject.transform, true);
                carky[i].transform.position = new Vector3(select.transform.position.x, select.transform.position.y + 2 * Znak.nota_height * (i + 1), select.transform.position.z);
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
            int targ_carek = (Math.Abs(vyska) - 6) / 2 - 1;// cilove mnozstvi pomocnych car  zmenseno o sest(vdalenonost referencni linky od te horni) / 2 liky jsou kazda dva body vysky - 1 kvuli originalum
            for (int i = 0; i < carky.GetLength(0); i++)
            {
                Destroy(carky[i]);
            }
            carky = new GameObject[targ_carek];
            for (int i = 0; i < targ_carek; i++)
            {
                carky[i] = Instantiate(select, gameObject.transform, true);
                carky[i].transform.position = new Vector3(select.transform.position.x, select.transform.position.y - 2 * Znak.nota_height * (i + 1), select.transform.position.z);
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
        //topfix nepouzite
        if (topfix != 0)
        {
            topfix_GO.SetActive(true);
        }
        else
        {
            topfix_GO.SetActive(false);
        }
        //postfix (tecky)
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

    public override void Do_lig(bool previous = false) // udela ligaturu s nasledujici notou( nebo predchozi pokud previous je true)
    {
        if (previous)
        {
            if (prev != null)
            {
                prev.Do_lig(); 
            }
        }
        else if (next != null)// naledujici znak musi existovat
        {
            if (Next.is_nota())// ligatury jsou pouze s jinou notou
            {
                if (((Nota)next).vyska == vyska)// ligatura spojuje noty stejen vysky
                {
                    if (lig_next != null)// pokud zde uz ligatura je tak li odstranime
                    {
                        Destroy(lig_next);
                        if (next.Lig_prev != null)
                        {
                            Destroy(next.Lig_prev);
                        }
                    }
                    else if (next.Pos_y == Pos_y)// pokud jsou obe noty na stejne radce
                    {
                        if (lig_next != null)
                        {
                            Destroy(lig_next);
                        }
                        if (Next.Lig_prev != null)
                        {
                            Destroy(Next.Lig_prev);
                        }
                        Lig_next = CTRL.get_lig(0, this);
                        Lig_next.transform.position = new Vector3((transform.position.x + Next.transform.position.x) / 2, transform.position.y + 5f);
                        Lig_next.transform.localScale = new Vector3((transform.position.x - Next.transform.position.x) / (takt_width + cara_width) * 0.6f, 0.3f);
                        Next.Lig_prev = lig_next;
                        if (vyska > 1)
                        {
                            lig_next.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                        else//pro nizke noty je ligatura obracena tj. pod notami
                        {
                            lig_next.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                        }
                    }
                    else// pro noty na krajich radku
                    {
                        if (lig_next != null)
                        {
                            Destroy(lig_next);
                        }
                        if (Next.Lig_prev != null)
                        {
                            Destroy(Next.Lig_prev);
                        }
                        Lig_next = CTRL.get_lig(1, this);
                        Lig_next.transform.position = new Vector3(transform.position.x +  36, transform.position.y);
                        Next.Lig_prev = CTRL.get_lig(2, next);
                        Next.Lig_prev.transform.position = new Vector3(Next.transform.position.x - 36, Next.transform.position.y);
                        if (vyska > 1)
                        {
                            lig_next.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                            Next.Lig_prev.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                        else//pro nizke noty je ligatura obracena tj. pod notami
                        {
                            lig_next.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                            Next.Lig_prev.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                        }
                    }
                }
                else
                {
                    if (lig_next != null) // pokud ligatura neni mozna tak se ujistime ze nexstuje
                    {
                        Destroy(lig_next); 
                    }
                }
            }
            else
            {
                if (lig_next != null)
                {
                    Destroy(lig_next);
                }
            }
        }
    }

    public override void Do_data() // priradi odkazy na obekty
    {
        prapor_GOs = new GameObject[2];
        postfix_GO = new GameObject[3];
        prefix_GO = gameObject.transform.GetChild(1).gameObject;
        postfix_GO[0] = gameObject.transform.GetChild(2).gameObject;
        postfix_GO[1] = gameObject.transform.GetChild(8).gameObject;
        postfix_GO[2] = gameObject.transform.GetChild(9).gameObject;
        topfix_GO = gameObject.transform.GetChild(3).gameObject;
        prapor_GOs[0] = gameObject.transform.GetChild(4).gameObject;
        prapor_GOs[1] = gameObject.transform.GetChild(5).gameObject;
        carka_licha = gameObject.transform.GetChild(6).gameObject;
        carka_suda = gameObject.transform.GetChild(7).gameObject;
        prapor_GOs[0].SetActive(true);
        prapor_GOs[1].SetActive(true);
        carka_licha.SetActive(true);
        carka_suda.SetActive(true);
        carky = new GameObject[1];
        carky[0] = Instantiate(carka_licha, gameObject.transform);
        carky[0].SetActive(false);
        Update_gfx();
    }
}

public class Pomlka : Znak
{
    
    public override string Give_String()
    {
        return "P," + Delka + "," + postfix + ";";
    }

    public override bool is_nota()
    {
        return false;
    }

    public override bool FromString(string input) // ziska data ze stringu
    {
        bool error = false;
        char[] filter = new char[1] { ',' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        if (data[0] == "P")
        {
            Int32 output = new Int32();
            if (int.TryParse(data[1], out output))
            {
                Delka = output;
                Update_delka();
            }
            else
            {
                error = true;
            }
            
            if (int.TryParse(data[2], out output))
            {
                postfix = output;
            }
            else
            {
                error = true;
            }
        }
        else
        {
            error = true;
            Debug.Log("attepted to destring non Pomlka string");
        }
        return error;
    }

    public override int[] Copy()// posle data na change menu
    {
        return new int[3] {1, Delka, postfix};
    }

    public override void Paste(int[] input) // vystup z change menu
    {
        if (input[0] == 1)
        {
            Delka = input[1];
            postfix = input[2];
            Update_delka();
            Calc_Pos();
            Update_gfx();
            CTRL.Recalc(next);
        }
    }

    public override void Update_gfx()
    {
        //delka pomka
        switch (Delka)
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

    public override void Do_data()
    {
        postfix_GO = new GameObject[3];
        postfix_GO[0] = gameObject.transform.GetChild(2).gameObject;
        postfix_GO[1] = gameObject.transform.GetChild(8).gameObject;
        postfix_GO[2] = gameObject.transform.GetChild(9).gameObject;
        gameObject.transform.GetChild(4).gameObject.SetActive(false);// objekt ma default ne tyto objekty zapnute
        gameObject.transform.GetChild(5).gameObject.SetActive(false);// ikdyz by se u pomlky nemely vyskytovat
        gameObject.transform.GetChild(6).gameObject.SetActive(false);
        gameObject.transform.GetChild(7).gameObject.SetActive(false);
        Update_gfx();
    }
}
/*                   Nepouzito
class Acord : Znak
{
    Nota start;
    
    int topfix = 0;// 0 = nic 1 

    public override void Nota_Up(int i = 1)
    {
    }

    public override void Nota_Down(int i = 1)
    {
    }

    public override string Give_String()
    {
        string output = "";
        Nota selected = start;
        while (selected != null)
        {
            output = output + selected.Give_String();
        }
        return "A," + Delka + "," + output + ";";
    }

    public override bool is_nota()
    {
        return true;//maybe
    }

    public override bool FromString(string input)
    {
        bool error = false;
        char[] filter = new char[1] { ',' };
        string[] data = input.Split(filter, StringSplitOptions.RemoveEmptyEntries);
        if (data[0] == "A")
        {
            Int32 output = new Int32();
            if (int.TryParse(data[1], out output))
            {
                Delka = output;
                Update_delka();
            }
            else
            {
                error = true;
            }
        }
        else
        {
            error = true;
            Debug.Log("attepted to destring non Acord string");
        }
        return error;
    }
    public override int[] Copy()
    {
        return new int[3] { 3, Delka, topfix};
    }
    public override void Paste(int[] input)
    {
        if (input[0] == 3)
        {
            Delka = input[1];
            topfix = input[2];
            Update_delka();
        }
    }
}
*/
