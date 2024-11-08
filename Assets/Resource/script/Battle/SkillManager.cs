using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //배틀 스킬 관리자.
    //아이템 스킬은 어떻게 관리를 할까요~

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

    //현재 ui에서 보이는 스킬 셋을 바꾼다.
    public void UpdateSkillSet(string type) // type : all, base, skill, item.
    {
        for (int j = 0; j < skillSlotUi.Length; j++) skillSlotUi[j].SetActive(true);

        int i = 0;
        //all, 기본공격. 스킬. 아이템. 이 넷으로 구분? - 1.각 시트 누를때마다 불러오기 2.각 시트마다 skillslot미리 세팅해두기. 칸이 많지는 않아서 고민해봐야할듯
        foreach(JToken skill in jskill["Attack"])
        {
            BattleAction tmp = JsonUtility.FromJson<BattleAction>(skill.ToString()); ;
            //tmp.index = i; //action의 index는 그게 아닐꺼야..
            tmp.index = 0;
            skillSlotUi[i].GetComponent<SkillSlotUi>().Set(tmp);

            //Debug.Log(tmp.index + tmp.name + tmp.criticalProbability);
            i++;
        }
        for (; i < skillSlotUi.Length; i++) skillSlotUi[i].SetActive(false);

    }

    public void CategoryClicked(TextMeshProUGUI text)
    {
        //카테고리쪽에서 눌린경우.
    }

}
