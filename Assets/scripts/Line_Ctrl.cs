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
    public bool hud = true;

    public bool full = false;
    public float nota_lenght = 396f / 15;

    public Line_Ctrl Next { get => next; set => next = value; }
    public Line_Ctrl Prev { get => prev; set => prev = value; }
    public Takt Last { get => last; set => last = value; }

    void Start() { }
    void Update() { }

    public void SetActive(bool state, bool cascade = false)
    {
        if (hud)
        {
            state = true;
        }
        klic.SetActive(state);
        end.SetActive(state);
        pred.SetActive(state);
        gameObject.SetActive(state);
        Takt selected = first;
        if (selected != null)
        {
            while (selected != last)
            {
                selected.SetActive(false);
                selected = selected.next;
            }
            selected.SetActive(false); 
        }
        if (cascade)
        {
            if (next != null)
            {
                next.SetActive(state);
            }
        }
    }

    public void count(Takt selected, bool repeat = true)
    {
        if (selected == null)
        {
            if (End_hud.posy != id)
            {
                SetActive(false, true);
            }
            return;
        }
        if (!selected.status)
        {
            SetActive(false);
        }
        float sum = 0;
        first = selected;
        full = false;
        while (selected != null && !full)
        {
            if (sum + selected.count() <= 15)
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
                Znak.CTRL.Add_line(master).count(selected);
            }
        }
        else
        {
            selected = first;
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
            if (last.posledni == null && repeat)
            {
                first = last;
                count(first, false);
                return;
            }
            if (last.posledni.Delka == 4)
            {
                sum = sum - 1.5f;
            }
            else
            {
                sum--;
            }
            nota_lenght = 396f / sum;
            Znak.CTRL.Recalc(first.prvni, last.posledni);
            selected = first;
            while (selected != last)
            {
                selected.Repos();
                selected = selected.next;
            }
            selected.Repos();
            last.SetActive(false);
        }
        else
        {
            nota_lenght = 396f / 15;
            Znak.CTRL.Recalc(first.prvni, last.posledni);
            selected = first;
            while (selected != last)
            {
                selected.Repos();
                selected = selected.next;
            }
            selected.Repos();
            last.SetActive(true);
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

    public Znak prvni;
    public Znak posledni;

    public Takt next;
    public Takt prev;

    private Line_Ctrl lC;
    bool full = false;

    public bool status = true;

    public double delka;

    public Line_Ctrl LC
    {
        get => lC;
        set
        {
            lC = value;
            Znak vybrany = prvni;
            if (vybrany != null)
            {
                while (vybrany != posledni)
                {
                    vybrany.Linka = value;
                    vybrany = vybrany.Next;
                }
                vybrany.Linka = lC; 
            }
        }
    }
    

    public void SetActive(bool state, bool cascade = false)
    {
        status = state;
        cara.SetActive(state);
        if (cascade && next != null)
        {
            next.SetActive(false, true);
        }
    }

    public Takt(GameObject cara, Znak prvni, Takt prev, Line_Ctrl lC)
    {
        this.cara = cara;
        this.prvni = prvni;
        if (posledni == null)
        {
            posledni = prvni;
        }
        if (prev != null)
        {
            this.prev = prev;
            prev.next = this;
        }
        LC = lC;
        LC.Last = this;
        cara.transform.position = new Vector3(posledni.transform.position.x + 8f, LC.transform.position.y);
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
                next.recalc(vybrany.Next); 
            }
            else
            {
                next = Znak.CTRL.Add_takt(LC.master, vybrany);
                next.recalc(vybrany.Next);
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
        cara.transform.position = new Vector3(posledni.transform.position.x + 8f, LC.transform.position.y);
    }

    public void Repos()
    {
        if (posledni != null)
        {
            cara.transform.position = new Vector3(posledni.transform.position.x + 8f, LC.transform.position.y);
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
        if (vybrany != null)
        {
            if (vybrany.Delka == 4)
            {
                sum = sum + 1.5f;
            }
            else
            {
                sum++;
            } 
        }
        return sum;
    }

    public double Delka()// call only after recalc
    {
        Znak vybrany = prvni;
        double sum = 0;
        if (vybrany != null)
        {
            while (vybrany != posledni)
            {
                sum = sum + Math.Pow(2, vybrany.Delka) * (2 - Math.Pow(2, -vybrany.Postfix));
                vybrany = vybrany.Next;
            }
            sum = sum + Math.Pow(2, vybrany.Delka) * (2 - Math.Pow(2, -vybrany.Postfix)); 
        }
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
            posledni = target;
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

