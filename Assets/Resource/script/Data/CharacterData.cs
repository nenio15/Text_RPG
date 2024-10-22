using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.LightingExplorerTableColumn;

[System.Serializable]
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
    public string Skill = "Rock";
}

public class CharacterData
{
    enum DataType
    {
        Skill,
        Hp,
        Mp,
        Exp
    }

    public JObject SetJson(string route)
    {
        string cha = new ConvertJson().MakeJson(route); //jsonconverter �ִµ�?
        return JObject.Parse(cha);
    }

    /*
    //character�޾Ƴ���. �ϴ� �Ⱦ�
    public Character Set(string route)
    {
        string str = new ConvertJson().MakeJson(route);
        return JsonUtility.FromJson<Character>(str);
    }
    */

    //json���� ���� ������Ʈ. player�� �����ִµ�. ��..
    public void Upload(int type, string content, JObject raw_data, string route)
    {
        switch (type)
        {
            case (int)DataType.Skill:
                raw_data["Info"]["Skill"] = content; //json�� �ݿ��ǳ�, �ߺ����� �����̴�. �ٸ� ���⵵�� ������ �ϴ� �̷��� �д�.
                break;
            case 1:
                break;
            default:
                break;
        }

        File.WriteAllText(route, raw_data.ToString());
    }


}
