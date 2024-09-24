using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dictionary
{
    //이름과 카테고리를 받는다. 경로를 통해 참조한다. itemdata를 반환한다.
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
