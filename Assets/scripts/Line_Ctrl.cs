using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Ctrl : MonoBehaviour
{

    public static HUD_ctrl End_hud;
    Line_Ctrl next;
    Line_Ctrl prev;

    public GameObject klic;
    public GameObject end;
    public GameObject pred;

    public Holder master;

    Takt first;
    Takt last;

    public int id = 0;

    public bool full = false;
    public float nota_lenght = 475.2f / 23;

    public Line_Ctrl Next { get => next; set => next = value; }
    public Line_Ctrl Prev { get => prev; set => prev = value; }
    public Takt Last { get => last; set => last = value; }

    void Start()
    {
    }
    void Update() { }

    public void SetActive(bool state, bool cascade = false)
    {
        klic.SetActive(state);
        end.SetActive(state);
        pred.SetActive(state);
        gameObject.SetActive(state);
        if (cascade)
        {
            if (next != null)
            {
                next.SetActive(state);
            }
        }
    }

    public void count(Takt selected)
    {

         
        if (selected == null)
        {
            if (End_hud.posy != id)
            {
                SetActive(false, true);
            }
        }
        if (!selected.status)
        {
            SetActive(false);
        }
        float sum = 0;
        first = selected;
        while (selected != null && !full)
        {
            if (sum + selected.count() <= 22)
            {
                selected.LC = this;
                sum = sum + selected.count();
                selected = selected.next;
            }
            else
            {
                full = true;
                selected.prev = last;
            }
        }
        if (selected != null)
        {
            if (next != null)
            {
                next.count(selected);
            }
            else
            {
                Znak.CTRL.Add_line(master);
                next.count(selected);
            }
        }
        else
        {
            while (selected.next != null)
            {
                selected = selected.next;
            }
            last = selected;
            if (next != null)
            {
                next.SetActive(false, true);
            }
        }

        if (full)
        {
            nota_lenght = 475.2f / sum;
        }
        else
        {
            nota_lenght = 475.2f / 22;
        }
    }

    public void prep_Destroy()
    {
        Destroy(klic);
        Destroy(end);
        Destroy(pred);
        Takt vybrany = first;
        while (vybrany != last)
        {
            vybrany = vybrany.next;
            Destroy(vybrany.prev.cara);
        }
        Destroy(vybrany.cara);
        Destroy(gameObject);
        Destroy(this);
    }
}

public class Takt
{
    public GameObject cara;

    Znak prvni;
    Znak posledni;

    public Takt next;
    public Takt prev;

    public Line_Ctrl LC;
    bool full = false;

    public bool status = true;

    public double delka;

    public void SetActive(bool state, bool cascade = false, int call = 0)
    {

        call++;
        Debug.Log(call);
        status = state;
        cara.SetActive(state);
        if (cascade && next != null)
        {
            next.SetActive(false, true, call);
        }
    }

    public Takt(GameObject cara, Znak prvni, Takt prev, Line_Ctrl lC)
    {
        this.cara = cara;
        this.prvni = prvni;
        if (prev != null)
        {
            this.prev = prev;
            prev.next = this;
        }
        LC = lC;
        LC.Last = this;
    }

    public void recalc(Znak vybrany)
    {

         
        prvni = vybrany;
        if (vybrany == null)
        {
            SetActive(false, true);
            prvni = null;
            posledni = null;
            return;
        }
        SetActive(true);
        delka = 0;
        while (vybrany != null && !full)
        {
            if (delka + Math.Pow(2, vybrany.Delka) * (2 - Math.Pow(2, -vybrany.Postfix)) <= Znak.CTRL.takt)
            {
                delka = delka + Math.Pow(2, vybrany.Delka) * (2 - Math.Pow(2, -vybrany.Postfix));
                vybrany.Takt = this;
                vybrany = vybrany.Next;
            }
            else
            {
                posledni = vybrany.Prev;
                full = true;
            }
        }
        if (vybrany != null)
        {
            if (next != null)
            {
                next.recalc(vybrany); 
            }
            else
            {
                next = Znak.CTRL.Add_takt(LC.master, vybrany);
                next.recalc(vybrany);
            }
        }
        else
        {
            vybrany = prvni;
            while (vybrany.Next != null)
            {
                vybrany = vybrany.Next;
            }
            posledni = vybrany;
            if (next != null)
            {
                next.SetActive(false, true);
            }
        }
    }

    public float count()
    { 
        Znak vybrany = prvni;
        float sum = 0;
        Delka();
        while (vybrany != posledni)
        {
            if (vybrany.Delka == 4)
            {
                sum = sum + 1.5f;
            }
            else
            {
                sum++;
            }
            vybrany = vybrany.Next;
        }
        if (vybrany.Delka == 4)
        {
            sum = sum + 1.5f;
        }
        else
        {
            sum++;
        }
        return sum;
    }

    public double Delka()// call only after recalc
    {
        Znak.CTRL.Do_Takty(LC.master);
        Znak vybrany = prvni;
        double sum = 0;
        while (vybrany != posledni)
        {
            sum = sum + Math.Pow(2, vybrany.Delka) * (2 - Math.Pow(2, -vybrany.Postfix));
            vybrany = vybrany.Next;
        }
        sum = sum + Math.Pow(2, vybrany.Delka) * (2 - Math.Pow(2, -vybrany.Postfix));
        return sum;
    }

    public void add(Znak target)
    {
        if (delka + Math.Pow(2, target.Delka) * (2 - Math.Pow(2, -target.Postfix)) <= Znak.CTRL.takt)
        {
            delka = delka + Math.Pow(2, target.Delka) * (2 - Math.Pow(2, -target.Postfix));
            target.Takt = this;
            target = posledni;
            if (prvni == null)
            {
                prvni = target;
                SetActive(true);
            }
        }
        else
        {
            full = true;
            if (next != null)
            {
                next.add(target);
            }
            else
            {
                next = Znak.CTRL.Add_takt(LC.master, target);
            }
        }
    }
}

