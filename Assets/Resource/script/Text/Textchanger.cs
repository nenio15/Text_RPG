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
    //�� �����ʹ� json�� ������.
    private string player = "�÷��̾�";
    private string space = "��";

    //for #player & path + "\Scenario... \Npc...
    private Dictionary<string, string> convert_hash = new Dictionary<string, string>(); // .Add(key, Value);
        //private Dictionary<string, string> connet_path = new Dictionary<string, string>;
    private string path = @"\Resource\Text\";
    

    public void Organize()
    {
        //�ʱ� path����. ������ 
        //string mainroute = Application.dataPath + @"\Resource\Text\main.txt";
        string mainroute = Application.dataPath + path + "main.txt";
        string jpath = Application.dataPath + @"\Resource\Text\Scenario\scenario.json";  //\Resource\Text\Scenario\tutorial01.txt
        string str = MakeJson(jpath);
        int move = 0;
        int i = 0;

        //Native Object ���
        JObject jroot = JObject.Parse(str);

        //condition Ȯ������. (���)
        //� �ó����� ����Ʈ�� ������, �ٸ� json�� ������Ű��.. (scenario selector.cs)
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
                    Debug.Log(op + "�Դϴ� " + code[i++] + " " + code[i++] + " " + code[i++] + " " + code[i++]);
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

        void Region(string spot, int chap, int detail)     //������ get
        {
            string str = MakeJson(Application.dataPath + @"\Resource\Text\Field\Region.json");
            JObject jroot = JObject.Parse(str);
            JToken jregion = jroot["Region"];
            //jroot["Forest", "sky", "cave", "beach", "sea", "city"] -> enum class�� ���ڿ�..?
            // connect path = jregion["name"].ToString

            //if (jregion["name"].ToString() == spot)
            foreach (JToken des in jregion["Type"][chap]["description"]) //�ӽ� for
                System.IO.File.AppendAllText(mainroute, Setstring(des.ToString()) + '\n');
                //Debug.Log(des);

            return;
        }

        void NpcCall(string name, string situ, string num)
        {
            //�ش� npc�� ã�Ƽ�, situ�� ���߰�, num�� �ִ� dia�� �ٸ� ���� ����
            string str = MakeJson(Application.dataPath + @"\Resource\Text\Npc\Npc.json");
            JObject jroot = JObject.Parse(str);
            JToken jnpc = jroot[name];
            //convert_hash.Add("#" + name, jnpc["name"].ToString()); // ex) #edward : �������

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
                    //list selection�� �޾�, �Ʒ� ������obj�� ǥ���Ѵ�.
                    //������obj�� <list>[] ���� ����. �׿� ���� �ൿ�� �Ѵ�.
                    //key opcode�� GetOpcode("key", key[select]); �� ���� ���
                }
            }

            return;
        }

        void Battle(string type, string name, int num, int situ)
        {

            //���� : ����, �ΰ��� ���?
            //�̸�, ����. �׸��� ����� �߰�, ���, �����̻� ��?
            Debug.Log(name + "�� " + num + "���� ���Խ��ϴ�.");
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
                    //GetOpcode(k, effect); (���, key�� sc_key�� �������� ����� �ɼ��̶�, �� �Լ��� �ƴϴ�.
                //GetOpcode(k, selection[k][select]["effect"][0]);

            }
            
            return;
        }

        // key�� sc_key�� dia�������� �����Ѵ�.(������ 22/11/16 )
        // op == key. "key" : { "list" : ... , "s1" : { "effect" : ["op", "codes", "res1" ...], ["op2", ...] }
        /*
         * ��, dia.key�� sc_key�� ������.. �ϴ� dia�� ����, �׸��� �־��..
         * �Ƹ��� script�� dia��, npc�� dia�� "������" key�� sc_key�� ���θ� �˾ƺ���. if(jscipt["key"] != null), sc_key
         * ��쿡 ���󼭴� �����ž�. �ٵ� �� �Լ����� ���� �� �ִٸ� ������.
         * key�ް�, list�� ���� s1�� �޾�, �׸��� �ȿ��� effect�� ã��. �׷���, �� op�� �ٽ�..GetOpcode�� ��? ���ѷ����� ����.
         * 
         * effect�߿��� dice�� �ִµ���... �̰� ��¼��?
         * sc_key���� ������ ��. key������ �Ƹ� �ʿ��ҰŰ��ƺ���
         *  effect : { .., ["dice", "sen", 1, 1, 2] ���� �ް�
         *  dice : { "1", "2" : ... �� ���� �������. dice�� �� ���� �����ҰŰ���..
         *  
         *  1.key�� list �ϳ����� �޾� effect�� �д´�.
         *  2.effect�� list����, ["op", ... ] op�� ���� �´� �ൿ�� ��Ų��. (�Ƹ� ȣ��..?)
         *      ������ ������.. op�� ���� code�� ���������..
         *  3.�׳� effect�� ��¼��..? �ֳ���? -> ����� ���� �����Ұ�.
         */

        //System.IO.File.AppendAllText(mainroute, "\n"); // + final null line
    }

    public string MakeJson(string jpath)    //parsing�� �ϰ� �׳� �ִ°Ŵ� �ȵǳ�? ����..?
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