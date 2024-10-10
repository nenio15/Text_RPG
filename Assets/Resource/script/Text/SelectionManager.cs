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
        //�̰� �ϴ� �������� �ٲܰ� 2024-07-30
        textManager = FindObjectOfType<TextManager>();
        textChanger = GameObject.FindObjectOfType<TextChanger>();
        //player = GameObject.Find("Player");
        judgement = FindObjectOfType<Judgement>();
        battleManager = GameObject.FindObjectOfType<BattleManager>();
        diceManager = new DiceManager();

        mainroute = Application.dataPath + @"\Resource\Text\main.txt";
    }

    //������ ����. �� ��Ȳ�� ���� �ٸ� ���� ����
    public void ShowSelection(string option, int idx, int c_state)
    {

        switch (c_state)
        {
            //�ó������� ���
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
            //������ ���.
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
    
    //btn Ŭ���� ��ȣ�ۿ�
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

                // ���̽��� ó��
                if (btnData.diceType != "")
                {
                    result = diceManager.RollingDice(btnData.difficulty);
                    jkey = jcur[btnData.displayDescription][result];
                }

                // ȿ���� ó��
                foreach (JToken block in jkey)
                {
                    //Debug.Log("CLICK_code : " + code.ToString());
                    textChanger.GetOpcode(block["type"].ToString(), block, 1);
                    //textChanger.GetOpcode(code[0].ToString(), code, 1);
                }

                //�̺�Ʈ ���� �Ұž�?
                //EventInformar.CheckAll();

                //���� ���� ���
                //textManager.ReadStory(true);
                textManager.Reread();
                break;
            case State.Battle:
                //Ŭ���� ��ư ������ player_info�� �ݿ�
                player.GetComponent<CharacterData>().UpdateData(0, btnData.displayText.text);
                break;
            case State.Strategy:
                Debug.Log("CLICK_TEXT[STATEGY]" + btnData.displayText.text);
                //������ ������ ���� ��Ȱ��ȭ?
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

    //��ư �ؽ�Ʈ�� Ȱ��ȭ
    private void BtnScenarioActive(JToken jcur)
    {
        foreach (string list in jcur["list"]) // string�� ����ȯ ������?
        {
            //int idx = 0;
            BtnData tmp = button[len].GetComponent<BtnData>();

            /*
            //effect�� ���빰�� ���� ���, ���� ��ȯ
            //if (jcur[list]["effect"] == null) { Debug.LogError("[ERROR][JSON] : effect contain non contents"); return; }
            //JToken jtmp = jcur[list]["effect"][0];

            //dice�� �ִ� ���
            //if (jtmp[idx].ToString().Equals("dice")) tmp.Active(list, jtmp[++idx].ToString(), (int)jtmp[++idx]);
            //else tmp.Active(list);
            */
            tmp.Active(list);

            //��ư �� Ȱ��ȭ
            button[len++].SetActive(true);
        }
    }

    //���� ��ư Ȱ��ȭ
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


    //��ư ��Ȱ��ȭ
    private void BtnUnActive()
    {
        for (; len > 0; len--) button[len - 1].SetActive(false);
    }

}
