using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class InfoMenu : MonoBehaviour
{
    private static InfoMenu instance;
    public static InfoMenu Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<InfoMenu>();
            }
            return instance;
        }
    }

    public Text info;

    public void WriteLine(string line)
    {
        string text = Datastamp() + line + "\n" + info.text;
        info.text = text;
    }

    private string Datastamp()
    {
        DateTime now = DateTime.Now;

        string timeStamp = "[" + now.Hour + ":" + now.Minute + "] ";

        return timeStamp;
    }
}
