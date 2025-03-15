using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dictionary
{
    //�̸��� ī�װ��� �޴´�. ��θ� ���� �����Ѵ�. itemdata�� ��ȯ�Ѵ�.
    public itemData SetItem(string name, string category)
    {
        //tmpĭ�� ����ó��. - �����ʿ�
        if(Resources.Load<TextAsset>("Text/Info/Dictionary/Item/" + category) == null) return null;
        string str = Resources.Load<TextAsset>("Text/Info/Dictionary/Item/" + category).ToString();


        JObject jitemdata = JObject.Parse(str);
        //Debug.Log("Finding : " + name + category);
        foreach (JToken item in jitemdata["item"])
            if(item["name"].ToString() == name)
                return JsonUtility.FromJson<itemData>(item.ToString());

        return null;
    }

    //��ȯ�� ������ ��.
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
