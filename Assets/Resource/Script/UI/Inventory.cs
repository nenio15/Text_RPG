using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Itemlist
{
    public string name;
    public string type;
    public int count;
}

[Serializable]
public class ItemTable
{
    public List<Itemlist> item;
}

public class Inventory : MonoBehaviour
{
    // <!-- items로 일부변경. itemslots로 지정되어있는 함수들이 남아있음 -->
    //subwindow도 setactive 세팅말고. transform으로 해야할듯. 그래야 start가 첫 로딩때 먹히지.

    //[SerializeField] private ItemSlot[] items; //이것이 왜 필요한 것인가. 흠좀무?
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
    [SerializeField] private GameObject ShopPanel;
    public GameObject selected_item;
    //public itemData itemData;

    //Json 관련 선언
    private string inventory_route;
    private string tableJson;
    //private static string k = "item";
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;
    private ItemTable itemTable;

    public static Inventory Instance;

    private void Awake()
    {
        Instance = this;
        inventory_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/Inventory.json";
        UpdateList(itemslots);
        TargetSlots(ShopPanel);
    }

    public void UpdateAll()
    {
        UpdateList(itemslots);
        TargetSlots(ShopPanel);
    }

    public void TargetSlots(GameObject parent)
    {
        ItemSlotUi[] tmp = parent.GetComponentsInChildren<ItemSlotUi>();
        UpdateList(tmp);
        //이걸로 가야할듯?
    }

    public void UpdateList(ItemSlotUi[] items)
    {
        /*
         * 1.json에서 parse한다.
         * 해당 list는 foreach로 접근하여, 각 slots에 끼운다.
         * null이 아닐때까지..? 아니야 foreach동안만 update시킨다.
         * 그렇게 한다.
         * 추가가 생기면.. 그건 슬롯에도 반영시키고, json에도 반영시킨다.
         * 
         * update관련도 아직 미구현 상태.
         */

        //번호 붙이기
        for(int j = 0; j < items.Length; j++)
            items[j].index = j;


        int i = 0;
        string str = convertJson.MakeJson(inventory_route);
        jroot = JObject.Parse(str);
        itemTable = JsonUtility.FromJson<ItemTable>(jroot.ToString());

        //인벤토리.json에서 아이템 읽어서 각 slot에 할당.
        foreach (Itemlist item in itemTable.item)
        {
            items[i].Set(item);
            i++;
        }

        /*
        //빈칸은 tmp로 채우기
        for (; i < items.Length; i++)
        {
            Itemlist tmp = new Itemlist();
            items[i].Set(tmp);
        }
        */
    }

    private ItemSlot GetItemStack(ItemSlot item)
    {
        //스택이 쌓이는 동일한 종류가 아이템 창에 있을 경우.
        for(int i = 0; i < itemslots.Length; i++)
            if(itemslots[i].itemslot == item) //&& itemslots[i].itemslot.count) 갯수제한
                 return itemslots[i].itemslot;

        return null;
    }

    private ItemSlotUi GetEmptySlot()
    {
        for (int i = 0; i < itemslots.Length; i++)
            if (itemslots[i].itemslot.itemData == null)
                return itemslots[i];

        return null;
    }

    //약식으로 아이템을 얻어 json에 갱신한다. 바로 json으로 가는게 맞는지는 잘 모르겠다..흠.
    public void AddItem(Itemlist dropItem)
    {
        //Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());
        //쌓일 수 있는 아이템.
        if(dropItem.type == "Consumption")
        {
            foreach(Itemlist item in itemTable.item)
                if(item.name == dropItem.name) item.count += dropItem.count;

            tableJson = JsonConvert.SerializeObject(itemTable);
            File.WriteAllText(inventory_route, tableJson.ToString());
            UpdateAll();
            return;
        }

        //아닐경우.
        itemTable.item.Add(dropItem);
        tableJson = JsonConvert.SerializeObject(itemTable); 
        File.WriteAllText(inventory_route, tableJson);
        UpdateAll();
        return;

        //인벤토리 빈칸이 없을 경우. 한번 묻기. 이거는 나중에 구현...
        //ThrowItem(item);
    }

    public void Selected(int index)
    {
        desPanel.GetComponent<DescribePanel>().Set(itemslots[index].itemslot);
        OnItemClick(itemslots[index].gameObject);

        if (itemslots[index] == null) return;

        //ItemSlot cur_item = itemslots[index].itemslot;
        //대충... panel한테서 이거저거를 주어야하는데. 각 요소를 내가 받아? 그냥?
        //panel을 관리하는 놈이 따로 있는게 편하지 않을까... 고민 좀 해볼게
        //Debug.Log(cur_item.itemData.effect[0].name);
    }

    //임시 이름. 변경 필요...
    public void OnItemClick(GameObject item)
    {
        // 이전에 선택된 아이템의 강조 해제
        if (selected_item != null)
        {
            DeselectItem(selected_item);
        }

        // 새로 선택된 아이템 강조
        SelectItem(item);
        selected_item = item;
    }

    private void SelectItem(GameObject item)
    {
        // 강조 효과 적용 (예: 배경색 변경, 테두리 추가 등)
        item.GetComponent<Outline>().enabled = true;
        //item.GetComponent<Image>().color = Color.yellow; // 예시로 배경색을 노란색으로 변경
    }

    private void DeselectItem(GameObject item)
    {
        // 강조 효과 해제
        item.GetComponent<Outline>().enabled = false;
        //item.GetComponent<Image>().color = Color.white; // 기본 배경색으로 변경
    }
}
