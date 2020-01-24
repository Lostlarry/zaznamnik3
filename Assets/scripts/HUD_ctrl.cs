using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_ctrl : MonoBehaviour
{
    public Hand_Ctrl CTRL;
    public GameObject[] toggle_able;
    Vector3 default_pos;

    public bool select; 

    public virtual void Adjust_HUD(GameObject target, bool toggle = true)
    {
        if (select)
        {
            this.gameObject.transform.localPosition = default_pos + target.transform.localPosition;
            for (int i = 0; i < toggle_able.GetLength(0); i++)
            {
                toggle_able[i].SetActive(toggle);
            } 
        }
        else
        {
            Vector3 tmp = target.transform.localPosition;
            if(target.GetComponent<Znak>().Bump_pos())
            {
                CTRL.Relay_signal(10);
            }
            this.gameObject.transform.localPosition = default_pos + target.transform.localPosition;
            target.transform.localPosition = tmp;
        }
    }

    public void Send_Command(int cmd_id)
    {
        if (CTRL != null)
        {
            CTRL.Relay_signal(cmd_id);
        }
    }

    public Vector3 Get_Pos()
    {
        return transform.localPosition - default_pos;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        default_pos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
