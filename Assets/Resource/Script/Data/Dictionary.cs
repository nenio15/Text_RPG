using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
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
        if (Resources.Load<TextAsset>("Text/Info/Dictionary/Memory/" + category) == null) { Debug.Log("narrative format does not match"); return null; }
        string str = Resources.Load<TextAsset>("Text/Info/Dictionary/Memory/" + category).ToString();

        JObject jitemdata = JObject.Parse(str);
        foreach (JToken narrative in jitemdata["narrative"])
            if (narrative["name"].ToString() == name)
                return JsonUtility.FromJson<NarrativeSlot>(narrative.ToString());

        return null;
    }

    public QuestForm SetQuest(Questlist q)
    {
        if (Resources.Load<TextAsset>("Text/Quest/" + q.region + "/" + q.name) == null) { Debug.Log("quest format does not match"); return null; }
        string str = Resources.Load<TextAsset>("Text/Quest/" + q.region + "/" + q.name).ToString();

        JObject jitemdata = JObject.Parse(str);
        foreach (JToken quest in jitemdata["quest"])
            if ((int)quest["num"] == q.num)
                return JsonUtility.FromJson<QuestForm>(quest.ToString());

        return null;
    }

    //�����Ŷ� ��ġ���ϴµ� ������ �� ��ġ����...
    public JToken GetCondition(string condition_type, Questlist q)
    {
        if (Resources.Load<TextAsset>("Text/Quest/" + q.region + "/" + q.name) == null) { Debug.Log("quest format does not match"); return null; }
        string str = Resources.Load<TextAsset>("Text/Quest/" + q.region + "/" + q.name).ToString();

        JObject jitemdata = JObject.Parse(str);
        foreach (JToken quest in jitemdata["quest"])
            if ((int)quest["num"] == q.num)
                return quest[condition_type];

        return null;
    }

    //player_info = JsonUtility.FromJson<Character>(info.ToString());
}
