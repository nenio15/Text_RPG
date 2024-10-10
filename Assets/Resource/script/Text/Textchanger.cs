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
    [SerializeField] private TextManager textmanager;
    [SerializeField] private BattleManager battlemanager;
    ConvertJson convertJson = new ConvertJson();

    //임의 설정. 캐릭터 스크립트 필요
    private string player = "플레이어";
    private string space = "숲";

    //for #player & path + "\Scenario... \Npc...
    //private Dictionary<string, string> convert_hash = new Dictionary<string, string>(); // .Add(key, Value);

    //경로 설정
    private string path = @"/Resource/Text/";  //this position is moved so... where..?
    private string mainroute, keyroute;
    private string curmain;

    //미리 갈 경로 설정
    public string pre_main;
    public int pre_move;

    JArray key_jarray, sc_key_jarray;
    JObject key_jroot;

    [SerializeField] public JObject jbase;

    private void Start()
    {
        //실제 .txt 키 .json
        mainroute = Application.dataPath + path + "main.txt";
        keyroute = Application.dataPath + path + "main.json";

        battlemanager = FindObjectOfType<BattleManager>();
    }

    public int ReadScenarioParts(int move, string jmain)
    {
        // 시나리오 이름으로 추적. (폴더명(@Scenario))\파일명\시나리오명
        curmain = jmain;
        string scnroute = Application.dataPath + path + @"Scenario/" + curmain + ".json";
        string str = convertJson.MakeJson(scnroute);
        string key_str = convertJson.MakeJson(keyroute);

        int op_num = 0;

        // read 할 부분 초기화
        File.WriteAllText(keyroute, "{ \"key\" : [{}], \"sc_key\" : [{}] }");    // 초기화
        File.WriteAllText(mainroute, "");                                       // main reset

        key_jarray = new JArray(); 
        sc_key_jarray = new JArray();

        //Native Object 방식
        jbase = JObject.Parse(str);
        key_jroot = JObject.Parse(key_str);

        JToken jnow = jbase["scenario"][move];

        //script문 따라가기
        foreach (JToken jscript in jnow["script"])
        {
            //JToken jscript = jnow["script"][i];
            //JToken jlist = jnow["scriptlist"][op_num++]; //이것도 없애긴 해야하는데 op_num여기만 쓰임?

            /*
             * 1.종류를 분류한다.  type을 읽는다. script본문이 필요하다.
             * 2.각 분류마다 정해진 처리를 진행한다.
             * 3.그 값을 어딘가에 처리. .. 음.
             */
            //정해진 code list를 읽고, key를 읽는다.
            GetOpcode(jscript["type"].ToString(), jscript, op_num);
            //CheckKeys("::opcode here", jscript);
        }

        //Debug.Log("readParts end one");
        return 0;
    }

    //아예 하위 스크립트로 빼버릴 것. 내지는 하위 함수를 하위 스크립트로
    public int GetOpcode(string script_type, JToken cur_blockcode, int idx)  // idx == 1 -> effect code
    {
        JToken cur_typecode = cur_blockcode;
        string breakpoint = "#\n"; //모든 스크립트가 중단점을 필요로 하는가?

        //이거... 하위 스크립트로 빼자 그냥.. 그게 났겠다.
        //"type" : "dia" / "con dia" / "choice" / "sound" / "illust" / "effect" / "script move" / "scene over" / "wait/hold" / 
        switch (script_type)
        {
            //본문 관련
            case "dia": //본문
                //cur_typecode = cur_blockcode["dia"];
                Dialog(cur_blockcode["dia"], idx);
                break;
            case "con dia": //조건부 대화문. 여기서... 다이스 굴리는 형식으로, 그거 이펙트 따로 빼고, 
                //condition을 확인하고, 참이면 dialog를 첨가한다. 아니면 그냥 안 한다.(혹은 '실패'로그 띄워준다던가)
                //cur_typecode = cur_blockcode["con"];
                Debug.Log("con is" + cur_blockcode["con"]);
                //cur_typecode = cur_blockcode["dia"];
                Debug.Log("con dia is" + cur_blockcode["dia"]);
                Dialog(cur_blockcode["dia"], idx);
                break;


            //선택지 관련
            case "choice": //선택지 key 추가
                //cur_typecode = cur_blockcode["choice"];
                Choice(cur_blockcode["choice"]);
                breakpoint = "#key\n";
                break;


            //이동 관련
            case "script move": 
                //if ((int)code[idx] == -1) break; // 필요한지 ㅁ?ㄹ
                cur_typecode = cur_blockcode["move"];
                Movement((int)cur_typecode, curmain, "move");                
                break;
            case "scene over": //json 이동
                cur_typecode = cur_blockcode["over"];
                Movement((int)cur_typecode[0], cur_typecode[1].ToString(), "over");
                break;

            case "wait": //흠좀무.. 필요한 기능은 맞지. 그러면 이 코드 자체를 느리게 해야할텐데. 그게 구현이 쉽냐의 문제. 나중에 구현할것.


            //이벤트 관련
            case "battle": //배틀 소환
                cur_typecode = cur_blockcode["battle"];
                //좀 더러울지도...
                Battle(cur_typecode[0].ToString(), cur_typecode[1].ToString(), (int)cur_typecode[2], (int)cur_typecode[3]);
                break;
            case "quest": //퀘스트 진행도 내지는 획득 류

            case "effect": //player hp나 mp, exp등의 값을 추가, 제거하는 레벨. + 아이템, 서사 획득.


            //이펙트 관련
            case "sound": //소리효과 재생 (옵션 있)
            case "img": //삽화추가
                break;
            //case "npc": //rpl과 같은? 대화로 들어가기.. 나중에 구현

            default:
                Debug.LogError(script_type + " is non type");
                break;

        }

        //모든 지점에 breakpooint가 과연 필요할까? ex) sound와 dia의 합심
        File.AppendAllText(mainroute, breakpoint);

        return 0;

        /*
        //Debug.Log("GETcode : " + op + " & "+ code);
        switch (op)
        {
            case "jmp": //다른 시나리오로 이동  // jmp/0/plain/Plain | 0/plain/Plain
            case "rpl": //같은 json(시나리오/마을)에서의 이동
                if ((int)code[idx] == -1) break; // 필요한지 ㅁ?ㄹ
                pre_move = (int)code[idx++];
                pre_main = (op == "rpl") ? curmain : code[idx].ToString();

                File.AppendAllText(mainroute, '#' + op + '\n');
                break;
            case "dia": //시나리오 show // effect : [ "dia", [ [1], [2] ] ]   // [1]은 2차원 배열이라 할 수 있겠지..
                if (idx == 1) foreach (JToken jdes in code[1]) 
                        File.AppendAllText(mainroute, Setstring(jdes.ToString()) + '\n');
                else foreach (JToken jdes in code) 
                        File.AppendAllText(mainroute, Setstring(jdes.ToString()) + '\n');  //System.IO.
                break;
            case "btl":
                Battle(code[idx++].ToString(), code[idx++].ToString(), (int)code[idx++], (int)code[idx++]);
                break;
            case "mov": //call region. area. moment //버려진 명령어. 2024-07-09. ※폐기할것.
                Region(code[idx++].ToString(), (int)code[idx++], (int)code[idx++]);   //Region(code[op][i++].ToString(), (int)code[op][i++], (int)code[op][i++]);
                break;
            case "npc": //뼈대만 남은 코드..
                NpcCall(code[idx++].ToString(), code[idx++].ToString(), code[idx++].ToString());
                break;
            default:
                //Debug.Log(op + " don't exist on Decodeing fucntion");
                break;
        }

        return 0;
        */
    }

    //main.txt에 text첨가
    private void Dialog(JToken logue, int idx)
    {
        //idx 삭제할것.
        foreach (JToken jdes in logue)
            File.AppendAllText(mainroute, Setstring(jdes.ToString()) + '\n');  //System.IO.
        return;
    }
    
    private void Movement(int move, string main, string sign)
    {
        pre_move = move;
        pre_main = main;
        File.AppendAllText(mainroute, '#' + sign + '\n');

    }

    //key내용을 옮깁니다~~~.
    private void Choice(JToken selection)
    {
        key_jarray.Add(selection.ToObject<JObject>());  // jobject만 add
        key_jroot["key"] = key_jarray;

        File.WriteAllText(keyroute, key_jroot.ToString());
    }

    //폐기 예정.
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
            File.AppendAllText(mainroute, k);
        File.WriteAllText(keyroute, key_jroot.ToString());
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
        string str = convertJson.MakeJson(Application.dataPath + @"/Resource/Text/Field/Region.json");
        JObject jroot = JObject.Parse(str);
        JToken jregion = jroot["Region"];                                                                           //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class로 문자열..?

        //if (jregion["name"].ToString() == spot)
        foreach (JToken des in jregion["Type"][chap]["description"]) //임시 for
            File.AppendAllText(mainroute, Setstring(des.ToString()) + '\n');
        //Debug.Log(des);

        return;
    }

    private void NpcCall(string name, string situ, string num)
    {
        //해당 npc를 찾아서, situ를 맞추고, num에 있는 dia나 다른 것을 행함
        string str = convertJson.MakeJson(Application.dataPath + @"/Resource/Text/Npc/Npc.json");
        JObject jroot = JObject.Parse(str);
        JToken jnpc = jroot[name][situ][num];
        //convert_hash.Add("#" + name, jnpc["name"].ToString()); // ex) #edward : 에드워드
        foreach (JToken dia in jnpc["dia"])
            File.AppendAllText(mainroute, Setstring(dia.ToString()) + '\n');   //" "는 알아서 추가하던가.. 말던가,

        CheckKeys("opcode..?", jnpc);

        foreach (JProperty others in jnpc)
            if (others.Name == "effect")
                foreach(JToken code in jnpc["effect"])
                    GetOpcode(code[0].ToString(), code, 1);


        //if(jnpc[situ][num]["key"] != null) CreateSelection("key", jnpc[situ][num]);
        //CheckKeys("opcode..?", jnpc);
        return;
    }

    private void Battle(string jbattle, string root, int num, int situ)
    {
        
        //File.AppendAllText(mainroute, "#battle " + name + num + '\n');
        //종류 : 몬스터, 인간형 등등?
        //이름, 숫자. 그리고 시츄는 발각, 기습, 상태이상 등?
        //Debug.Log("JSON[monster] : " + jbattle + "이 " + root + "발생.");

        //배틀 미리 세팅
        battlemanager.BattlePreset(jbattle, root, num, situ); //"goblin", "forest_goblin", 1, 0]},

        //TextManager가 읽으면 배틀 진입
        File.AppendAllText(mainroute, "#btl\n");

        return;
    }

        /*
         * sc_key에는 관측이 됨. key에서도 아마 필요할거같아보임
         *  effect : { .., ["dice", "sen", 1, 1, 2] 형식 받고
         *  dice : { "1", "2" : ... 로 따로 취급하자. dice는 좀 많이 복잡할거같네..
         */

        //System.IO.File.AppendAllText(mainroute, "\n"); // + final null line
    

    private string Setstring(string raw_string)
    {
        string[] divied = raw_string.Split('"');
        string sentence = "";
        
        for(int i = 1; i < divied.GetLength(0) - 1; i++)
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