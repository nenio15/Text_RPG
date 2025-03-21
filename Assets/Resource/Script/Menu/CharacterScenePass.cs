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

    //캐릭터 생성을 통한 플레이 버튼과,
    //캐릭터 선택을 통한 플레이 분할이 나을듯..
    //해당 스크립트는 일단, 캐릭터 생성에 한함.

    public void IdSelect(string id)
    {
        decision_id = id;
    }

    public void CharSelect()
    {
        //id와 scenario 확인 후 진입
        if (decision_id == "") return;
        route = UnityEngine.Application.persistentDataPath;
        string world = convertJson.MakeJson(route + "/" + decision_id + "/Info/World.json");
        JObject jworld = JObject.Parse(world);
        if(jworld["Scenario"] == null) { Debug.LogError("scenario가 지정되지 않았습니다."); return; }
        
        PlayerPrefs.SetString("Cur_scenario", jworld["Scenario"].ToString());
        PlayerPrefs.SetString("Char_route", decision_id);
        SceneManager.LoadScene("MainScene");
    }

    //외부에서 생성 후 바로 플레이 - 변경필 
    public void CreateAndPlay()
    {
        string id = CreateNewCharacter();

        //"Scenario" : "tutorial"
        PlayerPrefs.SetString("Cur_scenario", label.text);
        PlayerPrefs.SetString("Char_route", id);
        SceneManager.LoadScene("MainScene");
    }

    //기존 캐릭터가 없을시, 생성.
    public string CreateNewCharacter() // string name, ... else stats.
    {
        //Char == name

        //...? 이거 왜 씨발 안 막힘?
        if (Char.text == "" || Char.text == null) { Debug.LogError("nonText"); return "0"; }
        Debug.Log(Char.text);
        route = UnityEngine.Application.persistentDataPath;
        
        //캐릭터 리스트 존재하지않으면 생성.
        TextAsset str = Resources.Load<TextAsset>("Text/Info/Charlist");
        if (!File.Exists(route + "/Charlist.json")) File.WriteAllText(route + "/Charlist.json", str.text);

        //id 생성. 중복확인.
        cur_id = GenerateRandomString(16);
        string strlist = convertJson.MakeJson(route + "/Charlist.json");
        JObject jcharlist = JObject.Parse(strlist);
        bool check_dup;
        int i = 0;
        do
        {
            check_dup = false;
            if (i++ > 10000) break; //대충 무한 부팅 막기
            foreach (JToken id in jcharlist["List"])
            {
                if (id["id"] != null && id["id"].ToString() == cur_id)
                {
                    cur_id = GenerateRandomString(16); check_dup = true; break;
                }
            }
        } while (check_dup);

        //캐릭터 생성. 그리고 list업로딩.
        JObject character = new JObject
        {
            ["name"] = Char.text,
            ["id"] = cur_id
        };
        JArray list = (JArray)jcharlist["List"];
        list.Add(character);
        File.WriteAllText(route + "/Charlist.json", jcharlist.ToString());
        CreateFreeset(route + "/" + cur_id, cur_id);

        //현재 시나리오 루트 작성.
        string world = convertJson.MakeJson(route + "/" + cur_id + "/Info/World.json");
        JObject jworld = JObject.Parse(world);
        jworld["Scenario"] = label.text;
        File.WriteAllText(route + "/" + cur_id + "/Info/World.json", jworld.ToString());

        //스킬 세팅 생성. 기본 클래스 방식.
        SetSkillSets();

        //종료.
        Debug.Log("Creat Complete");
        return cur_id;
    }

    //새 캐릭터 폴더 및 파일 생성
    private void CreateFreeset(string resources, string id)
    {
        Directory.CreateDirectory(route + "/" + id);
        Directory.CreateDirectory(resources + "/Info");

        //Info내 파일 생성
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Text/Info/Backup/Info");
        foreach (TextAsset jsonFile in jsonFiles)
            File.WriteAllText(resources + "/Info/" + jsonFile.name + ".json", jsonFile.text);

        //Freeset 파일 생성
        int j = jsonFiles.Length;
        TextAsset[] backFiles = Resources.LoadAll<TextAsset>("Text/Info/Backup");
        foreach (TextAsset jsonFile in backFiles)
        {
            //info와 중첩되는 파일 생략
            if (j > 0 && jsonFiles[jsonFiles.Length - j--].name == jsonFile.name) continue;
            if(jsonFile.name == "main") File.WriteAllText(resources + "/main.txt", jsonFile.text);
            else File.WriteAllText(resources + "/" + jsonFile.name + ".json", jsonFile.text);
        }

        //퀘스트 리스트인데, 위치가 꼭 여기여야하나?
        CreateQuestline(resources + "/Quest");
    }


    string region = "none";
    //퀘스트 리스트 세팅(초기용) - 업뎃용은 어떻게...
    private void CreateQuestline(string resources)
    {
        Directory.CreateDirectory(resources);
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Text/Quest");

        string str = Resources.Load<TextAsset>("Text/Info/Backup/Info/Questlist").ToString();
        JObject jroot = JObject.Parse(str);
        QuestTable tmp = JsonUtility.FromJson<QuestTable>(jroot.ToString());
        string cur_region = "none"; //초기값이 Forest != forest라서 발생한 오류 같음.. 그래서 table만 넣어지고 이후의 tmp가 안들어간 느낌. 그러면 초기값을 어케 써야좋을까..
        //각 지역별 quest.json 파일 읽어내기 
        foreach (TextAsset jsonFile in jsonFiles)
        {
            JToken jquest = JObject.Parse(jsonFile.text);
            //quest.json안 num마다 region확인 후 분류 - 같은 파일은 거의 동일하겠지만. 혹시 모르니.
            foreach (JToken quest in jquest["quest"])
            {
                //region의 변경시, 여럿 초기화
                if (cur_region == "none")
                {
                    cur_region = quest["condition"]["region"].ToString();
                    region = resources + "/" + quest["condition"]["region"].ToString();
                    if (!Directory.Exists(region)) Directory.CreateDirectory(region);
                }
                if (cur_region != quest["condition"]["region"].ToString())
                {
                    region = resources + "/" + quest["condition"]["region"].ToString();
                    //기존까지 모은 table로 작성.
                    if (!Directory.Exists(region)) Directory.CreateDirectory(region);
                    string questlist = JsonConvert.SerializeObject(tmp);
                    Debug.Log(questlist);
                    File.WriteAllText(resources + "/" + cur_region + "/available.json", questlist);

                    //변수 초기화
                    tmp = JsonUtility.FromJson<QuestTable>(jroot.ToString());
                    cur_region = quest["condition"]["region"].ToString();
                }
                tmp.quest.Add(new Questlist(quest["name"].ToString(), quest["condition"]["region"].ToString(), (int)quest["num"]));
            }
            //tmp.quest[i++] = JsonUtility.FromJson<Questlist>(jsonFile.text); //i랑 json이랑 index가 안 맞을것임.
        }
        File.WriteAllText(region + "/available.json", tmp.ToString()); //마지막 region처리

        //quest frame양식만 만들기.
        File.WriteAllText(resources + "/fail.json", str);
        File.WriteAllText(resources + "/complete.json", str);
    }

    private void SetSkillSets()
    {
        //BaseDefense. BaseSkill. WarriorSkill. WizardSkill.
        //TextAsset back = Resources.Load<TextAsset>("Text/Info/Backup/SkillSet/BaseSkill.json");
        //base 세팅
        string skillroute = route + "/" + cur_id + "/Info/Skill.json";
        string charskill = convertJson.MakeJson(route + "/" + cur_id + "/Info/Skill.json");
        JObject jcharskill = JObject.Parse(charskill);
        JArray jskills = new JArray();

        //baseskill 가져오기
        string str = Resources.Load<TextAsset>("Text/Info/SkillSet/BaseSkill").ToString();
        JObject jbaseskill = JObject.Parse(str);
        foreach(JToken jskill in jbaseskill["Attack"]) jskills.Add(jskill.ToObject<JObject>());

        //basedefense. 중에, 방어 가져오기. -> 추후 매개변수를 통해 하나 선택
        str = Resources.Load<TextAsset>("Text/Info/SkillSet/BaseDefense").ToString();
        jbaseskill = JObject.Parse(str);
        foreach (JToken jskill in jbaseskill["Attack"])
        {
            if (jskill["name"].ToString() == "방어")
                jskills.Add(jskill.ToObject<JObject>());
        }


        jcharskill["Attack"] = jskills;
        File.WriteAllText(route + "/" + cur_id + "/Info/Skill.json", jcharskill.ToString());
    }

    // 임의의 영문자 및 숫자를 포함한 16자리 문자열 생성
    static string GenerateRandomString(int length)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder result = new StringBuilder();
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            // validChars에서 랜덤으로 문자 선택
            char randomChar = validChars[random.Next(validChars.Length)];
            result.Append(randomChar);
        }

        return result.ToString();
    }

}
