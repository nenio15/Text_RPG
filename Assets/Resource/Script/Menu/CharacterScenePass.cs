using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;
using System.Drawing;

public class CharacterScenePass : MonoBehaviour
{
    public TextMeshProUGUI Char;
    public TextMeshProUGUI label;
    private string route;
    private string cur_id;
    private string decision_id = "";
    private ConvertJson convertJson = new ConvertJson();

    //ĳ���� ������ ���� �÷��� ��ư��,
    //ĳ���� ������ ���� �÷��� ������ ������..
    //�ش� ��ũ��Ʈ�� �ϴ�, ĳ���� ������ ����.

    public void IdSelect(string id)
    {
        decision_id = id;
    }

    public void CharSelect()
    {
        //id�� scenario Ȯ�� �� ����
        if (decision_id == "") return;
        route = UnityEngine.Application.persistentDataPath;
        string world = convertJson.MakeJson(route + "/" + decision_id + "/Info/World.json");
        JObject jworld = JObject.Parse(world);
        if(jworld["Scenario"] == null) { Debug.LogError("scenario�� �������� �ʾҽ��ϴ�."); return; }
        
        PlayerPrefs.SetString("Cur_scenario", jworld["Scenario"].ToString());
        PlayerPrefs.SetString("Char_route", decision_id);
        SceneManager.LoadScene("MainScene");
    }

    //�ܺο��� ���� �� �ٷ� �÷��� - ������ 
    public void CreateAndPlay()
    {
        string id = CreateNewCharacter();

        //"Scenario" : "tutorial"
        PlayerPrefs.SetString("Cur_scenario", label.text);
        PlayerPrefs.SetString("Char_route", id);
        SceneManager.LoadScene("MainScene");
    }

    //���� ĳ���Ͱ� ������, ����.
    public string CreateNewCharacter() // string name, ... else stats.
    {
        //Char == name

        //...? �̰� �� ���� �� ����?
        if (Char.text == "" || Char.text == null) { Debug.LogError("nonText"); return "0"; }
        Debug.Log(Char.text);
        route = UnityEngine.Application.persistentDataPath;
        
        //ĳ���� ����Ʈ �������������� ����.
        TextAsset str = Resources.Load<TextAsset>("Text/Info/Charlist");
        if (!File.Exists(route + "/Charlist.json")) File.WriteAllText(route + "/Charlist.json", str.text);

        //id ����. �ߺ�Ȯ��.
        cur_id = GenerateRandomString(16);
        string strlist = convertJson.MakeJson(route + "/Charlist.json");
        JObject jcharlist = JObject.Parse(strlist);
        bool check_dup;
        int i = 0;
        do
        {
            check_dup = false;
            if (i++ > 10000) break; //���� ���� ���� ����
            foreach (JToken id in jcharlist["List"])
            {
                if (id["id"] != null && id["id"].ToString() == cur_id)
                {
                    cur_id = GenerateRandomString(16); check_dup = true; break;
                }
            }
        } while (check_dup);

        //ĳ���� ����. �׸��� list���ε�.
        JObject character = new JObject
        {
            ["name"] = Char.text,
            ["id"] = cur_id
        };
        JArray list = (JArray)jcharlist["List"];
        list.Add(character);
        File.WriteAllText(route + "/Charlist.json", jcharlist.ToString());
        CreateFreeset(route + "/" + cur_id, cur_id);

        //���� �ó����� ��Ʈ �ۼ�.
        string world = convertJson.MakeJson(route + "/" + cur_id + "/Info/World.json");
        JObject jworld = JObject.Parse(world);
        jworld["Scenario"] = label.text;
        File.WriteAllText(route + "/" + cur_id + "/Info/World.json", jworld.ToString());

        //��ų ���� ����. �⺻ Ŭ���� ���.
        SetSkillSets();

        //����.
        Debug.Log("Creat Complete");
        return cur_id;
    }

    //�� ĳ���� ���� �� ���� ����
    private void CreateFreeset(string resources, string id)
    {
        Directory.CreateDirectory(route + "/" + id);
        Directory.CreateDirectory(resources + "/Info");

        //Info�� ���� ����
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Text/Info/Backup/Info");
        foreach (TextAsset jsonFile in jsonFiles)
            File.WriteAllText(resources + "/Info/" + jsonFile.name + ".json", jsonFile.text);

        //Freeset ���� ����
        int j = jsonFiles.Length;
        TextAsset[] backFiles = Resources.LoadAll<TextAsset>("Text/Info/Backup");
        foreach (TextAsset jsonFile in backFiles)
        {
            //info�� ��ø�Ǵ� ���� ����
            if (j > 0 && jsonFiles[jsonFiles.Length - j--].name == jsonFile.name) continue;
            if(jsonFile.name == "main") File.WriteAllText(resources + "/main.txt", jsonFile.text);
            else File.WriteAllText(resources + "/" + jsonFile.name + ".json", jsonFile.text);
        }

        //����Ʈ ����Ʈ�ε�, ��ġ�� �� ���⿩���ϳ�?
        CreateQuestline(resources + "/Quest");
    }


