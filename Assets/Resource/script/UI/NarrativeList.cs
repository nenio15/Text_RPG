using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrativeList : MonoBehaviour
{
    //narrative ����â ������ ����

    //why this is divided?
    //[SerializeField] private NarrativeSlot[] narratives;
    [SerializeField] private List<NarrativeSlotUi> narrativeslots;
    [SerializeField] private GameObject desPanel;

    //Json ���� ����
    private string narrative_route;
    private static string k = "narrative";

    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;

    //private JToken jinventory;

    public static NarrativeList Instance;

    private void Awake()
    {
        Instance = this;
        narrative_route = Application.persistentDataPath + "/Info/NarrativeList.json";
        UpdateList();
    }

    public void UpdateList()
    {

        //��ȣ ���̱�
        for (int j = 0; j < narrativeslots.Count; j++)
            narrativeslots[j].index = j;


        int i = 0;
        string str = convertJson.MakeJson(narrative_route);
        jroot = JObject.Parse(str);

        //�� slot�� narrative �Ҵ�. ī�װ��� ���� ���ġ�� ���..
        foreach (JToken narrative in jroot[k])
        {
            NarrativeSlot tmp = new NarrativeSlot();
            tmp = dictionary.SetNarrative(narrative["name"].ToString(), narrative["type"].ToString());
            if (tmp == null) { Debug.LogError(i + " : �ش� narrative�� Dictionary�� �������� �ʽ��ϴ�."); continue; }
            //tmp.isEquipment = (tmp.itemData.type != "Consumption") ? true : false;
            //tmp.count = (int)item["count"];

            narrativeslots[i].narrativeSlot = tmp;
            narrativeslots[i].Set();
            i++;
        }
    }

    //�ӽ� ����.
    public void Selected(int index)
    {
        //desPanel.GetComponent<DescribePanel>().Set(narrativeslots[index].itemslot);
        TextMeshProUGUI[] texts = desPanel.GetComponentsInChildren<TextMeshProUGUI>();

        if (narrativeslots[index] == null) return;


        NarrativeSlot tmp = narrativeslots[index].narrativeSlot;
        texts[0].text = tmp.name;
        texts[1].text = tmp.type;
        texts[2].text = tmp.describe;
        texts[3].text = tmp.comment;
        
    }

}

