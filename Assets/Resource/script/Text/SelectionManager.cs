using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Timeline;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    [SerializeField] private TextChanger textChanger;
    [SerializeField] private GameObject player;
    [SerializeField] private Judgement judgement;
    [SerializeField] private BattleManager battleManager;
    public GameObject[] button;
    [SerializeField] private int len = 0;
    

    private Vector3 destination = new Vector3(0.0f, -800.0f, -4.0f);
    private Vector2 speed = Vector2.zero;
    private float time = 0.2f;
    private string mainroute, jroute, str;

    private JObject jroot;
    private JToken jcur;
    private ConvertJson convertJson = new ConvertJson();

    public string atb;

    enum State
    {
        Scenario,
        Battle,
        Strategy
    }

    State state;

    private void Start()
    {
        textManager = FindObjectOfType<TextManager>();
        textChanger = GameObject.FindObjectOfType<TextChanger>();
        //player = GameObject.Find("Player");
        judgement = FindObjectOfType<Judgement>();
        battleManager = GameObject.FindObjectOfType<BattleManager>();

        mainroute = Application.dataPath + @"\Resource\Text\main.txt";
    }

    //셀렉션 나열
    public void ShowSelection(string option, int idx, int c_state)
    {

        switch (c_state)
        {
            case (int)State.Scenario:
                state = State.Scenario;
                jroute = Application.dataPath + @"\Resource\Text\main.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option][idx];
                Debug.Log("SELECT : " + option);


                //리스트에 있는 만큼의 버튼을 활성화, 나열
                foreach (JToken list in jcur["list"])
                {
                    // list의 수에 따른 위치 세팅 변경 ( 그냥 프리셋을 하나 만드는게 편할듯)
                    //RectTransform rect = button[len].GetComponent<RectTransform>();
                    //rect.anchoredPosition = new Vector2(pos[len % 2], (len/2) * -200);

                    //버튼 텍스트와 활성화
                    button[len].GetComponentInChildren<Text>().text = list.ToString();
                    atb = list.ToString();
                    button[len++].SetActive(true);
                }

                //destination = new Vector3(0.0f, -800.0f, -4.0f);
                //StartCoroutine(Moving(gameObject));
                break;
            case (int)State.Battle:
                state = State.Battle;
                player = GameObject.Find("Player");

                jroute = Application.dataPath + @"\Resource\Text\Info\Skill.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option];

                BtnActive(jcur);

                break;
            case (int)State.Strategy:
                state = State.Strategy;
                Debug.Log("INSTANT STRATEGY");
                jroute = Application.dataPath + @"\Resource\Text\Battle\WeaponAction.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option];

                BtnActive(jcur);

                break;

        }


    }
    
    //btn 클릭시 상호작용
    public void OnClick(Text t)
    {
        switch (state)
        {
            case State.Scenario:
                Debug.Log("CLICK_TEXT[Scenario] : " + t.text);
                JToken jkey = jcur[t.text];

                // 복수의 효과 처리 -> 따로 함수 처리
                foreach (JToken code in jkey["effect"])
                {
                    Debug.Log("CLICK_code : " + code.ToString());
                    string decode = code[0].ToString();

                    //dice는 성공 실패 여부가 포함되어 있어 따로 처리를 한다.(나중에 json변경... 2024-01-29)
                    if (decode == "dice") textChanger.GetOpcode(code[0].ToString(), jkey, 1);
                    else textChanger.GetOpcode(code[0].ToString(), code, 1);

                }

                //이벤트 삽입 할거야?
                //EventInformar.CheckAll();

                //다음 문장 출력
                textManager.ReadStory(true);
                break;
            case State.Battle:
                //Debug.Log("CLICK_TEXT[Battle] : " + t.text);
                //클릭한 버튼 내용을 player_info에 반영
                player.GetComponent<CharacterData>().UpdateData(0, t.text);
                break;
            case State.Strategy:
                Debug.Log("CLICK_TEXT[STATEGY]" + t.text);
                //순서가 꼬여서 먼저 비활성화?
                BtnUnActive();

                judgement.DesicionWinner(battleManager.player, battleManager.adjacent_enemy, t.text);

                return;
            default:

                break;
        }
        //destination = new Vector3(0.0f, -2000.0f, -4.0f);
        //StartCoroutine(Moving(gameObject));

        BtnUnActive();
    }

    // sc_obj 클릭, 선택지가 갱신된다.
    public void OnClickObj(GameObject keyword)
    {
        string word = keyword.GetComponent<Keyword>().keyword;
        int idx = keyword.GetComponent<Keyword>().idx;
        ShowSelection("sc_key", idx, 0);
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

    //버튼 텍스트와 활성화
    public void BtnActive(JToken jcur)
    {
        foreach (JToken list in jcur["list"])
        {

            button[len].GetComponentInChildren<Text>().text = list.ToString();
            button[len++].SetActive(true);
        }
    }

    //버튼 비활성화
    public void BtnUnActive()
    {
        for (; len > 0; len--) button[len - 1].SetActive(false);
    }

}
