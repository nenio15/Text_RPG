using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Textchanger : MonoBehaviour
{
    [SerializeField] private TextManager textmanager;

    //임의 설정. 캐릭터 스크립트 필요
    private string player = "플레이어";
    private string space = "숲";

    //for #player & path + "\Scenario... \Npc...
    private Dictionary<string, string> convert_hash = new Dictionary<string, string>(); // .Add(key, Value);
        //private Dictionary<string, string> connet_path = new Dictionary<string, string>;
    private string path = @"\Resource\Text\";  //this position is moved so... where..?
    private string mainroute, keyroute;

    JArray key_jarray, sc_key_jarray;
    JObject key_jroot;

    [SerializeField] public JToken jbase; // public이어야 참조 되는 거지?

    public int readScenarioParts(int move, string jmain, string jsub)
    {
        //초기 path설정. 텍스트 자체에 보여지는 .txt, 실제 자료형 .json
        mainroute = Application.dataPath + path + "main.txt"; 
        keyroute = Application.dataPath + path + "main.json";
        Debug.Log(move + "에서 " + jmain + "그리고 " + jsub);

        // 시나리오 이름으로 추적. (폴더명(@Scenario))\파일명\시나리오명
        string scnroute = Application.dataPath + path + @"Scenario\" + jmain + ".json";  //\Resource\Text\Scenario\scenarion.json
        string str = MakeJson(scnroute);
        string key_str = MakeJson(keyroute);

        // read 할 부분 초기화
        File.WriteAllText(keyroute, "{ \"key\" : [{}], \"sc_key\" : [{}] }");    // 초기화
        File.WriteAllText(mainroute, "");                                       // main reset
        
        int op_num = 0;
        
        key_jarray = new JArray(); 
        sc_key_jarray = new JArray();

        //Native Object 방식
        JObject jroot = JObject.Parse(str);
        key_jroot = JObject.Parse(key_str);

        //condition 확인절차. (고민)
        //어떤 시나리오 리스트를 받을지, 다른 json에 정리시키기.. (scenario selector.cs)
        jbase = jroot[jsub]; //해당 시나리오

        do
        {
            Debug.Log("CHANGER : " + move);
            JToken jnow = jbase["scenario"][move];
            int lnd_cmd = 0; // non 명령어
            //check state       //Debug.Log(jnow["chapter"].ToString() + "\nsynopciys : " + jnow["synopciys"].ToString());
            foreach (JToken jscript in jnow["script"])
            {
                JToken jlist = jnow["scriptlist"][op_num++];
                lnd_cmd = GetOpcode(jlist.ToString(), jscript[jlist.ToString()], 0);
                CheckKeys("::opcode here", jscript);

                if (lnd_cmd == 1) return (int)jscript[jlist.ToString()][0]; //상위 명령이 필요한 경우, 복귀 (일단 임의임....... 무슨 상위냐면 rpl관련이라 리셋이 필요
                //상위에게 요구하는 바는... 스크립트가 종속관계가 맞는가?
                //대등관계로 서로 정보를 요구하는 형식이 될 순 없는가. 그러면 이 스크립트가 독립을 해야해.
                //독립성을 띄느냐는... 모델과 뷰 + 컨트롤러의 형식을 띄고 있지. 독립이 가능해. 그러면... 흠
            }

        } while (false); //그냥 뒀어요. 나중에 조건 달 수도 있어서


        Debug.Log("readParts end one");
        return 0;
    }


    //2024-01-04 | opocde도 따로 빼자 하나의 함수에 하나의 기능을.

    //mainroute의 초기화 위치 땜 public이 불가함..?
    //get scripts ( { } per 1 ) -> "op" : ["codes", "res1", "res2" ...]
    //아예 하위 스크립트로 빼버릴 것
    public int GetOpcode(string op, JToken code, int idx)  // idx == 1 -> effect code
    {
        //Debug.Log("GETcode : " + op + " & "+ code);
        switch (op)
        {
            
            case "jmp": //다른 시나리오, 스크립트로 이동 (그러므로 if문 내용은 안 쓸듯... 2024-01-29
                Debug.Log("REQUEST[event] : occuring?");

                //if ((int)code[1] != 0) textmanager.endStoryPart((int)code[1], "", "");  //move cur scenario
                textmanager.endStoryPart(0, code[2].ToString(), code[3].ToString()); //move another scenario
                break;
            case "rpl": //같은 시나리오/마을에서의 이동
                if ((int)code[idx] == -1) break; // escape for ... get out!!!
                textmanager.endStoryPart((int)code[1], "", "");
                break;
            case "dice":
                RollDice(code); // token을 받을것. 거기서.. 
                //Debug.Log("OPCODE[dice] : " + op + "입니다 " + code[idx++] + " " + code[idx++] + " " + code[idx++] + " " + code[idx++]);
                break;
            case "mov": //call region. area. moment
                Region(code[idx++].ToString(), (int)code[idx++], (int)code[idx++]);   //Region(code[op][i++].ToString(), (int)code[op][i++], (int)code[op][i++]);
                break;
            case "dia": //get str, show 
                if (idx == 1) // effect : [ "dia", [ [1], [2] ] ]   // [1]은 2차원 배열이라 할 수 있겠지..
                    foreach (JToken jdes in code[1])
                        File.AppendAllText(mainroute, Setstring(jdes.ToString()) + '\n');
                else
                    foreach (JToken jdes in code)
                        File.AppendAllText(mainroute, Setstring(jdes.ToString()) + '\n');  //System.IO.
                break;
            case "npc":
                NpcCall(code[idx++].ToString(), code[idx++].ToString(), code[idx++].ToString());
                break;
            case "btl":
                Battle(code[idx++].ToString(), code[idx++].ToString(), (int)code[idx++], (int)code[idx++]);
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
                dice_type = (int)list[2];
                dice_up = (int)list[3];
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
        string str = MakeJson(Application.dataPath + @"\Resource\Text\Field\Region.json");
        JObject jroot = JObject.Parse(str);
        JToken jregion = jroot["Region"];
        //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class로 문자열..?
        // connect path = jregion["name"].ToString

        //if (jregion["name"].ToString() == spot)
        foreach (JToken des in jregion["Type"][chap]["description"]) //임시 for
            File.AppendAllText(mainroute, Setstring(des.ToString()) + '\n');
        //Debug.Log(des);

        return;
    }

    private void NpcCall(string name, string situ, string num)
    {
        //해당 npc를 찾아서, situ를 맞추고, num에 있는 dia나 다른 것을 행함
        string str = MakeJson(Application.dataPath + @"\Resource\Text\Npc\Npc.json");
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

    private void Battle(string type, string name, int num, int situ)
    {
        File.AppendAllText(mainroute, "#battle " + name + num + '\n');
        //종류 : 몬스터, 인간형 등등?
        //이름, 숫자. 그리고 시츄는 발각, 기습, 상태이상 등?
        Debug.Log("JSON[monster] : " + name + "이 " + num + "마리 나왔습니다.");
        return;
    }

        /*
         * sc_key에는 관측이 됨. key에서도 아마 필요할거같아보임
         *  effect : { .., ["dice", "sen", 1, 1, 2] 형식 받고
         *  dice : { "1", "2" : ... 로 따로 취급하자. dice는 좀 많이 복잡할거같네..
         */

        //System.IO.File.AppendAllText(mainroute, "\n"); // + final null line
    

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

    private string Setstring(string raw_string)
    {
        string[] divied = raw_string.Split('"');
        return ReplaceS(divied[1]);
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