using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Save_Load : MonoBehaviour
{
    public Screen_Ctrl SCR_ctrl;

    void Start() { }
    void Update() { }

    string input = "";

    public bool Save()
    {
        string savefile = "SaveFile.nml";
        //string savefile = strEx("Save File", "nml", "/saves");
        if (savefile != "")
        {
            string output = gameObject.GetComponent<Hand_Ctrl>().Give_String();
            TextWriter writer = new StreamWriter(savefile, false);
            writer.WriteLine(output);
            writer.Close();
            SCR_ctrl.log_save(3);// file saved
            return true;
        }
        return false;
        
    }
    public bool Load()
    {
        string savefile = "SaveFile.nml";
        //string savefile = strEx("Load File", "nml", "/saves");
        if (System.IO.File.Exists(savefile))
        {
            TextReader reader = new StreamReader(savefile);
            input = reader.ReadLine();
            reader.Close();
            SCR_ctrl.log_save(2);// loading file
            return true;
        }
        else
        {
            SCR_ctrl.log_save(1);// no file detected
        }
        return false;
    }
    /*
    string strEx(string name, string ext, string add = "")
    {
        return EditorUtility.OpenFilePanel(name, add, ext);
    }
    */
    public string getString()
    {
        return input;
    }
}
