using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //��Ʋ ��ų ������.
    //������ ��ų�� ��� ������ �ұ��~

    private string skill_route;
    private JObject jskill;
    private JToken skillToken;

    [SerializeField] private GameObject[] skillSlotUi;

    private void Start()
    {
        skill_route = Application.persistentDataPath + "/Info/Skill.json";
        string skill_str = new ConvertJson().MakeJson(skill_route);
        
        jskill = JObject.Parse(skill_str);
        //skillToken = jskill["Attack"];


        UpdateSkillSet("all");
    }

    //���� ui���� ���̴� ��ų ���� �ٲ۴�.
    public void UpdateSkillSet(string type) // type : all, base, skill, item.
    {
        for (int j = 0; j < skillSlotUi.Length; j++) skillSlotUi[j].SetActive(true);

        int i = 0;
        //all, �⺻����. ��ų. ������. �� ������ ����? - 1.�� ��Ʈ ���������� �ҷ����� 2.�� ��Ʈ���� skillslot�̸� �����صα�. ĭ�� ������ �ʾƼ� ����غ����ҵ�
        foreach(JToken skill in jskill["Attack"])
        {
            BattleAction tmp = JsonUtility.FromJson<BattleAction>(skill.ToString()); ;
            //tmp.index = i; //action�� index�� �װ� �ƴҲ���..
            tmp.index = 0;
            skillSlotUi[i].GetComponent<SkillSlotUi>().Set(tmp);

            //Debug.Log(tmp.index + tmp.name + tmp.criticalProbability);
            i++;
        }
        for (; i < skillSlotUi.Length; i++) skillSlotUi[i].SetActive(false);

    }

    public void CategoryClicked(TextMeshProUGUI text)
    {
        //ī�װ��ʿ��� �������.
    }

}
