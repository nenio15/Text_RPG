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
    public string name; //�������� name�� img�� ����� ����. name�� ���߿� ��Ʈ�� ���� ��� ����?
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
    //�̰� �ʹ� �䵥.. �׳� Children���� �� �޾ƹ����°� ���� �ʳ�? ��.
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

    public void Set(Questlist quest) //, string route)//���� �� �ޱ�
    {
        cur_quest = dictionary.SetQuest(quest);
        title.text = cur_quest.name;
        level.text = cur_quest.level.ToString();
        hint.text = cur_quest.hint;
        content.text = cur_quest.des;

        //region, goal, reward�� �ۼ� �줿��.
        //GetDetail(route);
        GetDetail(quest);
    }

    public void GetDetail(Questlist quest)
    {
        //route�� ������ json���� ����.
        string str = Resources.Load<TextAsset>("Text/Quest/" + quest.region + "/" + quest.name).ToString();
        jcom = dictionary.GetCondition("complete_condition", quest); //JObject.Parse(str);
        //JToken jdetail = //jquest["quest"][0]; //�̰� ��... ��...  ��, �� resources.load�� dictionary�� �־���ϴ� �����̾���...
        JToken jcomcon = jcom;
        JToken jreward = dictionary.GetCondition("reward", quest);
        
        region.text = jcomcon["region"].ToString();
        goal.text = jcomcon["type"].ToString(); //�̰ſ� ���� ���� �м��� �ʿ��ϴ�.. �̰Ŵ� �ؼ��Ⱑ ���� �ʿ��ѵ�.... �������� �ʿ��ϰڳ�?
        reward.text = "gold : " + jreward["gold"].ToString() + "  exp : " + jreward["exp"].ToString();// + " \nget : " + jreward["get"] //�̰͵� ����� �ʿ�... ��. �Լ��� �Լ��� �Լ����. �� ��.
    }

    public void Goal()
    {
        //����
    }

    public void Reward()
    {
        //����
    }
}
