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
    private string player = "플레이어";
    private string space = "숲";

    public void Organize()
    {
        string mainroute = Application.dataPath + @"\Resource\Text\main.txt";
        string jpath = Application.dataPath + @"\Resource\Text\Scenario\scenario.json";  //\Resource\Text\Scenario\tutorial01.txt
        string str = MakeJson(jpath);

        //Native Object 방식
        JObject jroot = JObject.Parse(str);

        //condition 확인절차. (고민)

        JToken jToken = jroot["Tutorial"]["scenario"];      //tutorial대신 str1이 필요(이전 시나리오에서 받은 값)
        int move = 0;

        do  //good algorithm?
        {
            //now -> start하고 전부 출력(과부화)
            //later -> 일정부분 출력하고, null되거나 일정 진척 나가면 new 출력
            JToken jnow = jToken[move];

            //check state
            if (move != (int)jnow["state"]) Debug.Log(move + " " + jnow["state"] + " 불일치");

            //read selection    -> how?
            /*
            foreach(JToken jsel in jnow["selection"])
            {
                //selection을... 어디에 저장하냐.?
            }
            */

            //description
            if (jnow["description"] != null)
                foreach (JToken jdes in jnow["description"])
                {
                    string nowline = Setstring(jdes.ToString());
                    //#처리   don' like. need to change
                    if (nowline.Contains("#"))
                    {
                        nowline = nowline.Replace("#player", player);
                        nowline = nowline.Replace("#space", space);
                        nowline = nowline.Replace("#monster", "goblin");
                    }

                    System.IO.File.AppendAllText(mainroute, nowline + '\n');
                }
            
            //move & call
            move = (int)jnow["move"]; //move가 다수인 경우는? ...info classify
            if (jnow["call"].ToString() != "NULL")
            {
                JToken jcall = jnow["call"];
                if (jcall[0].ToString() == "Region")
                    Region(jcall[1].ToString(), ((int)jcall[2]), ((int)jcall[3]));
                else if (jcall[0].ToString() == "Battle")
                    Battle(jcall[1].ToString(), (int)jcall[2]);
                else if (jcall[0].ToString() == "Town")
                    Debug.Log(jcall[1].ToString() + ((int)jcall[2]));  //J_town 함수

            }

        } while (move != -1);

        void Region(string spot, int chap, int detail)     //지역별로 얻는다.
        {
            string jpath = Application.dataPath + @"\Resource\Text\Field\Region.json";
            string str = MakeJson(jpath);
            JObject jroot = JObject.Parse(str);
            JToken jregion = jroot["Region"];
            //Debug.Log(str);

            if (jregion["name"].ToString() == spot)
                foreach (JToken des in jregion["Type"][chap]["description"]) //임시 for
                {
                    System.IO.File.AppendAllText(mainroute, Setstring(des.ToString()) + '\n');
                    //Debug.Log(des);  
                }

        }

        void Battle(string monster, int num)
        {
            //Debug.Log(monster + "이 " + num + "마리 나왔습니다.");
        }


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
        //문제점. @이랑 #을 여기서 할지, 다른 스크립트에서 처리할지, 함수에서 처리할지..

        if (scenario.Length > 0)
        {
            for (int i = 0; i < scenario.Length; i++)
            {
                //#처리
                if (scenario[i].Contains("#"))
                {
                    scenario[i] = scenario[i].Replace("#player", player);
                    scenario[i] = scenario[i].Replace("#space", space);
                    scenario[i] = scenario[i].Replace("#monster", "goblin");
                }

                if (scenario[i].Contains("@"))  //@처리
                {
                    string[] var = scenario[i].Split(',');
                    if (var[0] == "@Region")
                    {
                        //지형묘사 분류 텍스트를 가져와서 001과 01의 탐색 후 추가. 이번에는 단순하게
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
                        System.IO.File.AppendAllText(mainroute, "전투랍니다!! \n");
                    }
                    else if (var[0] == "@Selection")
                    {
                        System.IO.File.AppendAllText(mainroute, "선택지 \n");
                    }
                    else if (var[0] == "@Town")
                    {
                        //town호출
                        System.IO.File.AppendAllText(mainroute, "마을이다!! \n");
                    }
                    continue;
                }
                if (i >= scenario.Length) break;
                //텍스트 
                System.IO.File.AppendAllText(mainroute, scenario[i] + '\n');
            }
        }

        //foreach(string line in scenario)


        void Region(string spot, string chap, string detail)     //지역별로 얻는다.
        {
            if (spot == "forest")
            {
                string mainroute = Application.dataPath + @"\Resource\Text\main.txt";
                string route = Application.dataPath + @"\Resource\Text\Field\Forest.txt";
                string[] forest_region = System.IO.File.ReadAllLines(route);

                int numbering = int.Parse(detail);
                int line = VarManager.forest_node[numbering] + 1;


                //이 줄부터 END가 있을때까지 출력
                do
                {
                    System.IO.File.AppendAllText(mainroute, forest_region[line] + '\n');
                } while (!forest_region[++line].Contains("END"));


            }
        }

    }

    */

}