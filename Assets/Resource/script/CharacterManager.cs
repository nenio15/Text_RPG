using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class Character
{
    public int Level = 1;
    public int[] Hp = { 3, 3 };
    public int[] Mp = { 0, 0 };
    public int[] Stat = { 1, 1, 1, 1, 1, 1 };
    //int money;
    //int exp;

    public string Name = "ȫ�浿";
    public string Class = "Warrior";
}
// ����Ʈ�� ���������� ���.. �ʿ�..


public class CharacterManager : MonoBehaviour
{
    string charoute;

    //TextAsset jsonData = Resources.Load("Text/Battle/Player") as TextAsset;
    //var _data = JsonUtility.FromJson<Character>(jsonData.ToString());
    //string jsonstring = JsonUtility.ToJson(object obj);
    //T obj = JsonUtility.FromJson<T>(jsonData);
    private void Start()
    {
        charoute = Application.dataPath + @"\Resource\Text\Battle\Player.json";
        string cha = MakeJson(charoute);

        // first set
        JObject player = JObject.Parse(cha);
        JToken info = player["Info"];
        JToken bag = player["Backpack"];

        // ����Ϸ�
        Character list = JsonUtility.FromJson<Character>(info.ToString());
        list.Level = 2;
        //JToken ll = list.ToString();

        // �� string�� json���� ��ȯ���Ѽ� ���Խ�ų��.
        string newone = JsonUtility.ToJson(list);
        Debug.Log("new : " + newone);

        player["Info"] = newone;
        File.WriteAllText(charoute, player.ToString());
    }


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

}
