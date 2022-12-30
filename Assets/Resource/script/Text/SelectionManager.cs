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
        // key�� list��ŭ �Ұ�. list�� ���ñ� �Žñ��ϰ�.


        foreach ( JToken list in jcur["list"])
        {
            //RectTransform rect = button[len].GetComponent<RectTransform>();
            //rect.anchoredPosition = new Vector2(pos[len % 2], (len/2) * -200);
            // list�� ���� ���� ��ġ�����̱� �ѵ�... ���߿�

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
            // dice�� jdes�� ��ü�� �޾ƾ�.. �� -> �׷��� suc�̶�, fail�� ó����...(��ġ�� �ٲܱ�... ��¿��..)
            if (code[0].ToString() == "dice")
                textchanger.GetOpcode(code[0].ToString(), jkey, 1);
            else
                textchanger.GetOpcode(code[0].ToString(), code, 1);
            //textchanger.CheckKeys("::opcode", code);
        }
        

        //���� ���� ���(�� �ٸ�)
        TextManager.GetComponent<TextManager>().ReadStory(true);

        for (; len > 0; len--)
            button[len - 1].SetActive(false);

        //destination = new Vector3(0.0f, -2000.0f, -4.0f);
        //StartCoroutine(Moving(gameObject));
    }

    // sc_obj Ŭ��, �������� ���ŵȴ�.
    public void OnClickObj(GameObject keyword)
    {
        string word = keyword.GetComponent<Keyword>().keyword;
        int idx = keyword.GetComponent<Keyword>().idx;
        ShowSelection("sc_key", idx);
        Debug.Log("SC_OBJ : " + word);

        //�������� ������, ���� main.txt�� ������ �߰��ȴ�. �׷���. ���� �Ʒ��� �߰��� �ȴ�... ��
        // key, sc_key�� ����ʰ� ���� �ٿ��ٰ� �߰���Ű�� �ʹ�.
        // 1.m_text���� ���Ž�Ų��.
        // 2.���� �� Ÿ�Ϸ���, main.txt�� �߰��� ������ ��Ų��. 
        //  2-1. �� ���, current�� �Ƹ� ����ҰŴ�. ( before = < current, after = >= current , append, main += after)

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

    private string MakeJson(string jpath)    //parsing�� �ϰ� �׳� �ִ°Ŵ� �ȵǳ�? ����..?
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
