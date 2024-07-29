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

    //���� ����. ĳ���� ��ũ��Ʈ �ʿ�
    private string player = "�÷��̾�";
    private string space = "��";

    //for #player & path + "\Scenario... \Npc...
    //private Dictionary<string, string> convert_hash = new Dictionary<string, string>(); // .Add(key, Value);

    //��� ����
    private string path = @"\Resource\Text\";  //this position is moved so... where..?
    private string mainroute, keyroute;
    private string curmain;

    JArray key_jarray, sc_key_jarray;
    JObject key_jroot;

    [SerializeField] public JObject jbase;

    private void Start()
    {
        //���� .txt Ű .json
        mainroute = Application.dataPath + path + "main.txt";
        keyroute = Application.dataPath + path + "main.json";

        battlemanager = FindObjectOfType<BattleManager>();
    }

    public int ReadScenarioParts(int move, string jmain)
    {
        // �ó����� �̸����� ����. (������(@Scenario))\���ϸ�\�ó�������
        curmain = jmain;
        string scnroute = Application.dataPath + path + @"Scenario\" + curmain + ".json";
        string str = convertJson.MakeJson(scnroute);
        string key_str = convertJson.MakeJson(keyroute);

        int op_num = 0;

        // read �� �κ� �ʱ�ȭ
        File.WriteAllText(keyroute, "{ \"key\" : [{}], \"sc_key\" : [{}] }");    // �ʱ�ȭ
        File.WriteAllText(mainroute, "");                                       // main reset

        key_jarray = new JArray(); 
        sc_key_jarray = new JArray();

        //Native Object ���
        jbase = JObject.Parse(str);
        key_jroot = JObject.Parse(key_str);

        do
        {
            //Debug.Log("CHANGER : " + move);
            JToken jnow = jbase["scenario"][move];
            //int lnd_cmd = 0; // non ��ɾ�
            foreach (JToken jscript in jnow["script"])
            {
                JToken jlist = jnow["scriptlist"][op_num++];
                //lnd_cmd = GetOpcode(jlist.ToString(), jscript[jlist.ToString()], 0);
                GetOpcode(jlist.ToString(), jscript[jlist.ToString()], 0);
                CheckKeys("::opcode here", jscript);

                //if (lnd_cmd == 1) return (int)jscript[jlist.ToString()][0]; //���� ����� �ʿ��� ���, ���� (�ϴ� ������....... ���� �����ĸ� rpl�����̶� ������ �ʿ�
            }

        } while (false);

        //Debug.Log("readParts end one");
        return 0;
    }

    //�ƿ� ���� ��ũ��Ʈ�� ������ ��. ������ ���� �Լ��� ���� ��ũ��Ʈ��
    public int GetOpcode(string op, JToken code, int idx)  // idx == 1 -> effect code
    {
        Debug.Log("GETcode : " + op + " & "+ code);
        switch (op)
        {
            
            case "jmp": //�ٸ� �ó������� �̵�  // jmp/0/plain/Plain | 0/plain/Plain
                Debug.Log("REQUEST[event] : occuring?");            //if ((int)code[1] != 0) textmanager.EndStoryPart((int)code[1], "", "");  //move cur scenario
                textmanager.ClearText();
                ReadScenarioParts((int)code[++idx], code[++idx].ToString());
                break;
            case "rpl": //���� json(�ó�����/����)������ �̵�
                if ((int)code[idx] == -1) break;                            // escape for ... get out!!!
                textmanager.ClearText();
                ReadScenarioParts((int)code[idx], curmain);
                //textmanager.EndStoryPart((int)code[1], "", "");
                break;
            //case "dice": //dice roll 
            //    RollDice(code); // token�� ������. �ű⼭.. 
                //Debug.Log("OPCODE[dice] : " + op + "�Դϴ� " + code[idx++] + " " + code[idx++] + " " + code[idx++] + " " + code[idx++]);
            //    break;
            case "mov": //call region. area. moment //������ ��ɾ�. 2024-07-09. ������Ұ�.
                Region(code[idx++].ToString(), (int)code[idx++], (int)code[idx++]);   //Region(code[op][i++].ToString(), (int)code[op][i++], (int)code[op][i++]);
                break;
            case "dia": //�ó����� show
                if (idx == 1) // effect : [ "dia", [ [1], [2] ] ]   // [1]�� 2���� �迭�̶� �� �� �ְ���..
                    foreach (JToken jdes in code[1])
                        File.AppendAllText(mainroute, Setstring(jdes.ToString()) + '\n');
                else
                    foreach (JToken jdes in code)
                        File.AppendAllText(mainroute, Setstring(jdes.ToString()) + '\n');  //System.IO.
                break;
            case "npc": //���븸 ���� �ڵ�..
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
            File.AppendAllText(mainroute, k);
        File.WriteAllText(keyroute, key_jroot.ToString());
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

        int roll_result = Random.Range(0, dice_type);
        Debug.Log("DICE_ROLL : " + roll_result);

        // ���� ���ġ.(player ���� Ŭ������ ���� �޾ƾ��ҵ� �ѵ�...
        //if ((int)player[stat] >= dice_up) result++;
        string result = (roll_result >= dice_type / 2) ? "succ" : "fail";
        Debug.Log("DICE_RESULT : " + result);

        foreach (JToken code in info[result])
        {
            GetOpcode(code[0].ToString(), code, 1);
        }

    }

    private void Region(string spot, int chap, int detail)     //������ get
    {
        string str = convertJson.MakeJson(Application.dataPath + @"\Resource\Text\Field\Region.json");
        JObject jroot = JObject.Parse(str);
        JToken jregion = jroot["Region"];                                                                           //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class�� ���ڿ�..?

        //if (jregion["name"].ToString() == spot)
        foreach (JToken des in jregion["Type"][chap]["description"]) //�ӽ� for
            File.AppendAllText(mainroute, Setstring(des.ToString()) + '\n');
        //Debug.Log(des);

        return;
    }

    private void NpcCall(string name, string situ, string num)
    {
        //�ش� npc�� ã�Ƽ�, situ�� ���߰�, num�� �ִ� dia�� �ٸ� ���� ����
        string str = convertJson.MakeJson(Application.dataPath + @"\Resource\Text\Npc\Npc.json");
        JObject jroot = JObject.Parse(str);
        JToken jnpc = jroot[name][situ][num];
        //convert_hash.Add("#" + name, jnpc["name"].ToString()); // ex) #edward : �������
        foreach (JToken dia in jnpc["dia"])
            File.AppendAllText(mainroute, Setstring(dia.ToString()) + '\n');   //" "�� �˾Ƽ� �߰��ϴ���.. ������,

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
        //���� : ����, �ΰ��� ���?
        //�̸�, ����. �׸��� ����� �߰�, ���, �����̻� ��?
        Debug.Log("JSON[monster] : " + jbattle + "�� " + root + "�߻�.");
        battlemanager.BattlePreset(jbattle, root, num, situ); //"goblin", "forest_goblin", 1, 0]},

        File.AppendAllText(mainroute, "#btl\n");
        //���⼭ ������ �־�� ����.

        return;
    }

        /*
         * sc_key���� ������ ��. key������ �Ƹ� �ʿ��ҰŰ��ƺ���
         *  effect : { .., ["dice", "sen", 1, 1, 2] ���� �ް�
         *  dice : { "1", "2" : ... �� ���� �������. dice�� �� ���� �����ҰŰ���..
         */

        //System.IO.File.AppendAllText(mainroute, "\n"); // + final null line
    

    private string Setstring(string raw_string)
    {
        string[] divied = raw_string.Split('"');
        return ReplaceS(divied[1]);
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