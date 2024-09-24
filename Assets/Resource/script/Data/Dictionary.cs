using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dictionary
{
    //�̸��� ī�װ��� �޴´�. ��θ� ���� �����Ѵ�. itemdata�� ��ȯ�Ѵ�.
    public itemData SetItem(string name, string category)
    {
        string dictionary_route = Application.dataPath + "/Resource/Text/Info/Dictionary/Item/" + category + ".json";
        string str = new ConvertJson().MakeJson(dictionary_route);

        JObject jitemdata = JObject.Parse(str);
        //Debug.Log("Finding : " + name + category);
        foreach (JToken item in jitemdata["item"])
            if(item["name"].ToString() == name)
                return JsonUtility.FromJson<itemData>(item.ToString());

        return null;
    }

    //player_info = JsonUtility.FromJson<Character>(info.ToString());
}
