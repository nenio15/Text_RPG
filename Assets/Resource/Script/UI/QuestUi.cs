using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static UnityEditor.Progress;
using UnityEditor.Localization.Plugins.XLIFF.V12;


[Serializable]
public class QuestTable
{
    public List<Questlist> quest;
}

[Serializable]
public class Questlist
{
    public string name;
    public string region;
    public int num;

    public override bool Equals(object obj)
    {
        if (obj is Questlist other)
        {
            return name == other.name && region == other.region && num == other.num;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (name, region, num).GetHashCode();
    }
}

[Serializable]
public class QuestForm
{
    [Header("INFO")]
    public string name; //아이템은 name과 img를 나누어서 관리. name은 나중에 시트에 따라 언어 변경?
    public string img;
    public int num;
    public string des;
    public string hint;

    public int level;
    public string state;
    public string prerelation;
    public string postrelation;

    //public ItemAddition[] effect;
    //public ItemAddition[] limit;

    //public string addition;

}

public class QuestUi : MonoBehaviour
{
    //이거 너무 긴데.. 그냥 Children으로 다 받아버리는게 낫지 않나? 흠.
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI region;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI hint;
    [SerializeField] private TextMeshProUGUI goal;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private TextMeshProUGUI reward;

    public QuestForm cur_quest;
    private JToken jcom;
    private Dictionary dictionary = new Dictionary();

    public void Set(Questlist quest) //, string route)//대충 값 받기
    {
        cur_quest = dictionary.SetQuest(quest);
        title.text = cur_quest.name;
        level.text = cur_quest.level.ToString();
        hint.text = cur_quest.hint;
        content.text = cur_quest.des;

        //region, goal, reward의 작성 띠ㅏ로.
        //GetDetail(route);
        GetDetail(quest);
    }

    public void GetDetail(Questlist quest)
    {
        //route는 지역과 json명을 포함.
        string str = Resources.Load<TextAsset>("Text/Quest/" + quest.region + "/" + quest.name).ToString();
        jcom = dictionary.GetCondition("complete_condition", quest); //JObject.Parse(str);
        //JToken jdetail = //jquest["quest"][0]; //이거 왜... 흠...  아, 위 resources.load가 dictionary에 있어야하는 내용이었네...
        JToken jcomcon = jcom;
        JToken jreward = dictionary.GetCondition("reward", quest);
        
        region.text = jcomcon["region"].ToString();
        goal.text = jcomcon["type"].ToString(); //이거에 대한 전격 분석이 필요하다.. 이거는 해석기가 따로 필요한데.... 구조도가 필요하겠네?
        reward.text = "gold : " + jreward["gold"].ToString() + "  exp : " + jreward["exp"].ToString();// + " \nget : " + jreward["get"] //이것도 양식이 필요... 흠. 함수에 함수에 함수라니. 참 흠.
    }

    public void Goal()
    {
        //ㅇㅇ
    }

    public void Reward()
    {
        //ㅇㅇ
    }
}
