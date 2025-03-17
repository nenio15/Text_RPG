using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Localization.Platform.Android;

public class WindowManager : MonoBehaviour
{
    //얘는 뭔 차이가 있느냐라...
    [SerializeField] private TextChanger textChanger;
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
    [SerializeField] private GameObject ShopPanel;
    
    //Json 관련 선언
    private string itemsheet;
    private string type;
    private string tableJson;
    //private static string k = "item";
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;
    private ItemTable itemTable;

    
    private void Awake()
    {
        itemslots = ShopPanel.GetComponentsInChildren<ItemSlotUi>();
        //onSystemAction(gameObject, player);
        textChanger.onWindowAction += CallShop;
        //inventory_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/Inventory.json";
        //UpdateList(itemslots);
    }

    public void CallShop(string shoptype, string route)
    {
        string per_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Area/Town/" + route + ".json"; //여기 경로의 /Town을 고유명사로 해서 싹 확인을 하는가? 아니면 통일하는가?
        //첫 방문.
        if (!File.Exists(per_route)) CreateSheet(per_route, route);

        itemsheet = new ConvertJson().MakeJson(per_route);
        type = shoptype;
        UpdateList(itemslots);
        //해당 리스트에서 구매, 판매된 리스트를 기록한다.
        //jroot = JObject.Parse(itemsheet);

        //itemsheet = Resources.Load<TextAsset>("Text/Field/Building/Itemlist/" + route).ToString();
        //string str = Application.persistentDataPath + \ PlayerPrefs.GetString("Char_route") + \Area\Town/Townlist.json;
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
        for (int j = 0; j < items.Length; j++)
            items[j].index = j;


        int i = 0;
        //string str = convertJson.MakeJson(itemsheet);
        jroot = JObject.Parse(itemsheet);
        itemTable = JsonUtility.FromJson<ItemTable>(jroot[type].ToString());

        //인벤토리.json에서 아이템 읽어서 각 slot에 할당.
        foreach (Itemlist item in itemTable.item)
        {
            items[i].Set(item);
            i++;
        }

        /*
        //빈칸은 tmp로 채우기
        for(; i < items.Length; i++)
        {
            Itemlist tmp = new Itemlist();
            items[i].Set(tmp);
        }
        */

    }

    private ItemSlot GetItemStack(ItemSlot item)
    {
        //스택이 쌓이는 동일한 종류가 아이템 창에 있을 경우.
        for (int i = 0; i < itemslots.Length; i++)
            if (itemslots[i].itemslot == item) //&& itemslots[i].itemslot.count) 갯수제한
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

    //Resources는 수정이 안됨. 그렇다고 다 넣을수는 없고... 재판매 안하는걸로 하지. 뭐. -> persistent로 빼면 되긴하는데 굳이? ㅇㅇ..
    //약식으로 아이템을 얻어 json에 갱신한다. 바로 json으로 가는게 맞는지는 잘 모르겠다..흠.
    public void AddItem(Itemlist dropItem)
    {
        //Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());
        //쌓일 수 있는 아이템.
        if (dropItem.type == "Consumption")
        {
            foreach (Itemlist item in itemTable.item)
                if (item.name == dropItem.name) item.count += dropItem.count;

            tableJson = JsonConvert.SerializeObject(itemTable);
            //File.WriteAllText(inventory_route, tableJson.ToString());
            UpdateList(itemslots);
            return;
        }

        //아닐경우.
        itemTable.item.Add(dropItem);
        tableJson = JsonConvert.SerializeObject(itemTable);
        //File.WriteAllText(inventory_route, tableJson);
        UpdateList(itemslots);
        return;

        //인벤토리 빈칸이 없을 경우. 한번 묻기. 이거는 나중에 구현...
        //ThrowItem(item);
    }

    public void BuyFromShop()
    {
        Debug.Log("click buy");
    }

    //이거 안눌릴걸...
    public void Selected(int index)
    {
        //desPanel.GetComponent<DescribePanel>().Set(itemslots[index].itemslot);
//Debug.Log(itemslots[index].itemslot.itemData.name);
        if (itemslots[index] == null) return;
        ItemSlot cur_item = itemslots[index].itemslot;
        
        //Debug.Log(cur_item.itemData.effect[0].name);

    }

    //첫 방문 폴더&파일 만들기
    private void CreateSheet(string route, string key)
    {
        string r = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Area/Town";
        //string per_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Area/Town/" + route + ".json";
        if(!Directory.Exists(r)) Directory.CreateDirectory(r);

        if (Resources.Load("Text/Field/Building/Itemlist/" + key) == null) Debug.LogError("that itemsheet don't exist");
        string str = Resources.Load<TextAsset>("Text/Field/Building/Itemlist/" + key).ToString();
        File.WriteAllText(route, str);
    }

}
