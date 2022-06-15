using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Text m_TypingText;
    public string m_Message;
    public float m_Speed = 0.2f;

    private string route = Application.dataPath + @"\Resource\Text\main.txt";
    private string[] co_txt;

    void Start()
    {
        co_txt = System.IO.File.ReadAllLines(route);
    }

    // Update is called once per frame
    void Update()
    {
        //System.IO.File.ReadLines(route);
    }
}
