using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_ctrl : MonoBehaviour
{
    public Hand_Ctrl line;
    public GameObject[] toggle_able;

    public void Adjust_HUD(GameObject target, bool toggle = true)
    {
        this.gameObject.transform.localPosition = target.transform.localPosition;
        for (int i = 0; i < toggle_able.GetLength(0); i++)
        {
            toggle_able[i].SetActive(toggle);
        }
    }

    public void Send_Command(int cmd_id)
    {
        if (line != null)
        {
            line.Relay_signal(cmd_id);
        }
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
