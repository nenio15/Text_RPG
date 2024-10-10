using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.ComponentModel;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine.Timeline;
//using System.Globalization;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    [SerializeField] private TextChanger textChanger;
    [SerializeField] private GameObject player;
    [SerializeField] private Judgement judgement;
    [SerializeField] private BattleManager battleManager;
    public GameObject[] button;
    [SerializeField] private int len = 0;

    private DiceManager diceManager;
    

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
        //이거 싹다 지정으로 바꿀것 2024-07-30
        textManager = FindObjectOfType<TextManager>();
        textChanger = GameObject.FindObjectOfType<TextChanger>();
        //player = GameObject.Find("Player");
        judgement = FindObjectOfType<Judgement>();
        battleManager = GameObject.FindObjectOfType<BattleManager>();
        diceManager = new DiceManager();

        mainroute = Application.dataPath + @"\Resource\Text\main.txt";
    }

    //선택지 나열. 각 상황에 따른 다른 지점 참조
    public void ShowSelection(string option, int idx, int c_state)
    {

        switch (c_state)
        {
            //시나리오인 경우
            case (int)State.Scenario:
                state = State.Scenario;
                jroute = Application.dataPath + @"\Resource\Text\main.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option][idx];
                //Debug.Log("SELECT : " + option);


                //destination = new Vector3(0.0f, -800.0f, -4.0f);
                //StartCoroutine(Moving(gameObject));
                BtnScenarioActive(jcur);
                return;
            //전투의 경우.
            case (int)State.Battle:
                state = State.Battle;
                player = GameObject.Find("Player");

                jroute = Application.dataPath + @"\Resource\Text\Info\Skill.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option];

                BtnBattleActive(jcur);
                return;
            case (int)State.Strategy:
                state = State.Strategy;
                Debug.Log("INSTANT STRATEGY");
                jroute = Application.dataPath + @"\Resource\Text\Battle\WeaponAction.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option];

                BtnBattleActive(jcur);
                return;
        }
    }
    
    //btn 클릭시 상호작용
    public void OnClick(GameObject btn)
    {
        BtnData btnData = btn.GetComponent<BtnData>();

        switch (state)
        {
            case State.Scenario:
                //Debug.Log("CLICK_TEXT[Scenario] : " + btnData.displayDescription);
                JToken jkey = jcur[btnData.displayDescription];
                //JToken jkey = jcur[btnData.displayDescription]["effect"];
                string result;

                // 다이스롤 처리
                if (btnData.diceType != "")
                {
                    result = diceManager.RollingDice(btnData.difficulty);
                    jkey = jcur[btnData.displayDescription][result];
                }

                // 효과들 처리
                foreach (JToken block in jkey)
                {
                    //Debug.Log("CLICK_code : " + code.ToString());
                    textChanger.GetOpcode(block["type"].ToString(), block, 1);
                    //textChanger.GetOpcode(code[0].ToString(), code, 1);
                }

                //이벤트 삽입 할거야?
                //EventInformar.CheckAll();

                //다음 문장 출력
                //textManager.ReadStory(true);
                textManager.Reread();
                break;
            case State.Battle:
                //클릭한 버튼 내용을 player_info에 반영
                player.GetComponent<CharacterData>().UpdateData(0, btnData.displayText.text);
                break;
            case State.Strategy:
                Debug.Log("CLICK_TEXT[STATEGY]" + btnData.displayText.text);
                //순서가 꼬여서 먼저 비활성화?
                BtnUnActive();

                judgement.DesicionWinner(battleManager.player, battleManager.adjacent_enemy, btnData.displayText.text);

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
    private void BtnScenarioActive(JToken jcur)
    {
        foreach (string list in jcur["list"]) // string이 형변환 괜찮나?
        {
            //int idx = 0;
            BtnData tmp = button[len].GetComponent<BtnData>();

            /*
            //effect의 내용물이 없는 경우, 에러 반환
            //if (jcur[list]["effect"] == null) { Debug.LogError("[ERROR][JSON] : effect contain non contents"); return; }
            //JToken jtmp = jcur[list]["effect"][0];

            //dice가 있는 경우
            //if (jtmp[idx].ToString().Equals("dice")) tmp.Active(list, jtmp[++idx].ToString(), (int)jtmp[++idx]);
            //else tmp.Active(list);
            */
            tmp.Active(list);

            //버튼 뷰 활성화
            button[len++].SetActive(true);
        }
    }

    //전투 버튼 활성화
    private void BtnBattleActive(JToken jcur)
    {
        if (jcur["list"] == null) { Debug.LogError("[ERROR][JSON] : battle action has no contents"); return; }
        
        foreach (string list in jcur["list"])
        {
            BtnData tmp = button[len].GetComponent<BtnData>();
            tmp.Active(list);
            button[len++].SetActive(true);
        }
    }


    //버튼 비활성화
    private void BtnUnActive()
    {
        for (; len > 0; len--) button[len - 1].SetActive(false);
    }

}
