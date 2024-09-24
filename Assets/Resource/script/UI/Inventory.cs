using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    public itemData item;
    public int count = 1;
    public bool isEquipment = false;
}

public class Inventory : MonoBehaviour
{
    //subwindow도 setactive 세팅말고. transform으로 해야할듯. 그래야 start가 첫 로딩때 먹히지.

    //[SerializeField] private ItemSlot[] items;
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
    //public itemData itemData;

    //Json 관련 선언
    private string inventory_route;
    private static string k = "item";
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;
    //private JToken jinventory;

    public static Inventory Instance;


    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        inventory_route = Application.dataPath + "/Resource/Text/Info/Inventory.json";

        UpdateList();
    }

    public void UpdateList()
    {
        /*
         * 1.json에서 parse한다.
         * 해당 list는 foreach로 접근하여, 각 slots에 끼운다.
         * null이 아닐때까지..? 아니야 foreach동안만 update시킨다.
         * 그렇게 한다.
         * 추가가 생기면.. 그건 슬롯에도 반영시키고, json에도 반영시킨다.
         * 
         */

        //번호 붙이기 ... 필요함? 흠.
        for(int j = 0; j < itemslots.Length; j++)
            itemslots[j].index = j;


        int i = 0;
        string str = convertJson.MakeJson(inventory_route);
        jroot = JObject.Parse(str);
        //jinventory = jroot[k];

        //인벤토리.json에서 아이템 읽어서 각 slot에 할당.
        foreach (JToken item in jroot[k])
        {
            ItemSlot tmp = new ItemSlot();
            tmp.item = dictionary.SetItem(item["name"].ToString(), item["type"].ToString());
            if (tmp.item == null) { Debug.LogError(i + " : 해당 ITEM의 Dictionary가 참조되지 않습니다."); continue; }
            tmp.isEquipment = (tmp.item.type != "Consumption") ? true : false;
            tmp.count = (int)item["count"];

            itemslots[i].itemslot = tmp;
            itemslots[i].Set();
            //itemslots[i].itemslot.item.name = item["name"].ToString();
            i++;
        }
    }

    public void Selected(int index)
    {
        desPanel.GetComponent<DescribePanel>().Set(itemslots[index].itemslot);

        Debug.Log(index);
        if (itemslots[index] == null) return;

        ItemSlot cur_item = itemslots[index].itemslot;
        
        
        //대충... panel한테서 이거저거를 주어야하는데. 각 요소를 내가 받아? 그냥?
        //panel을 관리하는 놈이 따로 있는게 편하지 않을까... 고민 좀 해볼게

        //Debug.Log(cur_item.item.name);

    }

}
