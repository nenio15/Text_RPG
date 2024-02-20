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

    //���� ����. ĳ���� ��ũ��Ʈ �ʿ�
    private string player = "�÷��̾�";
    private string space = "��";

    //for #player & path + "\Scenario... \Npc...
    private Dictionary<string, string> convert_hash = new Dictionary<string, string>(); // .Add(key, Value);
        //private Dictionary<string, string> connet_path = new Dictionary<string, string>;
    private string path = @"\Resource\Text\";  //this position is moved so... where..?
    private string mainroute, keyroute;

    JArray key_jarray, sc_key_jarray;
    JObject key_jroot;

    [SerializeField] public JToken jbase; // public�̾�� ���� �Ǵ� ����?

    public int readScenarioParts(int move, string jmain, string jsub)
    {
        //�ʱ� path����. �ؽ�Ʈ ��ü�� �������� .txt, ���� �ڷ��� .json
        mainroute = Application.dataPath + path + "main.txt"; 
        keyroute = Application.dataPath + path + "main.json";
        Debug.Log(move + "���� " + jmain + "�׸��� " + jsub);

        // �ó����� �̸����� ����. (������(@Scenario))\���ϸ�\�ó�������
        string scnroute = Application.dataPath + path + @"Scenario\" + jmain + ".json";  //\Resource\Text\Scenario\scenarion.json
        string str = MakeJson(scnroute);
        string key_str = MakeJson(keyroute);

        // read �� �κ� �ʱ�ȭ
        File.WriteAllText(keyroute, "{ \"key\" : [{}], \"sc_key\" : [{}] }");    // �ʱ�ȭ
        File.WriteAllText(mainroute, "");                                       // main reset
        
        int op_num = 0;
        
        key_jarray = new JArray(); 
        sc_key_jarray = new JArray();

        //Native Object ���
        JObject jroot = JObject.Parse(str);
        key_jroot = JObject.Parse(key_str);

        //condition Ȯ������. (���)
        //� �ó����� ����Ʈ�� ������, �ٸ� json�� ������Ű��.. (scenario selector.cs)
        jbase = jroot[jsub]; //�ش� �ó�����

        do
        {
            Debug.Log("CHANGER : " + move);
            JToken jnow = jbase["scenario"][move];
            int lnd_cmd = 0; // non ��ɾ�
            //check state       //Debug.Log(jnow["chapter"].ToString() + "\nsynopciys : " + jnow["synopciys"].ToString());
            foreach (JToken jscript in jnow["script"])
            {
                JToken jlist = jnow["scriptlist"][op_num++];
                lnd_cmd = GetOpcode(jlist.ToString(), jscript[jlist.ToString()], 0);
                CheckKeys("::opcode here", jscript);

                if (lnd_cmd == 1) return (int)jscript[jlist.ToString()][0]; //���� ����� �ʿ��� ���, ���� (�ϴ� ������....... ���� �����ĸ� rpl�����̶� ������ �ʿ�
                //�������� �䱸�ϴ� �ٴ�... ��ũ��Ʈ�� ���Ӱ��谡 �´°�?
                //������� ���� ������ �䱸�ϴ� ������ �� �� ���°�. �׷��� �� ��ũ��Ʈ�� ������ �ؾ���.
                //�������� ����Ĵ�... �𵨰� �� + ��Ʈ�ѷ��� ������ ��� ����. ������ ������. �׷���... ��
            }

        } while (false); //�׳� �׾��. ���߿� ���� �� ���� �־


        Debug.Log("readParts end one");
        return 0;
    }


    //2024-01-04 | opocde�� ���� ���� �ϳ��� �Լ��� �ϳ��� �����.

    //mainroute�� �ʱ�ȭ ��ġ �� public�� �Ұ���..?
    //get scripts ( { } per 1 ) -> "op" : ["codes", "res1", "res2" ...]
    //�ƿ� ���� ��ũ��Ʈ�� ������ ��
    public int GetOpcode(string op, JToken code, int idx)  // idx == 1 -> effect code
    {
        //Debug.Log("GETcode : " + op + " & "+ code);
        switch (op)
        {
            
            case "jmp": //�ٸ� �ó�����, ��ũ��Ʈ�� �̵� (�׷��Ƿ� if�� ������ �� ����... 2024-01-29
                Debug.Log("REQUEST[event] : occuring?");

                //if ((int)code[1] != 0) textmanager.endStoryPart((int)code[1], "", "");  //move cur scenario
                textmanager.endStoryPart(0, code[2].ToString(), code[3].ToString()); //move another scenario
                break;
            case "rpl": //���� �ó�����/���������� �̵�
                if ((int)code[idx] == -1) break; // escape for ... get out!!!
                textmanager.endStoryPart((int)code[1], "", "");
                break;
            case "dice":
                RollDice(code); // token�� ������. �ű⼭.. 
                //Debug.Log("OPCODE[dice] : " + op + "�Դϴ� " + code[idx++] + " " + code[idx++] + " " + code[idx++] + " " + code[idx++]);
                break;
            case "mov": //call region. area. moment
                Region(code[idx++].ToString(), (int)code[idx++], (int)code[idx++]);   //Region(code[op][i++].ToString(), (int)code[op][i++], (int)code[op][i++]);
                break;
            case "dia": //get str, show 
                if (idx == 1) // effect : [ "dia", [ [1], [2] ] ]   // [1]�� 2���� �迭�̶� �� �� �ְ���..
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
                dice_type = (int)list[2];
                dice_up = (int)list[3];
            }

        int roll_result = Random.Range(0, dice_type);
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
        string str = MakeJson(Application.dataPath + @"\Resource\Text\Field\Region.json");
        JObject jroot = JObject.Parse(str);
        JToken jregion = jroot["Region"];
        //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class�� ���ڿ�..?
        // connect path = jregion["name"].ToString

        //if (jregion["name"].ToString() == spot)
        foreach (JToken des in jregion["Type"][chap]["description"]) //�ӽ� for
            File.AppendAllText(mainroute, Setstring(des.ToString()) + '\n');
        //Debug.Log(des);

        return;
    }

    private void NpcCall(string name, string situ, string num)
    {
        //�ش� npc�� ã�Ƽ�, situ�� ���߰�, num�� �ִ� dia�� �ٸ� ���� ����
        string str = MakeJson(Application.dataPath + @"\Resource\Text\Npc\Npc.json");
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

    private void Battle(string type, string name, int num, int situ)
    {
        File.AppendAllText(mainroute, "#battle " + name + num + '\n');
        //���� : ����, �ΰ��� ���?
        //�̸�, ����. �׸��� ����� �߰�, ���, �����̻� ��?
        Debug.Log("JSON[monster] : " + name + "�� " + num + "���� ���Խ��ϴ�.");
        return;
    }

        /*
         * sc_key���� ������ ��. key������ �Ƹ� �ʿ��ҰŰ��ƺ���
         *  effect : { .., ["dice", "sen", 1, 1, 2] ���� �ް�
         *  dice : { "1", "2" : ... �� ���� �������. dice�� �� ���� �����ҰŰ���..
         */

        //System.IO.File.AppendAllText(mainroute, "\n"); // + final null line
    

    private string MakeJson(string jpath)    //parsing�� �ϰ� �׳� �ִ°Ŵ� �ȵǳ�? ����..?
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