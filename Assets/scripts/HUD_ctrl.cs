using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_ctrl : MonoBehaviour
{
    public Hand_Ctrl CTRL;
    public GameObject[] toggle_able;
    Vector3 default_pos;
    public bool auth = false;
    public GameObject[] auth_able;

    public bool select; 

    public virtual void Adjust_HUD(GameObject target, bool toggle = true)
    {
        if (select)
        {
            gameObject.transform.position = target.transform.position;
            for (int i = 0; i < toggle_able.GetLength(0); i++)
            {
                toggle_able[i].SetActive(toggle);
            }
            for (int i = 0; i < auth_able.GetLength(0); i++)
            {
                auth_able[i].SetActive(auth);
            }
        }
        else
        {
            Vector3 tmp = target.transform.position;
            if(target.GetComponent<Znak>().Bump_pos())
            {
                Send_Command(10);
            }
            gameObject.transform.position = target.transform.position;
            target.transform.position = tmp;
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

    public Vector3 Get_Pos()
    {
        return gameObject.transform.position;
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
}
