using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    private Textchanger textchanger;
    int len = 0;
    

    private void Start()
    {
        mainroute = Application.dataPath + @"\Resource\Text\main.txt";
        keyroute = Application.dataPath + @"\Resource\Text\main.json";
        textchanger = TextManager.GetComponent<TextManager>().textchanger;
    }

    public void ShowSelection(string option, int idx)
    {   
        str = MakeJson(keyroute);
        jroot = JObject.Parse(str);
        jcur = jroot[option][idx];
        Debug.Log("SELECT : " + option);
        // key의 list만큼 할것. list는 저시기 거시기하고.


        foreach ( JToken list in jcur["list"])
        {
            //RectTransform rect = button[len].GetComponent<RectTransform>();
            //rect.anchoredPosition = new Vector2(pos[len % 2], (len/2) * -200);
            // list의 수에 따라 위치변경이긴 한데... 나중에

            button[len].GetComponentInChildren<Text>().text = list.ToString();
            button[len++].SetActive(true);
        }

        //destination = new Vector3(0.0f, -800.0f, -4.0f);
        //StartCoroutine(Moving(gameObject));
    }
    
    public void OnClick(Text t)
    {
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
            //textchanger.CheckKeys("::opcode", code);
        }
        

        //다음 문장 출력(한 줄만)
        TextManager.GetComponent<TextManager>().ReadStory(true);

        for (; len > 0; len--)
            button[len - 1].SetActive(false);

        //destination = new Vector3(0.0f, -2000.0f, -4.0f);
        //StartCoroutine(Moving(gameObject));
    }

    // sc_obj 클릭, 선택지가 갱신된다.
    public void OnClickObj(GameObject keyword)
    {
        string word = keyword.GetComponent<Keyword>().keyword;
        int idx = keyword.GetComponent<Keyword>().idx;
        ShowSelection("sc_key", idx);
        Debug.Log("SC_OBJ : " + word);

        //선택지를 누르면, 기존 main.txt에 내용이 추가된다. 그렇다. 가장 아래에 추가가 된다... 흠
        // key, sc_key를 상관않고 현재 줄에다가 추가시키고 싶다.
        // 1.m_text에만 갱신시킨다.
        // 2.어찌 잘 타일러서, main.txt의 중간에 삽입을 시킨다. 
        //  2-1. 이 경우, current를 아마 써야할거다. ( before = < current, after = >= current , append, main += after)

        /*
        JToken jkey = jcur[word];
        
        foreach (JToken code in jkey["effect"])
        {

            if (code[0].ToString() == "dice")
                textchanger.GetOpcode(code[0].ToString(), jkey, 1);
            else
                textchanger.GetOpcode(code[0].ToString(), code, 1);
        }

        TextManager.GetComponent<TextManager>().ReadStory(true);
        */
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
