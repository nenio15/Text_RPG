using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public GameObject TextManager;
    public GameObject[] button;

    private Vector3 destination = new Vector3(0.0f, -800.0f, -4.0f);
    private Vector2 speed = Vector2.zero;
    private float time = 0.2f;
    private string mainroute, keyroute, str;

    private JObject jroot;
    private JToken jcur;
    //private int[] pos = { -200, 200 };
    int len = 0;
    

    private void Start()
    {
        mainroute = Application.dataPath + @"\Resource\Text\main.txt";
        keyroute = Application.dataPath + @"\Resource\Text\main.json";


    }

    public void ShowSelection(int idx)
    {   
        str = MakeJson(keyroute);
        jroot = JObject.Parse(str);
        jcur = jroot["key"][idx];
        // key의 list만큼 할것. list는 저시기 거시기하고.


        foreach ( JToken list in jcur["list"])
        {
            //RectTransform rect = button[len].GetComponent<RectTransform>();
            //rect.anchoredPosition = new Vector2(pos[len % 2], (len/2) * -200);
            // list의 수에 따라 위치변경이긴 한데... 나중에

            button[len].GetComponentInChildren<Text>().text = list.ToString();
            button[len++].SetActive(true);
        }

        destination = new Vector3(0.0f, -800.0f, -4.0f);
        StartCoroutine(Moving(gameObject));
    }
    
    public void OnClick(Text t)
    {
        Textchanger textchanger = TextManager.GetComponent<TextManager>().textchanger;
        Debug.Log("CLICK_TEXT : " + t.text);
        JToken jkey = jcur[t.text];

        foreach (JToken code in jkey["effect"])
        {
            //Debug.Log("CLICK_code : " + code.ToString());
            // dice는 jdes그 자체를 받아야.. 함 -> 그래야 suc이랑, fail을 처리함...(위치를 바꿀까... 어쩔까..)
            if (code[0].ToString() == "dice")
                textchanger.GetOpcode(code[0].ToString(), jkey, 1);
            else
                textchanger.GetOpcode(code[0].ToString(), code, 1);
            
        }
        
        //다음 문장 출력(한 줄만)
        TextManager.GetComponent<TextManager>().ReadStory(true);

        for (; len > 0; len--)
            button[len - 1].SetActive(false);

        destination = new Vector3(0.0f, -2000.0f, -4.0f);
        StartCoroutine(Moving(gameObject));
    }

    IEnumerator Moving(GameObject obj)
    {
        //Debug.Log("moving");
        while (obj.transform.position != destination)
            yield return obj.transform.position = Vector2.SmoothDamp(obj.transform.position, destination, ref speed, time);
    }

    private string MakeJson(string jpath)    //parsing안 하고 그냥 넣는거는 안되나? 굳이..?
    {
        string str = null;
        using (StreamReader sr = File.OpenText(jpath))
        using (JsonTextReader reader = new JsonTextReader(sr))
        {
            str = sr.ReadToEnd();
            sr.Close();
        }
        return str;
    }

}
