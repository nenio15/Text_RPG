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

    public string Name = "홍길동";
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
        string cha = new ConvertJson().MakeJson(route); //jsonconverter 있는듯?
        return JObject.Parse(cha);
    }

    /*
    //character받아내기. 일단 안씀
    public Character Set(string route)
    {
        string str = new ConvertJson().MakeJson(route);
        return JsonUtility.FromJson<Character>(str);
    }
    */

    //json으로 정보 업데이트. player만 쓸수있는데. 흠..
    public void Upload(int type, string content, JObject raw_data, string route)
    {
        switch (type)
        {
            case (int)DataType.Skill:
                raw_data["Info"]["Skill"] = content; //json에 반영되나, 중복적인 선언이다. 다만 복잡도를 생각해 일단 이렇게 둔다.
                break;
            case 1:
                break;
            default:
                break;
        }

        File.WriteAllText(route, raw_data.ToString());
    }


}
