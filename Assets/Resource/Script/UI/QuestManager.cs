using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    /*
     * �ϴ�. json questlist�� ������.
     * �׸��� quest������ ��Ƴ��� ���� json�� �ʿ�.
     * �׸��� update����� �������� ����� ����. -> ��� ������ ������ ��ũ��Ʈ��... �Ƴ�.. �̰Ŵ� ������ ����. ���߿� ������ �����ϰԲ�.
     * �ǹ�. -> �׷� ������ ���� ����? ������.
     */

    [SerializeField] private GameObject[] questlists;

    //Json ���� ����
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
        //�� ����Ʈ���� �����ؼ� �����ϱ�.
        //questlists[j].GetComponentInChildren<TextMeshProUGUI>();
        
        
        int i = 0;
        string str = convertJson.MakeJson(questlist_route);
        jroot = JObject.Parse(str);
        //jinventory = jroot[k];

        //�κ��丮.json���� ������ �о �� slot�� �Ҵ�.
        foreach (JToken quest in jroot["quest"])
        {
            TextMeshProUGUI title = questlists[i].GetComponentInChildren<TextMeshProUGUI>();
            Image image = questlists[i].GetComponentInChildren<Image>();

            title.text = quest["title"].ToString();
            //image.sprite = questlists[i].

            //�׸��� ��� �ݿ�..? �ƴϾ� �װ� �ƴ���.

            i++;
        }
        

    }
}