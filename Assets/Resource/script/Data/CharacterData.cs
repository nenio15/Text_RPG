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
    
    public int[] Exp = { 0, 100 };
    public int Money = 0;

    public string Name = "ȫ�浿";
    public string Class = "Warrior";
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
    public void Upload(string type, int count, JObject raw_data, string route)
    {
        int i = 0;

        switch (type)
        {
            case "skill":
                //raw_data["Info"]["Skill"] = content; //json�� �ݿ��ǳ�, �ߺ����� �����̴�. �ٸ� ���⵵�� ������ �ϴ� �̷��� �д�.
                break;
            case "gold":
                //������ �̰� �ʱ�ȭ�� ���� ��ũ��Ʈ �� ���̾�. ����. 
                i = (int)raw_data["Info"]["Money"] + count;
                raw_data["Info"]["Money"] = i;
                break;
            case "exp":
                i = (int)raw_data["Info"]["Exp"][0] + count;
                raw_data["Info"]["Exp"][0] = i;
                break;
            default:
                break;
        }

        File.WriteAllText(route, raw_data.ToString());
    }


}
