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
    private string player = "�÷��̾�";
    private string space = "��";

    public void Organize()
    {
        string mainroute = Application.dataPath + @"\Resource\Text\main.txt";
        string jpath = Application.dataPath + @"\Resource\Text\Scenario\scenario.json";  //\Resource\Text\Scenario\tutorial01.txt
        string str = MakeJson(jpath);

        //Native Object ���
        JObject jroot = JObject.Parse(str);

        //condition Ȯ������. (���)

        JToken jToken = jroot["Tutorial"]["scenario"];      //tutorial��� str1�� �ʿ�(���� �ó��������� ���� ��)
        int move = 0;

        do  //good algorithm?
        {
            //now -> start�ϰ� ���� ���(����ȭ)
            //later -> �����κ� ����ϰ�, null�ǰų� ���� ��ô ������ new ���
            JToken jnow = jToken[move];

            //check state
            if (move != (int)jnow["state"]) Debug.Log(move + " " + jnow["state"] + " ����ġ");

            //read selection    -> how?
            /*
            foreach(JToken jsel in jnow["selection"])
            {
                //selection��... ��� �����ϳ�.?
            }
            */

            //description
            if (jnow["description"] != null)
                foreach (JToken jdes in jnow["description"])
                {
                    string nowline = Setstring(jdes.ToString());
                    //#ó��   don' like. need to change
                    if (nowline.Contains("#"))
                    {
                        nowline = nowline.Replace("#player", player);
                        nowline = nowline.Replace("#space", space);
                        nowline = nowline.Replace("#monster", "goblin");
                    }

                    System.IO.File.AppendAllText(mainroute, nowline + '\n');
                }
            
            //move & call
            move = (int)jnow["move"]; //move�� �ټ��� ����? ...info classify
            if (jnow["call"].ToString() != "NULL")
            {
                JToken jcall = jnow["call"];
                if (jcall[0].ToString() == "Region")
                    Region(jcall[1].ToString(), ((int)jcall[2]), ((int)jcall[3]));
                else if (jcall[0].ToString() == "Battle")
                    Battle(jcall[1].ToString(), (int)jcall[2]);
                else if (jcall[0].ToString() == "Town")
                    Debug.Log(jcall[1].ToString() + ((int)jcall[2]));  //J_town �Լ�

            }

        } while (move != -1);

        void Region(string spot, int chap, int detail)     //�������� ��´�.
        {
            string jpath = Application.dataPath + @"\Resource\Text\Field\Region.json";
            string str = MakeJson(jpath);
            JObject jroot = JObject.Parse(str);
            JToken jregion = jroot["Region"];
            //Debug.Log(str);

            if (jregion["name"].ToString() == spot)
                foreach (JToken des in jregion["Type"][chap]["description"]) //�ӽ� for
                {
                    System.IO.File.AppendAllText(mainroute, Setstring(des.ToString()) + '\n');
                    //Debug.Log(des);  
                }

        }

        void Battle(string monster, int num)
        {
            //Debug.Log(monster + "�� " + num + "���� ���Խ��ϴ�.");
        }


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

    private string Setstring(string old_string)
    {
        string[] divied = old_string.Split('"');
        return divied[1];
    }

    /*
    public void organ()
    {
        string mainroute = Application.dataPath + @"\Resource\Text\main.txt";
        string selfroute = Application.dataPath + @"\Resource\Text\Scenario\tutorial01.txt";
        string[] scenario = System.IO.File.ReadAllLines(selfroute);

        //string[] tmp;
        //������. @�̶� #�� ���⼭ ����, �ٸ� ��ũ��Ʈ���� ó������, �Լ����� ó������..

        if (scenario.Length > 0)
        {
            for (int i = 0; i < scenario.Length; i++)
            {
                //#ó��
                if (scenario[i].Contains("#"))
                {
                    scenario[i] = scenario[i].Replace("#player", player);
                    scenario[i] = scenario[i].Replace("#space", space);
                    scenario[i] = scenario[i].Replace("#monster", "goblin");
                }

                if (scenario[i].Contains("@"))  //@ó��
                {
                    string[] var = scenario[i].Split(',');
                    if (var[0] == "@Region")
                    {
                        //�������� �з� �ؽ�Ʈ�� �����ͼ� 001�� 01�� Ž�� �� �߰�. �̹����� �ܼ��ϰ�
                        string desroute = Application.dataPath + @"\Resource\Text\Field\Forest.txt";
                        string[] field = System.IO.File.ReadAllLines(desroute);
                        Region(var[1], var[2], var[3]);
                        //System.IO.File.AppendAllText(mainroute, field[1] + "\n");
                    }
                    else if (var[0] == "@Battlefield")
                    {
                        //InputJson(path);
                        //JObject dbSpec = new JObject(nwe JProperty("rmlfajrl"));
                        //dpSpec.Add("Description", JArray.FromObject(users));
                        //File.WriteAllText(path, dbSpec.ToString());
                        System.IO.File.AppendAllText(mainroute, "�������ϴ�!! \n");
                    }
                    else if (var[0] == "@Selection")
                    {
                        System.IO.File.AppendAllText(mainroute, "������ \n");
                    }
                    else if (var[0] == "@Town")
                    {
                        //townȣ��
                        System.IO.File.AppendAllText(mainroute, "�����̴�!! \n");
                    }
                    continue;
                }
                if (i >= scenario.Length) break;
                //�ؽ�Ʈ 
                System.IO.File.AppendAllText(mainroute, scenario[i] + '\n');
            }
        }

        //foreach(string line in scenario)


        void Region(string spot, string chap, string detail)     //�������� ��´�.
        {
            if (spot == "forest")
            {
                string mainroute = Application.dataPath + @"\Resource\Text\main.txt";
                string route = Application.dataPath + @"\Resource\Text\Field\Forest.txt";
                string[] forest_region = System.IO.File.ReadAllLines(route);

                int numbering = int.Parse(detail);
                int line = VarManager.forest_node[numbering] + 1;


                //�� �ٺ��� END�� ���������� ���
                do
                {
                    System.IO.File.AppendAllText(mainroute, forest_region[line] + '\n');
                } while (!forest_region[++line].Contains("END"));


            }
        }

    }

    */

}