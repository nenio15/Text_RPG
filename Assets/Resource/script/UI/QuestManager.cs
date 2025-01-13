using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    /*
     * 일단. json questlist를 만들자.
     * 그리고 quest형식을 담아내는 개별 json도 필요.
     * 그리고 update방식은 아이템의 방식을 차용. -> 사실 구조가 같으면 스크립트도... 아냐.. 이거는 구분이 편해. 나중에 수정이 용이하게끔.
     * 의문. -> 그럼 모듈식이 낫지 않음? 흠좀무.
     */

    [SerializeField] private GameObject[] questlists;

    //Json 관련 선언
    private string questlist_route;
    //private static string k = "item";
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;


    /*
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
    //public itemData itemData;

    
    //private JToken jinventory;

    public static Inventory Instance;
    */

    private void Awake()
    {
        //Instance = this;
        questlist_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/Questlist.json";
        UpdateList();
    }


    public void UpdateList()
    {
        //각 리스트마다 참조해서 수정하기.
        //questlists[j].GetComponentInChildren<TextMeshProUGUI>();
        
        
        int i = 0;
        string str = convertJson.MakeJson(questlist_route);
        jroot = JObject.Parse(str);
        //jinventory = jroot[k];

        //인벤토리.json에서 아이템 읽어서 각 slot에 할당.
        foreach (JToken quest in jroot["quest"])
        {
            TextMeshProUGUI title = questlists[i].GetComponentInChildren<TextMeshProUGUI>();
            Image image = questlists[i].GetComponentInChildren<Image>();

            title.text = quest["title"].ToString();
            //image.sprite = questlists[i].

            //그리고 어디 반영..? 아니야 그건 아니지.

            i++;
        }
        

    }
}