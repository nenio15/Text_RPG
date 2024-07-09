using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private TextManager textmanager;
    [SerializeField] private Textchanger textchanger;
    public GameObject[] button;
    [SerializeField] private int len = 0;

    private Vector3 destination = new Vector3(0.0f, -800.0f, -4.0f);
    private Vector2 speed = Vector2.zero;
    private float time = 0.2f;
    private string mainroute, jroute, str;

    private JObject jroot;
    private JToken jcur;
    private ConvertJson convertJson = new ConvertJson();


    enum State
    {
        Scenario,
        Battle,
        Strategy
    }

    State state;

    private void Start()
    {
        mainroute = Application.dataPath + @"\Resource\Text\main.txt";
    }

    //�ó������� �������� �����Ѵ� + 2024-07-09 �ó��������� �ϴ°� �ƴϴ�.
    public void ShowSelection(string option, int idx, int c_state)
    {
        //�������ѵ�... ��ư state�� ���� ��� ��

        switch (c_state)
        {
            case (int)State.Scenario:
                state = State.Scenario;
                jroute = Application.dataPath + @"\Resource\Text\main.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option][idx];
                Debug.Log("SELECT : " + option);


                //����Ʈ�� �ִ� ��ŭ�� ��ư�� Ȱ��ȭ, ����
                foreach (JToken list in jcur["list"])
                {
                    // list�� ���� ���� ��ġ ���� ���� ( �׳� �������� �ϳ� ����°� ���ҵ�)
                    //RectTransform rect = button[len].GetComponent<RectTransform>();
                    //rect.anchoredPosition = new Vector2(pos[len % 2], (len/2) * -200);

                    //��ư �ؽ�Ʈ�� Ȱ��ȭ
                    button[len].GetComponentInChildren<Text>().text = list.ToString();
                    button[len++].SetActive(true);
                }

                //destination = new Vector3(0.0f, -800.0f, -4.0f);
                //StartCoroutine(Moving(gameObject));
                break;
            case (int)State.Battle:
                state = State.Battle;
                Debug.Log("battle is going on");
                jroute = Application.dataPath + @"\Resource\Text\Info\Skill.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option];

                foreach (JToken list in jcur["list"])
                {
                    //��ư �ؽ�Ʈ�� Ȱ��ȭ
                    button[len].GetComponentInChildren<Text>().text = list.ToString();
                    button[len++].SetActive(true);
                }

                break;
            case (int)State.Strategy:
                    state = State.Strategy;
                    Debug.Log("rollllllling");
                break;

        }


    }
    
    //�ó����� �������� ��Ʋ �������� �ٸ��� ������..
    public void OnClick(Text t)
    {
        switch (state)
        {
            case State.Scenario:
                Debug.Log("CLICK_TEXT[Scenario] : " + t.text);
                JToken jkey = jcur[t.text];

                // ������ ȿ�� ó��
                foreach (JToken code in jkey["effect"])
                {
                    Debug.Log("CLICK_code : " + code.ToString());
                    string decode = code[0].ToString();

                    //dice�� ���� ���� ���ΰ� ���ԵǾ� �־� ���� ó���� �Ѵ�.(���߿� json����... 2024-01-29)
                    if (decode == "dice") textchanger.GetOpcode(code[0].ToString(), jkey, 1);
                    else textchanger.GetOpcode(code[0].ToString(), code, 1);

                }

                //�̺�Ʈ ���� �Ұž�?
                //EventInformar.CheckAll();

                //���� ���� ���
                textmanager.ReadStory(true);
                break;
            case State.Battle:
                Debug.Log("CLICK_TEXT[Battle] : " + t.text);
                break;
            default:




                for (; len > 0; len--) button[len - 1].SetActive(false);
                break;
        }
        //destination = new Vector3(0.0f, -2000.0f, -4.0f);
        //StartCoroutine(Moving(gameObject));


    }

    // sc_obj Ŭ��, �������� ���ŵȴ�.
    public void OnClickObj(GameObject keyword)
    {
        string word = keyword.GetComponent<Keyword>().keyword;
        int idx = keyword.GetComponent<Keyword>().idx;
        ShowSelection("sc_key", idx, 0);
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


}
