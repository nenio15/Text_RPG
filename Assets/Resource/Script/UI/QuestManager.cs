using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.PackageManager.Requests;
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
        UpdateList();

        //string str = Resources.Load<TextAsset>("Text/QuestForm/" + route).ToString();
        //jquest = JObject.Parse(str);
        //JToken jcomcon = jquest["complete_condition"]; -> 해당 type과 그 case에 따른 확인
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
        
        //인벤토리.json에서 아이템 읽어서 각 slot에 할당.
        foreach (JToken quest in jroot["quest"])
        {
            TextMeshProUGUI title = questlists[i].GetComponentInChildren<TextMeshProUGUI>();
            Image image = questlists[i].GetComponentInChildren<Image>();

            title.text = quest["name"].ToString();
            //image.sprite = questlists[i].
            //그리고 어디 반영..? 아니야 그건 아니지.
            i++;
        }

        //임시 정의. 테스팅용. 원래 여기 안씀.
        questUi.Set(questTable.quest[0]); //, "Forest/tmp");
    }

    public void OnClick(int index)
    {
        questUi.Set(questTable.quest[index]); //, "Forest/tmp");
    }

}