    string region = "none";
    //����Ʈ ����Ʈ ����(�ʱ��) - �������� ���...
    private void CreateQuestline(string resources)
    {
        Directory.CreateDirectory(resources);
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Text/Quest");

        string str = Resources.Load<TextAsset>("Text/Info/Backup/Info/Questlist").ToString();
        JObject jroot = JObject.Parse(str);
        QuestTable tmp = JsonUtility.FromJson<QuestTable>(jroot.ToString());
        string cur_region = "none"; //�ʱⰪ�� Forest != forest�� �߻��� ���� ����.. �׷��� table�� �־����� ������ tmp�� �ȵ� ����. �׷��� �ʱⰪ�� ���� ���������..
        //�� ������ quest.json ���� �о�� 
        foreach (TextAsset jsonFile in jsonFiles)
        {
            JToken jquest = JObject.Parse(jsonFile.text);
            //quest.json�� num���� regionȮ�� �� �з� - ���� ������ ���� �����ϰ�����. Ȥ�� �𸣴�.
            foreach (JToken quest in jquest["quest"])
            {
                //region�� �����, ���� �ʱ�ȭ
                if (cur_region == "none")
                {
                    cur_region = quest["condition"]["region"].ToString();
                    region = resources + "/" + quest["condition"]["region"].ToString();
                    if (!Directory.Exists(region)) Directory.CreateDirectory(region);
                }
                if (cur_region != quest["condition"]["region"].ToString())
                {
                    region = resources + "/" + quest["condition"]["region"].ToString();
                    //�������� ���� table�� �ۼ�.
                    if (!Directory.Exists(region)) Directory.CreateDirectory(region);
                    string questlist = JsonConvert.SerializeObject(tmp);
                    Debug.Log(questlist);
                    File.WriteAllText(resources + "/" + cur_region + "/available.json", questlist);

                    //���� �ʱ�ȭ
                    tmp = JsonUtility.FromJson<QuestTable>(jroot.ToString());
                    cur_region = quest["condition"]["region"].ToString();
                }
                tmp.quest.Add(new Questlist(quest["name"].ToString(), quest["condition"]["region"].ToString(), (int)quest["num"]));
            }
            //tmp.quest[i++] = JsonUtility.FromJson<Questlist>(jsonFile.text); //i�� json�̶� index�� �� ��������.
        }
        File.WriteAllText(region + "/available.json", tmp.ToString()); //������ regionó��

        //quest frame��ĸ� �����.
        File.WriteAllText(resources + "/fail.json", str);
        File.WriteAllText(resources + "/complete.json", str);
    }

    private void SetSkillSets()
    {
        //BaseDefense. BaseSkill. WarriorSkill. WizardSkill.
        //TextAsset back = Resources.Load<TextAsset>("Text/Info/Backup/SkillSet/BaseSkill.json");
        //base ����
        string skillroute = route + "/" + cur_id + "/Info/Skill.json";
        string charskill = convertJson.MakeJson(route + "/" + cur_id + "/Info/Skill.json");
        JObject jcharskill = JObject.Parse(charskill);
        JArray jskills = new JArray();

        //baseskill ��������
        string str = Resources.Load<TextAsset>("Text/Info/SkillSet/BaseSkill").ToString();
        JObject jbaseskill = JObject.Parse(str);
        foreach(JToken jskill in jbaseskill["Attack"]) jskills.Add(jskill.ToObject<JObject>());

        //basedefense. �߿�, ��� ��������. -> ���� �Ű������� ���� �ϳ� ����
        str = Resources.Load<TextAsset>("Text/Info/SkillSet/BaseDefense").ToString();
        jbaseskill = JObject.Parse(str);
        foreach (JToken jskill in jbaseskill["Attack"])
        {
            if (jskill["name"].ToString() == "���")
                jskills.Add(jskill.ToObject<JObject>());
        }


        jcharskill["Attack"] = jskills;
        File.WriteAllText(route + "/" + cur_id + "/Info/Skill.json", jcharskill.ToString());
    }

    // ������ ������ �� ���ڸ� ������ 16�ڸ� ���ڿ� ����
    static string GenerateRandomString(int length)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder result = new StringBuilder();
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            // validChars���� �������� ���� ����
            char randomChar = validChars[random.Next(validChars.Length)];
            result.Append(randomChar);
        }

        return result.ToString();
    }

}
