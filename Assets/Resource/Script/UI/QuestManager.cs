using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditor.PackageManager.Requests;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class QuestManager : MonoBehaviour
{
    /*
     * 그리고 update방식은 아이템의 방식을 차용. -> 사실 구조가 같으면 스크립트도... 아냐.. 이거는 구분이 편해. 나중에 수정이 용이하게끔.
     * 의문. -> 그럼 모듈식이 낫지 않음? 흠좀무.
     * 
     * 그래서 진입을 어떻게, 어디서 감시감독임?
     * quest의 분류도 필요한데. 지역만으로? 그것도 나쁘진 않지. 건물?
     */

    [SerializeField] private GameObject[] questlists;
    [SerializeField] private QuestUi questUi;
    public Questlist cur_quest;

    //Json 관련 선언
    private string questlist_route;
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;

    public QuestTable questTable; //여기에서 각각을 받아서 해야겠지요? ㅇㅇ..
    public static QuestManager Instance;

    private void Awake()
    {
        Instance = this;
        questlist_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/Questlist.json";

        //임시 정의. 테스팅용. 원래 여기 안씀.
        string str = convertJson.MakeJson(questlist_route);
        jroot = JObject.Parse(str);
        questTable = JsonUtility.FromJson<QuestTable>(jroot.ToString());
        cur_quest = questTable.quest[0];
        Debug.Log(cur_quest.name + cur_quest.region);
        questUi.Set(questTable.quest[0]); //, "Forest/tmp");

        UpdateList();

        //현재 리스트와 현재 region. 둘을 체킹한다.
        //1.condition, 2.complete 3.fail 모든 가짓수의 checkout이다. state가 ..?
    }

    //퀘스트 리스트에 대한 업뎃
    public void UpdateList()
    {
        int i = 0;
        string str = convertJson.MakeJson(questlist_route);
        jroot = JObject.Parse(str);
        questTable = JsonUtility.FromJson<QuestTable>(jroot.ToString());

        //번호 붙이기
        //for (int j = 0; j < items.Length; j++)
        //    items[j].index = j;
        
        foreach (JToken quest in jroot["quest"])
        {
            TextMeshProUGUI title = questlists[i].GetComponentInChildren<TextMeshProUGUI>();
            Image image = questlists[i].GetComponentInChildren<Image>();
            title.text = quest["name"].ToString();
            //image.sprite = questlists[i].
            i++;
        }

        //빈칸은 null // 원래는 리스트 생성을 제거.
        for (; i < questlists.Length; i++)
        {
            questlists[i].GetComponentInChildren<TextMeshProUGUI>().text = " ";
        }

    }

    //Accept, complete, drop
    //state : available, progress, complete, fail
    //state변경과 함께 다른 json으로 가야하는데. 그에 대한 수정 추가 필요.
    public void CompleteQuest()
    {
        //reward.
        Debug.Log(cur_quest.name);
        JToken reward = new Dictionary().GetCondition("reward", cur_quest);
        Debug.Log("대충 reward" + reward["gold"]);
        DropQuest();
    }

    //받을 수 있는 퀘스트의 한계 필요..?
    public void AcceptQuest()
    {
        //오류 발생시, windowManger의 Add를 참조
        questTable.quest.Add(cur_quest); //종류가 겹치면 죽나?
        Debug.Log("111");
        string tableJson = JsonConvert.SerializeObject(questTable);
        File.WriteAllText(questlist_route, tableJson);
        UpdateList();
    }

    public void DropQuest()
    {
        questTable.quest.Remove(cur_quest);
        string tableJson = JsonConvert.SerializeObject(questTable);
        File.WriteAllText(questlist_route, tableJson);        
        UpdateList();
        //view도 변경 필.
    }

    public void OnClick(int index)
    {
        cur_quest = questTable.quest[index];
        questUi.Set(cur_quest); //, "Forest/tmp");
    }

}