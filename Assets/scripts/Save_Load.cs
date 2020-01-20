using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Save_Load : MonoBehaviour
{
    void Start() { }
    void Update() { }

    string input = "";

    public void Save()
    {
        string savefile = strEx("Save File", "nml", "/saves");
        if (savefile != null)
        {
            string output = gameObject.GetComponent<Hand_Ctrl>().ToString();
            TextWriter writer = new StreamWriter(savefile, false);
            writer.WriteLine(output);
            writer.Close();
        }
    }
    public void Load()
    {
        string savefile = strEx("Load File", "nml", "/saves");
        if (savefile != null)
        {
            TextReader reader = new StreamReader(savefile);
            input = reader.ReadLine();
            reader.Close();
        }
    }

    string strEx(string name, string ext, string add = "")
    {
        return EditorUtility.OpenFilePanel(name, add, ext);
    }

    public string getString()
    {
        return input;
    }
}
