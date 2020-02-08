using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_ctrl : MonoBehaviour
{
    const float modx = -160f;
    const float mody = -56f;

    public Hand_Ctrl CTRL;
    public GameObject[] toggle_able;
    public bool auth = false;
    public GameObject[] auth_able;

    public int posy;
    public bool select; 

    public virtual void Adjust_HUD(Znak target)
    {
        Vector3 master_scale = gameObject.transform.parent.position;
        Vector3 ref_point = new Vector3(master_scale.x + modx, master_scale.y * 2 + mody);
        int mod = 0;
        if (target.Hand_id > 0)
        {
            mod = target.Hand_id - 1 + target.Pos_y;
        }
        if (select)
        {
            gameObject.transform.position = ref_point + new Vector3(target.Pos_x * CTRL.get_linka(target.master,posy).nota_lenght, (mod + target.Pos_y) * Hand_Ctrl.vyska_linek, 0);
            for (int i = 0; i < toggle_able.GetLength(0); i++)
            {
                toggle_able[i].SetActive(target.is_nota());
            }
            for (int i = 0; i < auth_able.GetLength(0); i++)
            {
                auth_able[i].SetActive(auth);
            }
        }
        else
        {
            posy = target.Pos_y;
            float posx = target.Pos_x;
            if (target.Delka == 4)
            {
                posx = posx + 1.5f;
            }
            else
            {
                posx++;
            }
            if (target.linka.full)
            {
                Send_Command(10);
                posy++;
                posx = 0;
            }
            gameObject.transform.position = ref_point + new Vector3(posx * target.linka.nota_lenght, (mod + posy) * Hand_Ctrl.vyska_linek, 0);
            CTRL.Do_Takty(target.master);
        }
    }

    public void set_auth(bool input)
    {
        auth = input;
        for (int i = 0; i < auth_able.GetLength(0); i++)
        {
            auth_able[i].SetActive(auth);
        }
    }


    public void Send_Command(int cmd_id)
    {
        if (CTRL != null)
        {
            CTRL.Relay_signal(cmd_id);
        }
    }

    public void Adjust_alpha(GameObject source)
    {
        Image[] list = GetComponentsInChildren<Image>();
        for (int i = 0; i < list.GetLength(0); i++)
        {
            Color paint = list[i].color;
            paint = new Color(paint.r, paint.g,paint.b, source.GetComponent<Slider>().value);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (!select)
        {
            Line_Ctrl.End_hud = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
