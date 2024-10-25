using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.ComponentModel;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    [SerializeField] private TextChanger textChanger;
    [SerializeField] private GameObject player;
    [SerializeField] private CombatCalculator judgement;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private ActionList actionList;
    public GameObject[] button;
    [SerializeField] private int len = 0;

    private DiceManager diceManager;
    

    private Vector3 destination = new Vector3(0.0f, -800.0f, -4.0f);
    private Vector2 speed = Vector2.zero;
    //private float time = 0.2f;
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
        diceManager = new DiceManager();
        mainroute = Application.persistentDataPath + "/main.txt";
        //actionList = GetComponent<ActionList>();
    }

    //선택지 나열. 각 상황에 따른 다른 지점 참조
    public void ShowSelection(string option, int idx, int c_state)
    {

        switch (c_state)
        {
            //시나리오인 경우
            case (int)State.Scenario:
                state = State.Scenario;
                jroute = Application.persistentDataPath + "/mainSet.json";
                str = convertJson.MakeJson(jroute);
                jroot = JObject.Parse(str);
                jcur = jroot[option][idx];

                BtnScenarioActive(jcur);
                return;
            //전투의 경우.
            case (int)State.Battle:
                state = State.Battle;
                player = GameObject.Find("Player");

                jroute = Application.persistentDataPath + "/Info/Skill.json";
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

                BtnUnActive();
                break;
            case State.Battle:
                //클릭한 버튼 내용을 player_info에 반영
                //player.GetComponent<PlayerHealth>().UpdateData(0, btnData.displayText.text);

                player.GetComponent<PlayerAction>().SetAction(btnData.displayText.text);
                //actionList.UpdateSet(); // 이거 여기 있어야함..?
                break;

            default:
                BtnUnActive();
                break;
        }

        
    }

    /*
    //selection 자체 move
    IEnumerator Moving(GameObject obj)
    {
        //Debug.Log("moving");
        while (obj.transform.position != destination)
            yield return obj.transform.position = Vector2.SmoothDamp(obj.transform.position, destination, ref speed, time);
    }
    */

    //버튼 텍스트와 활성화
    private void BtnScenarioActive(JToken jcur)
    {
        foreach (string list in jcur["list"]) // string이 형변환 괜찮나?
        {
            //int idx = 0;
            BtnData tmp = button[len].GetComponent<BtnData>();

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
    public void BtnUnActive() { for (; len > 0; len--) button[len - 1].SetActive(false); }

}
