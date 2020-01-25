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
            gameObject.transform.position = target.transform.position;
            for (int i = 0; i < toggle_able.GetLength(0); i++)
            {
                toggle_able[i].SetActive(toggle);
            } 
        }
        else
        {
            Znak tmp = target.GetComponent<Znak>();
            int tmp_x = tmp.Pos_x;
            int tmp_y = tmp.Pos_y;
            tmp_x++;
            if (tmp_x > Znak.notes_per_line)
            {
                tmp_x = tmp_x - Znak.notes_per_line;
                tmp_y++;
                Send_Command(10);
            }
            gameObject.transform.position = default_pos + new Vector3(tmp_x * Znak.nota_width, tmp_y * Hand_Ctrl.vyska_linek, 0);
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
