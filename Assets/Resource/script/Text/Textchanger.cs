using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    private string path = @"\Resource\Text\";  //this position is moved so... where..?
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
        string scnroute = Application.dataPath + path + @"Scenario\" + curmain + ".json";
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

        do
        {
            //Debug.Log("CHANGER : " + move);
            JToken jnow = jbase["scenario"][move];
            foreach (JToken jscript in jnow["script"]) //json 시나리오의 양상을 따라간다.
            {
                //정해진 code list를 읽고, key를 읽는다.
                JToken jlist = jnow["scriptlist"][op_num++];
                GetOpcode(jlist.ToString(), jscript[jlist.ToString()], 0); 
                CheckKeys("::opcode here", jscript);
            }

        } while (false);

        //Debug.Log("readParts end one");
        return 0;
    }

    //아예 하위 스크립트로 빼버릴 것. 내지는 하위 함수를 하위 스크립트로
    public int GetOpcode(string op, JToken code, int idx)  // idx == 1 -> effect code
    {
        //나중에... 바꿀것.
        /*
         *  //... 이거를 사전해석시켜서 생기는 문제가 많은데(rpl, sound, img, move, condia)... 그냥 그때그때 읽는걸로 해버려 그냥?
         *  //그러면 위의 ReadScenarioParts도 변경해서. page넘길때마다 실행되도록 바꿔야함. 추가로 값도 이리저리. 함수를 하나더 만들어야함.
         *  
         *  //이거... 하위 스크립트로 빼자 그냥.. 그게 났겠다.
         * foreach (JProperty opcodes in cur_script){ 
         *  switch(opcodes.Name)
         *  {
         *      case "rpl": //이동... 이것도 애매한게 발동 시점이.. key의 effect에 있는게 아닌 이상 코드가 꼬여.... 이것도 고민해봐야겠네.
         *      
         *      case "dia": //본문
         *      
         *      case "sound": //소리효과 재생 (옵션 있)
         *      
         *      case "img": //삽화추가
         *      
         *      case "move": //맵 상 이동. (이걸 여기 넣는게 맞겠지... 다른 조건에서 집어넣으려면 제약상황이 많아져. 결국 json에 넣는거는 필연이고.
         *      
         *      case "key": //선택지 key 추가
         *      
         *      case "condia": //조건부 대화문. 여기서... 다이스 굴리는 형식으로, 그거 이펙트 따로 빼고, 
         *  
         *      //case "wait": //흠좀무.. 필요한 기능은 맞지. 그러면 이 코드 자체를 느리게 해야할텐데. 그게 구현이 쉽냐의 문제. 나중에 구현할것.
         * 
         *      case "btl": //배틀 소환
         *      
         *      //case "npc": //rpl과 같은? 대화로 들어가기.. 나중에 구현
         *      
         *      case "quest": //퀘스트 진행도 내지는 획득 류
         *      
         *      case "value": //player hp나 mp, exp등의 값을 추가, 제거하는 레벨. + 아이템, 서사 획득.
         *      
         *  }
         * }
         */


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
                Debug.Log(op + " don't exist on Decodeing fucntion");
                break;
        }

        return 0;
    }

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

        int roll_result = Random.Range(0, dice_type);
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
        string str = convertJson.MakeJson(Application.dataPath + @"\Resource\Text\Field\Region.json");
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
        string str = convertJson.MakeJson(Application.dataPath + @"\Resource\Text\Npc\Npc.json");
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
        Debug.Log("JSON[monster] : " + jbattle + "이 " + root + "발생.");

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