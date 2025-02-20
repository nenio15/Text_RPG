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

public class MoveStartScene : MonoBehaviour
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
        if (decision_id == "") return;
        route = UnityEngine.Application.persistentDataPath;
        string world = convertJson.MakeJson(route + "/" + decision_id + "/Info/World.json");
        JObject jworld = JObject.Parse(world);
        if(jworld["Scenario"] == null) { Debug.LogError("scenario�� �������� �ʾҽ��ϴ�."); return; }
        
        PlayerPrefs.SetString("Cur_scenario", jworld["Scenario"].ToString());
        PlayerPrefs.SetString("Char_route", decision_id);
        SceneManager.LoadScene("MainScene");
    }

    //�ܺο��� ���� �� �ٷ� �÷���
    public void MoveToMain()
    {
        string id = CreateNewCharacter();

        //"Scenario" : "tutorial"
        PlayerPrefs.SetString("Cur_scenario", label.text);
        PlayerPrefs.SetString("Char_route", id);
        SceneManager.LoadScene("MainScene");
    }

    //���� ĳ���Ͱ� ������, ����.
    public string CreateNewCharacter()
    {
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
        //ĳ���� ����, ���� ����
        LoadFreeset(route + "/" + cur_id, cur_id);


        //���� �ó����� ��Ʈ �ۼ�.
        string world = convertJson.MakeJson(route + "/" + cur_id + "/Info/World.json");
        JObject jworld = JObject.Parse(world);
        jworld["Scenario"] = label.text;
        File.WriteAllText(route + "/" + cur_id + "/Info/World.json", jworld.ToString());

        Debug.Log("Creat Complete");
        return cur_id;
    }

    private void LoadFreeset(string resources, string id)
    {
        Directory.CreateDirectory(route + "/" + id);
        Directory.CreateDirectory(resources + "/Info");

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Text/Info/backup/Info");
        foreach (TextAsset jsonFile in jsonFiles)
            File.WriteAllText(resources + "/Info/" + jsonFile.name + ".json", jsonFile.text);

        int j = jsonFiles.Length;
        TextAsset[] backFiles = Resources.LoadAll<TextAsset>("Text/Info/backup");
        foreach (TextAsset jsonFile in backFiles)
        {
            if (j > 0 && jsonFiles[jsonFiles.Length - j--].name == jsonFile.name) continue;
            if(jsonFile.name == "main") File.WriteAllText(resources + "/main.txt", jsonFile.text);
            else File.WriteAllText(resources + "/" + jsonFile.name + ".json", jsonFile.text);
        }
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
