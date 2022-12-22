using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Textchanger
{
    //later classify -> json reading
    //이 데이터는 json에 있을것.
    private string player = "플레이어";
    private string space = "숲";

    //for #player & path + "\Scenario... \Npc...
    private Dictionary<string, string> convert_hash = new Dictionary<string, string>(); // .Add(key, Value);
        //private Dictionary<string, string> connet_path = new Dictionary<string, string>;
    private string path = @"\Resource\Text\";
    

    public void Organize()
    {
        //초기 path설정. 당장의 
        //string mainroute = Application.dataPath + @"\Resource\Text\main.txt";
        string mainroute = Application.dataPath + path + "main.txt";
        string jpath = Application.dataPath + @"\Resource\Text\Scenario\scenario.json";  //\Resource\Text\Scenario\tutorial01.txt
        string str = MakeJson(jpath);
        int move = 0;
        int i = 0;

        //Native Object 방식
        JObject jroot = JObject.Parse(str);

        //condition 확인절차. (고민)
        //어떤 시나리오 리스트를 받을지, 다른 json에 정리시키기.. (scenario selector.cs)
        JToken jbase = jroot["medium_0"];
        do
        {
            JToken jnow = jbase["scenario"][move];
            //check state       //Debug.Log(jnow["chapter"].ToString() + "\nsynopciys : " + jnow["synopciys"].ToString());
            foreach( JToken jscript in jnow["script"])
            {                
                JToken jlist = jnow["scriptlist"][i++];
                GetOpcode(jlist.ToString(), jscript[jlist.ToString()]);
            }

        } while (false);


        //get scripts ( { } per 1 ) -> "op" : ["codes", "res1", "res2" ...]
        void GetOpcode(string op, JToken code)  // "op", code = ["cond", "res1", ...]
        {
            int i = 0;
            //Debug.Log(op + code[0] + code[1]);

            // op = key, code = [ op, "cond", "res1", ...]
            //dia.key case
            if(op == "key")
            {
                op = code[i++].ToString();
            }
            //dia.sc_key case
            //Debug.Log("getcode : " + op + code[0] + code[1]);

            //scenario.script case
            switch (op)
            {
                //Debug.Log(op);
                case "dice":
                    Debug.Log(op + "입니다 " + code[i++] + " " + code[i++] + " " + code[i++] + " " + code[i++]);
                    break;
                case "mov": //call region. area. moment
                    Region(code[i++].ToString(), (int)code[i++], (int)code[i++]);   //Region(code[op][i++].ToString(), (int)code[op][i++], (int)code[op][i++]);
                    break;
                case "dia": //get str, show 
                    foreach (JToken jdes in code)
                        System.IO.File.AppendAllText(mainroute, Setstring(jdes.ToString()) + '\n');
                    break;
                case "npc":
                    NpcCall(code[i++].ToString(), code[i++].ToString(), code[i++].ToString());
                    break;
                case "btl":
                    Battle(code[i++].ToString(), code[i++].ToString(), (int)code[i++], (int)code[i++]);
                    break;
            }
            return;
        }

        void Region(string spot, int chap, int detail)     //지역별 get
        {
            string str = MakeJson(Application.dataPath + @"\Resource\Text\Field\Region.json");
            JObject jroot = JObject.Parse(str);
            JToken jregion = jroot["Region"];
            //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class로 문자열..?
            // connect path = jregion["name"].ToString

            //if (jregion["name"].ToString() == spot)
            foreach (JToken des in jregion["Type"][chap]["description"]) //임시 for
                System.IO.File.AppendAllText(mainroute, Setstring(des.ToString()) + '\n');
                //Debug.Log(des);

            return;
        }

        void NpcCall(string name, string situ, string num)
        {
            //해당 npc를 찾아서, situ를 맞추고, num에 있는 dia나 다른 것을 행함
            string str = MakeJson(Application.dataPath + @"\Resource\Text\Npc\Npc.json");
            JObject jroot = JObject.Parse(str);
            JToken jnpc = jroot[name];
            //convert_hash.Add("#" + name, jnpc["name"].ToString()); // ex) #edward : 에드워드

            //Debug.Log(jnpc["story"]);
            foreach(JToken dia in jnpc[situ][num]["dia"])
                System.IO.File.AppendAllText(mainroute, Setstring(dia.ToString()) + '\n');

            //if(jnpc[situ][num]["key"] != null) CreateSelection("key", jnpc[situ][num]);
            //if (jnpc[situ][num]["sc_key"] != null) CreateSelection("sc_key", jnpc[situ][num]);

            if (jnpc[situ][num]["key"] != null)
            {
                JToken key = jnpc[situ][num]["key"];
                foreach(JToken select in key["list"])
                {
                    //list selection을 받아, 아래 선택지obj를 표시한다.
                    //선택지obj에 <list>[] 값을 전달. 그에 따른 행동을 한다.
                    //key opcode는 GetOpcode("key", key[select]); 로 따로 취급
                }
            }

            return;
        }

        void Battle(string type, string name, int num, int situ)
        {

            //종류 : 몬스터, 인간형 등등?
            //이름, 숫자. 그리고 시츄는 발각, 기습, 상태이상 등?
            Debug.Log(name + "이 " + num + "마리 나왔습니다.");
            return;
        }

        void CreateSelection(string k, JToken selection) //before dia
        {
            //Debug.Log("1 " + k + selection);
            //if (selection[k] == null) return;
            int i = 0;
            
            while (selection[k]["list"][i] != null)
            {   
                string select = selection[k]["list"][i++].ToString();
                foreach (JToken effect in selection[k][select]["effect"])
                    Debug.Log(k + " : " + effect);
                    //GetOpcode(k, effect); (사실, key랑 sc_key는 선택지를 만드는 옵션이라, 이 함수가 아니다.
                //GetOpcode(k, selection[k][select]["effect"][0]);

            }
            
            return;
        }

        // key와 sc_key는 dia다음에만 존재한다.(지금은 22/11/16 )
        // op == key. "key" : { "list" : ... , "s1" : { "effect" : ["op", "codes", "res1" ...], ["op2", ...] }
        /*
         * 예, dia.key와 sc_key는 말이죠.. 일단 dia를 들어가요, 그리고서 있어요..
         * 아마도 script의 dia랑, npc의 dia를 "보고나서" key와 sc_key의 여부를 알아보지. if(jscipt["key"] != null), sc_key
         * 경우에 따라서는 나눌거야. 근데 이 함수에서 끝낼 수 있다면 좋을듯.
         * key받고, list에 따른 s1을 받아, 그리고 안에서 effect로 찾아. 그러면, 그 op를 다시..GetOpcode로 해? 무한루프네 하하.
         * 
         * effect중에는 dice도 있는데요... 이건 어쩌죠?
         * sc_key에는 관측이 됨. key에서도 아마 필요할거같아보임
         *  effect : { .., ["dice", "sen", 1, 1, 2] 형식 받고
         *  dice : { "1", "2" : ... 로 따로 취급하자. dice는 좀 많이 복잡할거같네..
         *  
         *  1.key의 list 하나씩을 받아 effect를 읽는다.
         *  2.effect의 list에서, ["op", ... ] op에 따라 맞는 행동을 시킨다. (아마 호출..?)
         *      나머지 파츠는.. op에 대한 code로 쓰고싶은데..
         *  3.그냥 effect는 어쩌죠..? 있나요? -> 양식은 따로 생각할것.
         */

        //System.IO.File.AppendAllText(mainroute, "\n"); // + final null line
    }

    public string MakeJson(string jpath)    //parsing안 하고 그냥 넣는거는 안되나? 굳이..?
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