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

    //ÀÓÀÇ ¼³Á¤. Ä³¸¯ÅÍ ½ºÅ©¸³Æ® ÇÊ¿ä
    private string player = "ÇÃ·¹ÀÌ¾î";
    private string space = "½£";

    //°æ·Î ¼³Á¤
    //private string path = @"/Resource/Text/";  //this position is moved so... where..?
    private string main_route, key_route;
    private string cur_main;
    private int cur_move;

    //¹Ì¸® °¥ °æ·Î ¼³Á¤
    public string next_main;
    public int next_move;
    public string event_scenario;
    public string quest_scenario;

    JArray key_jarray, sc_key_jarray;
    JObject key_jroot;

    [SerializeField] public JObject jbase;
    (string name, float rate)[] data = { ("1", 1.0f) };

    private void Start()
    {
        //½ÇÁ¦ .txt Å° .json
        //main_route = Application.dataPath + path + "main.txt";
        main_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/main.txt";
        key_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/mainSet.json";

        battleManager = FindObjectOfType<BattleManager>();


    }

    public int NewScenarioEnter(int move, string jmain)
    {
        /*
         * ¸Å´ÏÀú°¡ »õ ½Ã³ª¸®¿À(text)¸¦ ¿ä±¸ÇÑ´Ù.
         * ÇØ´ç Áö¿ª(jmain)À¸·Î ÁøÀÔÀü, ÀÌ°÷¿¡ ¸ÕÀú ¹ß»ýÇÏ´Â Äù½ºÆ®³ª ÀÌº¥Æ®°¡ ÀÖ´ÂÁö ¼­Ä¡ÇÑ´Ù.
         * ¸¸ÀÏ ÀÖ´Ù¸é, read ¼ø¼­¸¦ µÚ¹Ù²Ù¾î ½Ç½ÃÇÑ´Ù.
         * read´Â ±ä±ÞÀÌº¥Æ®¿Í °£Á¢ÀÌº¥Æ® µÎ°¡ÁöÀÌ´Ù¸¸, ±×¿¡´ëÇÑ ±¸ÇöÀº ¾ÆÁ÷ °í¹ÎÁßÀÌ¹Ç·Î ±ä±ÞÀÌº¥Æ®¸¸ Ãë±ÞÇÑ´Ù.
         */
        Debug.Log("call enter");

        cur_main = jmain;
        cur_move = move;
        string str = Resources.Load<TextAsset>("Text/Scenario/" + jmain).ToString();
        //int op_num = 0;
        int eventcall = -1;
        //Native Object ¹æ½Ä
        jbase = JObject.Parse(str);
        JToken jnow = jbase["scenario"][move];

        //¸ðµç ÁøÀÔÀÇ ½ÃÀÛ¿¡¼­¸¸ È®ÀÎ. ÁøÇàµµÁß¿¡ ¹æÇØx
        if (move == 0)
        {
            //Äù½ºÆ® À¯¹« È®ÀÎ.
            if (jbase["condition"]["region"].ToString() != null && QuestEncounter(jbase["condition"]["region"].ToString()) == 1)
            {
                Debug.Log("move to quest" + quest_scenario);
                cur_main = quest_scenario;
                ReadScenarioParts(0, quest_scenario);
                //Movement((int)cur_typecode, cur_main, "move"); //ÇØ´ç Äù½ºÆ®°¡ ³¡³ª¸é ¾îµð·Î °¡°ÔÇÏÁÒ? - Äù½ºÆ®´Â ¸¶Áö¸·¿¡ move.Áö¿ªÀÌ ÀÖ´Â°¡?
                return 0;
            }

            //ÀÌº¥Æ® À¯¹«¿Í ¸®µù
            eventcall = EventEncounter(jbase["condition"]["region"].ToString());
            if (eventcall >= 0)
            {
                cur_main = event_scenario;
                ReadScenarioParts(0, event_scenario);
                return 0;
            } //ÀÏ´Ü 0,1 ÇÏ³ª·Î ÅüÄ¡±â.
        }

        //±âÁ¸¿¡ ¹ÞÀº ½Ã³ª¸®¿À ¸®µù
        ReadScenarioParts(move, jmain);
        return 0;
    }

    //½Ã³ª¸®¿À ¸®´õ±â . ¸®µù¸¸ ÇÏµµ·Ï ¸ðµâÈ­
    public int ReadScenarioParts(int move, string jmain)
    {
        Debug.Log(jmain + move);
        string str = Resources.Load<TextAsset>("Text/Scenario/" + jmain).ToString();
        string key_str = convertJson.MakeJson(key_route);
        int op_num = 0;

        // read ÇÒ ºÎºÐ ÃÊ±âÈ­
        File.WriteAllText(key_route, "{ \"key\" : [{}], \"sc_key\" : [{}] }");    // ÃÊ±âÈ­
        File.WriteAllText(main_route, "");                                       // main reset

        key_jarray = new JArray();
        sc_key_jarray = new JArray();

        //Native Object ¹æ½Ä
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
        string breakpoint = "#\n"; //¸ðµç ½ºÅ©¸³Æ®°¡ Áß´ÜÁ¡À» ÇÊ¿ä·Î ÇÏ´Â°¡?

        switch (script_type)
        {
            //º»¹® °ü·Ã
            case "dia": //º»¹®
                Dialog(cur_blockcode["dia"], idx);
                break;
            case "con dia": //Á¶°ÇºÎ ´ëÈ­¹®. ¿©±â¼­... ´ÙÀÌ½º ±¼¸®´Â Çü½ÄÀ¸·Î, ±×°Å ÀÌÆåÆ® µû·Î »©°í, 
                //conditionÀ» È®ÀÎÇÏ°í, ÂüÀÌ¸é dialog¸¦ Ã·°¡ÇÑ´Ù. ¾Æ´Ï¸é ±×³É ¾È ÇÑ´Ù.(È¤Àº '½ÇÆÐ'·Î±× ¶ç¿öÁØ´Ù´ø°¡)
                Debug.Log("con is" + cur_blockcode["con"]);
                Debug.Log("con dia is" + cur_blockcode["dia"]);
                Dialog(cur_blockcode["dia"], idx);
                break;


            //¼±ÅÃÁö °ü·Ã
            case "choice": //¼±ÅÃÁö key Ãß°¡
                Choice(cur_blockcode["choice"]);
                breakpoint = "#key\n";
                break;

            //Ãß°¡Ã¢ ¶ç¿ì±â - ÁÖ·Î »óÁ¡¿ë
            case "window open":
                cur_typecode = cur_blockcode["open"];
                Window(cur_typecode[0].ToString(), cur_typecode[1].ToString());
                cur_move = (int)cur_blockcode["close"];
                break;

            //ÀÌµ¿ °ü·Ã
            case "script move":
                //if ((int)code[idx] == -1) break; // ÇÊ¿äÇÑÁö ¤±?¤©
                cur_typecode = cur_blockcode["move"];
                Movement((int)cur_typecode, cur_main, "move");
                break;
            case "scene over": //json ÀÌµ¿
                cur_typecode = cur_blockcode["over"];
                Movement((int)cur_typecode[0], cur_typecode[1].ToString(), "over");
                break;

            case "wait": //ÈìÁ»¹«.. ÇÊ¿äÇÑ ±â´ÉÀº ¸ÂÁö. ±×·¯¸é ÀÌ ÄÚµå ÀÚÃ¼¸¦ ´À¸®°Ô ÇØ¾ßÇÒÅÙµ¥. ±×°Ô ±¸ÇöÀÌ ½±³ÄÀÇ ¹®Á¦. ³ªÁß¿¡ ±¸ÇöÇÒ°Í.


            //ÀÌº¥Æ® °ü·Ã
            case "battle": //¹èÆ² ¼ÒÈ¯
                cur_typecode = cur_blockcode["battle"];
                Battle(cur_typecode[0].ToString(), cur_typecode[1].ToString(), (int)cur_typecode[2], (int)cur_typecode[3]);
                break;
            case "quest": //Äù½ºÆ® ÁøÇàµµ ³»Áö´Â È¹µæ ·ù

            case "effect": //player hp³ª mp, expµîÀÇ °ªÀ» Ãß°¡, Á¦°ÅÇÏ´Â ·¹º§. + ¾ÆÀÌÅÛ, ¼­»ç È¹µæ.


            //ÀÌÆåÆ® °ü·Ã
            case "sound": //¼Ò¸®È¿°ú Àç»ý (¿É¼Ç ÀÖ)
            case "img": //»ðÈ­Ãß°¡
                break;
            //case "npc": //rpl°ú °°Àº? ´ëÈ­·Î µé¾î°¡±â..

            default:
                Debug.LogError(script_type + " is non type");
                break;

        }

        //Áß´ÜÁ¡ Ãß°¡
        File.AppendAllText(main_route, breakpoint);

        return 0;
    }

    //main.txt¿¡ textÃ·°¡
    private void Dialog(JToken logue, int idx) //idx »èÁ¦ÇÒ°Í
    {
        foreach (JToken jdes in logue)
            File.AppendAllText(main_route, Setstring(jdes.ToString()) + '\n');  //System.IO.
        return;
    }

    //´ÙÀ½ ½ºÅ©¸³Æ® ÁöÁ¤
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
        //ÀÚµ¿ÀÐ±â. ±¦­v?
        textManager.ReadPage();
        Debug.Log(type + itemsheet);
    }

    public void CloseWindow()
    {
        window.SetActive(false);
        ReadScenarioParts(cur_move, cur_main);
    }

    //key¸¦ mainSetÀ¸·Î ¼¼ÆÃ
    private void Choice(JToken selection)
    {
        key_jarray.Add(selection.ToObject<JObject>());  // jobject¸¸ add
        key_jroot["key"] = key_jarray;

        File.WriteAllText(key_route, key_jroot.ToString());
    }

    //ÀüÅõ ÁøÀÔ Áß´ÜÄÚµå »ðÀÔ
    private void Battle(string jbattle, string root, int num, int situ)
    {
        //¹èÆ² ¹Ì¸® ¼¼ÆÃ
        battleManager.BattlePreset(jbattle, root, num, situ); //ÇöÀç ¸Å°³º¯¼ö °ÅÀÇ ÀÇ¹Ì¾ø°Ô ¼¼ÆÃµÇ¾îÀÖÀ½.

        //TextManager°¡ ÀÐÀ¸¸é ¹èÆ² ÁøÀÔ
        File.AppendAllText(main_route, "#btl\n");

        return;
    }

    private int QuestEncounter(string n)
    {
        //ÇöÀç ¹ÞÀº Äù½ºÆ®¿¡¼­ ÇöÀç Áö¿ª (Á¶°Ç)°ú ºÎÇÕÇÏ´ÂÁö È®ÀÎ. 
        //ºÎÇÕÇÏ´Ù¸é Äù½ºÆ® ¼± ½ÇÇà. ¾ê´Â ÀÏºÎ ¹¦»ç°¡ ÀÖ³Ä ¹¯ÀÚ¸é.. ±Û½ê´Ù.? ³ªÁß¿¡ Äù½ºÆ® ¸¸µé¸é ÆÇº°³ª°ÚÁö.

        return 0;
    }

    private int EventEncounter(string region)
    {
        //´øÀü/¸¶À» µîÀÇ Æ¯¼ö Áö¿ªÀÏ °æ¿ì, ÀÌº¥Æ® Á¾·ù¸¦ Á¦ÇÑÇÑ´Ù.
        //Æ¯Á¤ Äù½ºÆ® Áö¿ª¿¡ ÁøÀÔ½Ã, ¹«Á¶°ÇÀûÀ¸·Î Äù½ºÆ®¸¦ ¿ì¼±ÇÑ´Ù.?
        ProbabilityCalculator probabilityCalculator = new ProbabilityCalculator();
        event_scenario = probabilityCalculator.Probability(region);

        //¾Æ¹« ÀÏµµ ÀÏ¾î³ªÁö ¾Ê¾Ò´Ù.
        if (event_scenario == "none") return -1;

        Debug.Log("here is " + region + event_scenario);
        return 0; //0.°­Á¦ ÀÌº¥Æ®(¹Ù·Î ½ÃÀÛ)
        //return 1; //1.½ÅÈ£ ÀÌº¥Æ®(ÀÏºÎ ¹¦»ç + °¥Áö ¼±ÅÃÁö)

    }

    //Æó±â ¿¹Á¤.
    /*
    // key, sc_key -> main.json to use for keywords
    private void CheckKeys(string k, JToken cur_script) // how to use k..?
    {
        k = "0";
        // key      list -> effect ÀÏ¹ÝÀûÀÎ ¼±ÅÃÁö. µû¶ó¼­ ½ºÅÈ¸¸ °ü¿©
        // sc_key   list -> action, skill -> dice -> effect ÇÃ·¹ÀÌ¾îÀÇ Á÷¾÷, ½ºÅ³¿¡ µû¶ó ´Ù¸£°Ô °ü¿©
        foreach (JProperty keys in cur_script)
        {
            if (keys.Name == "key")
            {
                key_jarray.Add(cur_script["key"].ToObject<JObject>());  // jobject¸¸ add
                key_jroot["key"] = key_jarray;
                k = "#key\n";
            }
            else if (keys.Name == "sc_key")
            {
                sc_key_jarray.Add(cur_script["sc_key"].ToObject<JObject>());
                key_jroot["sc_key"] = sc_key_jarray;                    //key_jroot["sc_key"] = cur_script["sc_key"].ToObject<JObject>();

                if (k == "#key\n") k = "#key sc_key\n";
                else k = "#sc_key\n";     // key°¡ ¾ÃÈ÷´Â °æ¿ì°¡ ÀÖ´Ù... Èì..
                

                //list selectionÀ» ¹Þ¾Æ, ¾Æ·¡ ¼±ÅÃÁöobj¸¦ Ç¥½ÃÇÑ´Ù.
                //key opcode´Â GetOpcode("key", key[select]); ·Î µû·Î Ãë±Þ
            }
            else if (keys.Name == "dia")
                k = "#\n";
        }

        if( k[0] == '#')
            File.AppendAllText(main_route, k);
        File.WriteAllText(key_route, key_jroot.ToString());
    }

    // *** diceÀÇ succ, failÀ» effect ¾È¿¡´Ù ³ÖÀ»°Í!! *** 
    private void RollDice(JToken info)
    {
        string stat;
        int dice_type = 6, dice_up = 0;
        foreach (JToken list in info["effect"])
            if (list[0].ToString() == "dice")
            {
                stat = list[1].ToString();
                dice_type = (int)list[3]; //¹æ½ÄÀÌ ´Þ¤©¶ó¿ä...
                dice_up = (int)list[2];
            }

        int roll_result = UnityEngine.Random.Range(0, dice_type);
        Debug.Log("DICE_ROLL : " + roll_result);

        // ´ëÃæ »ó½ÂÄ¡.(player °ªÀ» Å¬·¡½º¿¡ µû·Î ¹Þ¾Æ¾ßÇÒµí ÇÑµ¥...
        //if ((int)player[stat] >= dice_up) result++;
        string result = (roll_result >= dice_type / 2) ? "succ" : "fail";
        Debug.Log("DICE_RESULT : " + result);

        foreach (JToken code in info[result])
            GetOpcode(code[0].ToString(), code, 1);

    }

    private void Region(string spot, int chap, int detail)     //Áö¿ªº° get
    {
        //string str = convertJson.MakeJson(Application.dataPath + @"/Resource/Text/Field/Region.json");
        string str = Resources.Load<TextAsset>("Text/Field/Region").ToString();
        JObject jroot = JObject.Parse(str);
        JToken jregion = jroot["Region"];                                                                           //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class·Î ¹®ÀÚ¿­..?

        //if (jregion["name"].ToString() == spot)
        foreach (JToken des in jregion["Type"][chap]["description"]) //ÀÓ½Ã for
            File.AppendAllText(main_route, Setstring(des.ToString()) + '\n');
        //Debug.Log(des);

        return;
    }

    private void NpcCall(string name, string situ, string num)
    {
        //ÇØ´ç npc¸¦ Ã£¾Æ¼­, situ¸¦ ¸ÂÃß°í, num¿¡ ÀÖ´Â dia³ª ´Ù¸¥ °ÍÀ» ÇàÇÔ
        //string str = convertJson.MakeJson(Application.dataPath + @"/Resource/Text/Npc/Npc.json");
        string str = Resources.Load<TextAsset>("Text/Npc/Npc").ToString();
        JObject jroot = JObject.Parse(str);
        JToken jnpc = jroot[name][situ][num];
        //convert_hash.Add("#" + name, jnpc["name"].ToString()); // ex) #edward : ¿¡µå¿öµå
        foreach (JToken dia in jnpc["dia"])
            File.AppendAllText(main_route, Setstring(dia.ToString()) + '\n');   //" "´Â ¾Ë¾Æ¼­ Ãß°¡ÇÏ´ø°¡.. ¸»´ø°¡,

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
            sentence += divied[i].Replace('\\', '"'); // ´ëÈ­¹® »ì¸®±â

        return ReplaceS(sentence);
    }

    private string ReplaceS(string c_line)  // + DictionÀÌ¿ë, key value°ª?
    {
        if (!c_line.Contains("#")) return c_line;
        // string player = "È«±æµ¿"
        // just 'for'? or json[converter] = #'list
        c_line = c_line.Replace("#player", player);
        c_line = c_line.Replace("#space", space);
        c_line = c_line.Replace("#edward", "¿¡µå¿öµå");
        c_line = c_line.Replace("#monster", "goblin");
        return c_line;
    }

}