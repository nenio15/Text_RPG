using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dictionary
{
    //이름과 카테고리를 받는다. 경로를 통해 참조한다. itemdata를 반환한다.
    public itemData SetItem(string name, string category)
    {
        //tmp칸의 예외처리. - 변경필요
        if(Resources.Load<TextAsset>("Text/Info/Dictionary/Item/" + category) == null) return null;
        string str = Resources.Load<TextAsset>("Text/Info/Dictionary/Item/" + category).ToString();


        JObject jitemdata = JObject.Parse(str);
        //Debug.Log("Finding : " + name + category);
        foreach (JToken item in jitemdata["item"])
            if(item["name"].ToString() == name)
                return JsonUtility.FromJson<itemData>(item.ToString());

        return null;
    }

    //반환값 변경할 것.
    public NarrativeSlot SetNarrative(string name, string category)
    {
        string str = Resources.Load<TextAsset>("Text/Info/Dictionary/Memory/" + category).ToString();

        JObject jitemdata = JObject.Parse(str);
        foreach (JToken narrative in jitemdata["narrative"])
            if (narrative["name"].ToString() == name)
                return JsonUtility.FromJson<NarrativeSlot>(narrative.ToString());

        return null;
    }

    //player_info = JsonUtility.FromJson<Character>(info.ToString());
}
