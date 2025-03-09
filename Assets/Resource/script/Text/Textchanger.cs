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
    ConvertJson convertJson = new ConvertJson();

    //���� ����. ĳ���� ��ũ��Ʈ �ʿ�
    private string player = "�÷��̾�";
    private string space = "��";

    //��� ����
    //private string path = @"/Resource/Text/";  //this position is moved so... where..?
    private string main_route, key_route;
    private string cur_main;

    //�̸� �� ��� ����
    public string next_main;
    public int next_move;

    JArray key_jarray, sc_key_jarray;
    JObject key_jroot;

    [SerializeField] public JObject jbase;

    private void Start()
    {
        //���� .txt Ű .json
        //main_route = Application.dataPath + path + "main.txt";
        main_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/main.txt";
        key_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/mainSet.json";

        battleManager = FindObjectOfType<BattleManager>();


    }

    //�ó����� �ؼ���.
    public int ReadScenarioParts(int move, string jmain)
    {
        // �ó����� �̸����� ����. (������(@Scenario))\���ϸ�\�ó�������
        cur_main = jmain;

        string str = Resources.Load<TextAsset>("Text/Scenario/" + cur_main).ToString();
        string key_str = convertJson.MakeJson(key_route);

        int op_num = 0;

        // read �� �κ� �ʱ�ȭ
        File.WriteAllText(key_route, "{ \"key\" : [{}], \"sc_key\" : [{}] }");    // �ʱ�ȭ
        File.WriteAllText(main_route, "");                                       // main reset

        key_jarray = new JArray(); 
        sc_key_jarray = new JArray();

        //Native Object ���
        jbase = JObject.Parse(str);
        key_jroot = JObject.Parse(key_str);

        JToken jnow = jbase["scenario"][move];

        //script�� ���󰡱�
        foreach (JToken jscript in jnow["script"])
        {
            GetOpcode(jscript["type"].ToString(), jscript, op_num);
        }

        return 0;
    }

    public int GetOpcode(string script_type, JToken cur_blockcode, int idx)  // idx == 1 -> effect code
    {
        JToken cur_typecode = cur_blockcode;
        string breakpoint = "#\n"; //��� ��ũ��Ʈ�� �ߴ����� �ʿ�� �ϴ°�?

        switch (script_type)
        {
            //���� ����
            case "dia": //����
                Dialog(cur_blockcode["dia"], idx);
                break;
            case "con dia": //���Ǻ� ��ȭ��. ���⼭... ���̽� ������ ��������, �װ� ����Ʈ ���� ����, 
                //condition�� Ȯ���ϰ�, ���̸� dialog�� ÷���Ѵ�. �ƴϸ� �׳� �� �Ѵ�.(Ȥ�� '����'�α� ����شٴ���)
                Debug.Log("con is" + cur_blockcode["con"]);
                Debug.Log("con dia is" + cur_blockcode["dia"]);
                Dialog(cur_blockcode["dia"], idx);
                break;


            //������ ����
            case "choice": //������ key �߰�
                Choice(cur_blockcode["choice"]);
                breakpoint = "#key\n";
                break;


            //�̵� ����
            case "script move": 
                //if ((int)code[idx] == -1) break; // �ʿ����� ��?��
                cur_typecode = cur_blockcode["move"];
                Movement((int)cur_typecode, cur_main, "move");                
                break;
            case "scene over": //json �̵�
                cur_typecode = cur_blockcode["over"];
                Movement((int)cur_typecode[0], cur_typecode[1].ToString(), "over");
                break;

            case "wait": //������.. �ʿ��� ����� ����. �׷��� �� �ڵ� ��ü�� ������ �ؾ����ٵ�. �װ� ������ ������ ����. ���߿� �����Ұ�.


            //�̺�Ʈ ����
            case "battle": //��Ʋ ��ȯ
                cur_typecode = cur_blockcode["battle"];
                Battle(cur_typecode[0].ToString(), cur_typecode[1].ToString(), (int)cur_typecode[2], (int)cur_typecode[3]);
                break;
            case "quest": //����Ʈ ���൵ ������ ȹ�� ��

            case "effect": //player hp�� mp, exp���� ���� �߰�, �����ϴ� ����. + ������, ���� ȹ��.


            //����Ʈ ����
            case "sound": //�Ҹ�ȿ�� ��� (�ɼ� ��)
            case "img": //��ȭ�߰�
                break;
            //case "npc": //rpl�� ����? ��ȭ�� ����..

            default:
                Debug.LogError(script_type + " is non type");
                break;

        }

        //�ߴ��� �߰�
        File.AppendAllText(main_route, breakpoint);

        return 0;
    }

    //main.txt�� text÷��
    private void Dialog(JToken logue, int idx) //idx �����Ұ�
    {
        foreach (JToken jdes in logue)
            File.AppendAllText(main_route, Setstring(jdes.ToString()) + '\n');  //System.IO.
        return;
    }
    
    //���� ��ũ��Ʈ ����
    private void Movement(int move, string main, string sign)
    {
        next_move = move;
        next_main = main;
        File.AppendAllText(main_route, '#' + sign + '\n');
    }

    //key�� mainSet���� ����
    private void Choice(JToken selection)
    {
        key_jarray.Add(selection.ToObject<JObject>());  // jobject�� add
        key_jroot["key"] = key_jarray;

        File.WriteAllText(key_route, key_jroot.ToString());
    }

    //���� ���� �ߴ��ڵ� ����
    private void Battle(string jbattle, string root, int num, int situ)
    {
        //��Ʋ �̸� ����
        battleManager.BattlePreset(jbattle, root, num, situ); //���� �Ű����� ���� �ǹ̾��� ���õǾ�����.

        //TextManager�� ������ ��Ʋ ����
        File.AppendAllText(main_route, "#btl\n");

        return;
    }

    //��� ����.
    /*
    // key, sc_key -> main.json to use for keywords
    private void CheckKeys(string k, JToken cur_script) // how to use k..?
    {
        k = "0";
        // key      list -> effect �Ϲ����� ������. ���� ���ȸ� ����
        // sc_key   list -> action, skill -> dice -> effect �÷��̾��� ����, ��ų�� ���� �ٸ��� ����
        foreach (JProperty keys in cur_script)
        {
            if (keys.Name == "key")
            {
                key_jarray.Add(cur_script["key"].ToObject<JObject>());  // jobject�� add
                key_jroot["key"] = key_jarray;
                k = "#key\n";
            }
            else if (keys.Name == "sc_key")
            {
                sc_key_jarray.Add(cur_script["sc_key"].ToObject<JObject>());
                key_jroot["sc_key"] = sc_key_jarray;                    //key_jroot["sc_key"] = cur_script["sc_key"].ToObject<JObject>();

                if (k == "#key\n") k = "#key sc_key\n";
                else k = "#sc_key\n";     // key�� ������ ��찡 �ִ�... ��..
                

                //list selection�� �޾�, �Ʒ� ������obj�� ǥ���Ѵ�.
                //key opcode�� GetOpcode("key", key[select]); �� ���� ���
            }
            else if (keys.Name == "dia")
                k = "#\n";
        }

        if( k[0] == '#')
            File.AppendAllText(main_route, k);
        File.WriteAllText(key_route, key_jroot.ToString());
    }

    // *** dice�� succ, fail�� effect �ȿ��� ������!! *** 
    private void RollDice(JToken info)
    {
        string stat;
        int dice_type = 6, dice_up = 0;
        foreach (JToken list in info["effect"])
            if (list[0].ToString() == "dice")
            {
                stat = list[1].ToString();
                dice_type = (int)list[3]; //����� �ޤ����...
                dice_up = (int)list[2];
            }

        int roll_result = UnityEngine.Random.Range(0, dice_type);
        Debug.Log("DICE_ROLL : " + roll_result);

        // ���� ���ġ.(player ���� Ŭ������ ���� �޾ƾ��ҵ� �ѵ�...
        //if ((int)player[stat] >= dice_up) result++;
        string result = (roll_result >= dice_type / 2) ? "succ" : "fail";
        Debug.Log("DICE_RESULT : " + result);

        foreach (JToken code in info[result])
            GetOpcode(code[0].ToString(), code, 1);

    }

    private void Region(string spot, int chap, int detail)     //������ get
    {
        //string str = convertJson.MakeJson(Application.dataPath + @"/Resource/Text/Field/Region.json");
        string str = Resources.Load<TextAsset>("Text/Field/Region").ToString();
        JObject jroot = JObject.Parse(str);
        JToken jregion = jroot["Region"];                                                                           //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class�� ���ڿ�..?

        //if (jregion["name"].ToString() == spot)
        foreach (JToken des in jregion["Type"][chap]["description"]) //�ӽ� for
            File.AppendAllText(main_route, Setstring(des.ToString()) + '\n');
        //Debug.Log(des);

        return;
    }

    private void NpcCall(string name, string situ, string num)
    {
        //�ش� npc�� ã�Ƽ�, situ�� ���߰�, num�� �ִ� dia�� �ٸ� ���� ����
        //string str = convertJson.MakeJson(Application.dataPath + @"/Resource/Text/Npc/Npc.json");
        string str = Resources.Load<TextAsset>("Text/Npc/Npc").ToString();
        JObject jroot = JObject.Parse(str);
        JToken jnpc = jroot[name][situ][num];
        //convert_hash.Add("#" + name, jnpc["name"].ToString()); // ex) #edward : �������
        foreach (JToken dia in jnpc["dia"])
            File.AppendAllText(main_route, Setstring(dia.ToString()) + '\n');   //" "�� �˾Ƽ� �߰��ϴ���.. ������,

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
        
        for(int i = 1; i < divied.GetLength(0) - 1; i++)
            sentence += divied[i].Replace('\\', '"'); // ��ȭ�� �츮��

        return ReplaceS(sentence);
    }

    private string ReplaceS(string c_line)  // + Diction�̿�, key value��?
    {
        if (!c_line.Contains("#")) return c_line;
        // string player = "ȫ�浿"
        // just 'for'? or json[converter] = #'list
        c_line = c_line.Replace("#player", player);
        c_line = c_line.Replace("#space", space);
        c_line = c_line.Replace("#edward", "�������");
        c_line = c_line.Replace("#monster", "goblin");
        return c_line;
    }

}