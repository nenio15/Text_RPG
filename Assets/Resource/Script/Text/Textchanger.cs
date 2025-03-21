using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

public class TextChanger : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private GameObject window;
    ConvertJson convertJson = new ConvertJson();

    //임의 설정. 캐릭터 스크립트 필요
    private string player = "플레이어";
    private string space = "숲";

    //경로 설정
    //private string path = @"/Resource/Text/";  //this position is moved so... where..?
    private string main_route, key_route;
    private string cur_main;
    private int cur_move;

    //미리 갈 경로 설정
    public string next_main;
    public int next_move;
    public string event_scenario;
    public string quest_scenario;

    JArray key_jarray, sc_key_jarray;
    JObject key_jroot;

    public Action<string, string> onWindowAction;

    [SerializeField] public JObject jbase;
    (string name, float rate)[] data = { ("1", 1.0f) };

    private void Start()
    {
        //실제 .txt 키 .json
        //main_route = Application.dataPath + path + "main.txt";
        main_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/main.txt";
        key_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/mainSet.json";

        battleManager = FindObjectOfType<BattleManager>();


    }

    public int NewScenarioEnter(int move, string jmain)
    {
        /*
         * 매니저가 새 시나리오(text)를 요구한다.
         * 해당 지역(jmain)으로 진입전, 이곳에 먼저 발생하는 퀘스트나 이벤트가 있는지 서치한다.
         * 만일 있다면, read 순서를 뒤바꾸어 실시한다.
         * read는 긴급이벤트와 간접이벤트 두가지이다만, 그에대한 구현은 아직 고민중이므로 긴급이벤트만 취급한다.
         */
        //Debug.Log("call enter");

        cur_main = jmain;
        cur_move = move;
        string str = Resources.Load<TextAsset>("Text/Scenario/" + jmain).ToString();
        //int op_num = 0;
        int eventcall = -1;
        //Native Object 방식
        jbase = JObject.Parse(str);
        JToken jnow = jbase["scenario"][move];

        //모든 진입의 시작에서만 확인. 진행도중에 방해x
        if (move == 0)
        {
            //퀘스트 유무 확인.
            if (jbase["condition"]["region"].ToString() != null && QuestEncounter(jbase["condition"]["region"].ToString()) == 1)
            {
                Debug.Log("move to quest" + quest_scenario);
                cur_main = quest_scenario;
                ReadScenarioParts(0, quest_scenario);
                //Movement((int)cur_typecode, cur_main, "move"); //해당 퀘스트가 끝나면 어디로 가게하죠? - 퀘스트는 마지막에 move.지역이 있는가?
                return 0;
            }

            //이벤트 유무와 리딩
            eventcall = EventEncounter(jbase["condition"]["region"].ToString());
            if (eventcall >= 0)
            {
                cur_main = event_scenario;
                ReadScenarioParts(0, event_scenario);
                return 0;
            } //일단 0,1 하나로 퉁치기.
        }

        //기존에 받은 시나리오 리딩
        ReadScenarioParts(move, jmain);
        return 0;
    }

    //시나리오 리더기 . 리딩만 하도록 모듈화
    public int ReadScenarioParts(int move, string jmain)
    {
        Debug.Log(jmain + move);
        string str = Resources.Load<TextAsset>("Text/Scenario/" + jmain).ToString();
        string key_str = convertJson.MakeJson(key_route);
        int op_num = 0;

        // read 할 부분 초기화
        File.WriteAllText(key_route, "{ \"key\" : [{}], \"sc_key\" : [{}] }");    // 초기화
        File.WriteAllText(main_route, "");                                       // main reset

        key_jarray = new JArray();
        sc_key_jarray = new JArray();

        //Native Object 방식
        jbase = JObject.Parse(str);
        key_jroot = JObject.Parse(key_str);

        JToken jnow = jbase["scenario"][move];


        foreach (JToken jscript in jnow["script"])
        {
            GetOpcode(jscript["type"].ToString(), jscript, op_num);
        }
        //if( eventcall == 0) { Debug.Log("move to event after ch.1"); }

        return 0;
    }

    public int GetOpcode(string script_type, JToken cur_blockcode, int idx)  // idx == 1 -> effect code
    {
        JToken cur_typecode = cur_blockcode;
        string breakpoint = "#\n"; //모든 스크립트가 중단점을 필요로 하는가?

        switch (script_type)
        {
            //본문 관련
            case "dia": //본문
                Dialog(cur_blockcode["dia"], idx);
                break;
            case "con dia": //조건부 대화문. 여기서... 다이스 굴리는 형식으로, 그거 이펙트 따로 빼고, 
                //condition을 확인하고, 참이면 dialog를 첨가한다. 아니면 그냥 안 한다.(혹은 '실패'로그 띄워준다던가)
                Debug.Log("con is" + cur_blockcode["con"]);
                Debug.Log("con dia is" + cur_blockcode["dia"]);
                Dialog(cur_blockcode["dia"], idx);
                break;


            //선택지 관련
            case "choice": //선택지 key 추가
                Choice(cur_blockcode["choice"]);
                breakpoint = "#key\n";
                break;

            //추가창 띄우기 - 주로 상점용
            case "window open":
                cur_typecode = cur_blockcode["open"];
                Window(cur_typecode[0].ToString(), cur_typecode[1].ToString());
                cur_move = (int)cur_blockcode["close"];
                break;

            //이동 관련
            case "script move":
                //if ((int)code[idx] == -1) break; // 필요한지 ㅁ?ㄹ
                cur_typecode = cur_blockcode["move"];
                Movement((int)cur_typecode, cur_main, "move");
                break;
            case "scene over": //json 이동
                cur_typecode = cur_blockcode["over"];
                Movement((int)cur_typecode[0], cur_typecode[1].ToString(), "over");
                break;

            case "wait": //흠좀무.. 필요한 기능은 맞지. 그러면 이 코드 자체를 느리게 해야할텐데. 그게 구현이 쉽냐의 문제. 나중에 구현할것.


            //이벤트 관련
            case "battle": //배틀 소환
                cur_typecode = cur_blockcode["battle"];
                Battle(cur_typecode[0].ToString(), cur_typecode[1].ToString(), (int)cur_typecode[2], (int)cur_typecode[3]);
                break;
            case "quest": //퀘스트 진행도 내지는 획득 류

            case "effect": //player hp나 mp, exp등의 값을 추가, 제거하는 레벨. + 아이템, 서사 획득.


            //이펙트 관련
            case "sound": //소리효과 재생 (옵션 있)
            case "img": //삽화추가
                break;
            //case "npc": //rpl과 같은? 대화로 들어가기..

            default:
                Debug.LogError(script_type + " is non type");
                break;

        }

        //중단점 추가
        File.AppendAllText(main_route, breakpoint);

        return 0;
    }

    //main.txt에 text첨가
    private void Dialog(JToken logue, int idx) //idx 삭제할것
    {
        foreach (JToken jdes in logue)
            File.AppendAllText(main_route, Setstring(jdes.ToString()) + '\n');  //System.IO.
        return;
    }

    //다음 스크립트 지정
    private void Movement(int move, string main, string sign)
    {
        //Debug.Log(next_main + main);
        next_move = move;
        next_main = main;
        File.AppendAllText(main_route, '#' + sign + '\n');
    }

    private void Window(string type, string itemsheet)
    {
        window.SetActive(true);
        onWindowAction(type, itemsheet);
        //textManager.Reread();
        //자동읽기. 괜춚?
        //textManager.ClearText();
        textManager.ReadPage();
        //textManager.Reread();
        Debug.Log(type + itemsheet);
    }

    public void CloseWindow()
    {
        window.SetActive(false);
        ReadScenarioParts(cur_move, cur_main);
        textManager.ClearText();
        textManager.ReadPage();
        //textManager.Reread();
    }

    //key를 mainSet으로 세팅
    private void Choice(JToken selection)
    {
        key_jarray.Add(selection.ToObject<JObject>());  // jobject만 add
        key_jroot["key"] = key_jarray;

        File.WriteAllText(key_route, key_jroot.ToString());
    }

    //전투 진입 중단코드 삽입
    private void Battle(string jbattle, string root, int num, int situ)
    {
        //배틀 미리 세팅
        battleManager.BattlePreset(jbattle, root, num, situ); //현재 매개변수 거의 의미없게 세팅되어있음.

        //TextManager가 읽으면 배틀 진입
        File.AppendAllText(main_route, "#btl\n");

        return;
    }

    private int QuestEncounter(string n)
    {
        //현재 받은 퀘스트에서 현재 지역 (조건)과 부합하는지 확인. 
        //부합하다면 퀘스트 선 실행. 얘는 일부 묘사가 있냐 묻자면.. 글쎄다.? 나중에 퀘스트 만들면 판별나겠지.

        return 0;
    }

    private int EventEncounter(string region)
    {
        //던전/마을 등의 특수 지역일 경우, 이벤트 종류를 제한한다.
        //특정 퀘스트 지역에 진입시, 무조건적으로 퀘스트를 우선한다.?
        ProbabilityCalculator probabilityCalculator = new ProbabilityCalculator();
        event_scenario = probabilityCalculator.Probability(region);

        //아무 일도 일어나지 않았다.
        if (event_scenario == "none") return -1;

        Debug.Log("here is " + region + event_scenario);
        return 0; //0.강제 이벤트(바로 시작)
        //return 1; //1.신호 이벤트(일부 묘사 + 갈지 선택지)

    }

    //폐기 예정.
    /*
    // key, sc_key -> main.json to use for keywords
    private void CheckKeys(string k, JToken cur_script) // how to use k..?
    {
        k = "0";
        // key      list -> effect 일반적인 선택지. 따라서 스탯만 관여
        // sc_key   list -> action, skill -> dice -> effect 플레이어의 직업, 스킬에 따라 다르게 관여
        foreach (JProperty keys in cur_script)
        {
            if (keys.Name == "key")
            {
                key_jarray.Add(cur_script["key"].ToObject<JObject>());  // jobject만 add
                key_jroot["key"] = key_jarray;
                k = "#key\n";
            }
            else if (keys.Name == "sc_key")
            {
                sc_key_jarray.Add(cur_script["sc_key"].ToObject<JObject>());
                key_jroot["sc_key"] = sc_key_jarray;                    //key_jroot["sc_key"] = cur_script["sc_key"].ToObject<JObject>();

                if (k == "#key\n") k = "#key sc_key\n";
                else k = "#sc_key\n";     // key가 씹히는 경우가 있다... 흠..
                

                //list selection을 받아, 아래 선택지obj를 표시한다.
                //key opcode는 GetOpcode("key", key[select]); 로 따로 취급
            }
            else if (keys.Name == "dia")
                k = "#\n";
        }

        if( k[0] == '#')
            File.AppendAllText(main_route, k);
        File.WriteAllText(key_route, key_jroot.ToString());
    }

    // *** dice의 succ, fail을 effect 안에다 넣을것!! *** 
    private void RollDice(JToken info)
    {
        string stat;
        int dice_type = 6, dice_up = 0;
        foreach (JToken list in info["effect"])
            if (list[0].ToString() == "dice")
            {
                stat = list[1].ToString();
                dice_type = (int)list[3]; //방식이 달ㄹ라요...
                dice_up = (int)list[2];
            }

        int roll_result = UnityEngine.Random.Range(0, dice_type);
        Debug.Log("DICE_ROLL : " + roll_result);

        // 대충 상승치.(player 값을 클래스에 따로 받아야할듯 한데...
        //if ((int)player[stat] >= dice_up) result++;
        string result = (roll_result >= dice_type / 2) ? "succ" : "fail";
        Debug.Log("DICE_RESULT : " + result);

        foreach (JToken code in info[result])
            GetOpcode(code[0].ToString(), code, 1);

    }

    private void Region(string spot, int chap, int detail)     //지역별 get
    {
        //string str = convertJson.MakeJson(Application.dataPath + @"/Resource/Text/Field/Region.json");
        string str = Resources.Load<TextAsset>("Text/Field/Region").ToString();
        JObject jroot = JObject.Parse(str);
        JToken jregion = jroot["Region"];                                                                           //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class로 문자열..?

        //if (jregion["name"].ToString() == spot)
        foreach (JToken des in jregion["Type"][chap]["description"]) //임시 for
            File.AppendAllText(main_route, Setstring(des.ToString()) + '\n');
        //Debug.Log(des);

        return;
    }

    private void NpcCall(string name, string situ, string num)
    {
        //해당 npc를 찾아서, situ를 맞추고, num에 있는 dia나 다른 것을 행함
        //string str = convertJson.MakeJson(Application.dataPath + @"/Resource/Text/Npc/Npc.json");
        string str = Resources.Load<TextAsset>("Text/Npc/Npc").ToString();
        JObject jroot = JObject.Parse(str);
        JToken jnpc = jroot[name][situ][num];
        //convert_hash.Add("#" + name, jnpc["name"].ToString()); // ex) #edward : 에드워드
        foreach (JToken dia in jnpc["dia"])
            File.AppendAllText(main_route, Setstring(dia.ToString()) + '\n');   //" "는 알아서 추가하던가.. 말던가,

        //CheckKeys("opcode..?", jnpc);

        foreach (JProperty others in jnpc)
            if (others.Name == "effect")
                foreach(JToken code in jnpc["effect"])
                    GetOpcode(code[0].ToString(), code, 1);


        //if(jnpc[situ][num]["key"] != null) CreateSelection("key", jnpc[situ][num]);
        //CheckKeys("opcode..?", jnpc);
        return;
    }
    */



    private string Setstring(string raw_string)
    {
        string[] divied = raw_string.Split('"');
        string sentence = "";

        for (int i = 1; i < divied.GetLength(0) - 1; i++)
            sentence += divied[i].Replace('\\', '"'); // 대화문 살리기

        return ReplaceS(sentence);
    }

    private string ReplaceS(string c_line)  // + Diction이용, key value값?
    {
        if (!c_line.Contains("#")) return c_line;
        // string player = "홍길동"
        // just 'for'? or json[converter] = #'list
        c_line = c_line.Replace("#player", player);
        c_line = c_line.Replace("#space", space);
        c_line = c_line.Replace("#edward", "에드워드");
        c_line = c_line.Replace("#monster", "goblin");
        return c_line;
    }

}