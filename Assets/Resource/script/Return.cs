using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return : MonoBehaviour
{
    
    
    void Start()
    {
        GameObject ch2 = GameObject.Find("Button2");
        ch2.SetActive(false);
    }

    public void ButtonChange()
    {
        GameObject ch1 = GameObject.Find("Button1");
        GameObject ch2 = GameObject.Find("Button2");
        //ButtonManager m2 = new ButtonManager();
        ch2.SetActive(false);
        ch1.SetActive(true);

    }

    void Update()
    {
        
    }
}
