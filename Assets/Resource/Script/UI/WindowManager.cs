using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Localization.Platform.Android;
using UnityEngine.UI;
using UnityEditor.Tilemaps;
using System.Security.Policy;

public class WindowManager : MonoBehaviour
{
    //얘는 뭔 차이가 있느냐라...
    [SerializeField] private TextChanger textChanger;
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
    [SerializeField] private GameObject ShopPanel;
    public GameObject selected_item;

    //Json 관련 선언
    private string itemsheet;
    private string type;
    private string tableJson;
    private JObject jroot;
    private ItemTable itemTable;
    private string sheet_route;

    public static WindowManager Instance;
    public Inventory inventory;
    public PlayerHealth playerHealth;

    private void Awake()
    {
        Instance = this;
        itemslots = ShopPanel.GetComponentsInChildren<ItemSlotUi>();
        textChanger.onWindowAction += CallShop;
        inventory.UpdateShop();
        //UpdateList(itemslots);
    }

    public void CallShop(string shoptype, string route)
    {
        sheet_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Area/Town/" + route + ".json"; //여기 경로의 /Town을 고유명사로 해서 싹 확인을 하는가? 아니면 통일하는가?
        //첫 방문.
        if (!File.Exists(sheet_route)) CreateSheet(sheet_route, route);

        itemsheet = new ConvertJson().MakeJson(sheet_route);
        type = shoptype;
        UpdateList();
        
        //itemsheet = Resources.Load<TextAsset>("Text/Field/Building/Itemlist/" + route).ToString();
        //string str = Application.persistentDataPath + \ PlayerPrefs.GetString("Char_route") + \Area\Town/Townlist.json;
    }

    public void UpdateList()//ItemSlotUi[] itemslots)
    {
        //번호 붙이기itemslots 이거 여기에 둬야함? 초기값 설정이잖.
        for (int j = 0; j < this.itemslots.Length; j++)
        {
            itemslots[j].index = j;
            itemslots[j].type = "window";
        }

        int i = 0;
        //string str = convertJson.MakeJson(itemsheet);
        jroot = JObject.Parse(itemsheet);
        itemTable = JsonUtility.FromJson<ItemTable>(jroot[type].ToString());

        //인벤토리.json에서 아이템 읽어서 각 slot에 할당.
        foreach (Itemlist item in itemTable.item)
        {
            itemslots[i].Set(item);
            i++;
        }
        
        //빈칸은 스프라이트 null
        for(; i < itemslots.Length; i++)
        {
            //itemslot의 icon을 null로 변경. 하드코딩을 이렇게 해도 될지 ㅁ?ㄹ
            itemslots[i].gameObject.GetComponentsInChildren<Image>()[1].sprite = null;
        }

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

    //약식으로 아이템을 얻어 json에 갱신한다. 바로 json으로 가는게 맞는지는 잘 모르겠다..흠.
    public void AddItem(Itemlist getItem)
    {
        //Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());
        //중첩가능 아이템.
        if (getItem.type == "Consumption")
        {
            foreach (Itemlist item in itemTable.item)
                if (item.name == getItem.name) item.count += getItem.count;
            
            tableJson = JsonConvert.SerializeObject(itemTable);
            jroot[type] = JToken.Parse(tableJson);
            File.WriteAllText(sheet_route, jroot.ToString());
            UpdateList();
            return;
        }

        //json양식이 다름ㅇㅇ.
        itemTable.item.Add(getItem);
        tableJson = JsonConvert.SerializeObject(itemTable);
        jroot[type] = JToken.Parse(tableJson);
        File.WriteAllText(sheet_route, jroot.ToString());
        UpdateList();
        return;

        //인벤토리 빈칸이 없을 경우. 한번 묻기. 이거는 나중에 구현...
        //ThrowItem(item);
    }

    //아이템을 떨군다. 근데 이거 window도 쓴단말이지 참...
    public void DropItem(Itemlist dropItem)
    {
        //Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());
        //쌓일 수 있는 아이템.
        if (dropItem.type == "Consumption")
        {
            foreach (Itemlist item in itemTable.item)
                if (item.name == dropItem.name)
                {
                    item.count -= dropItem.count;
                    if (item.count <= 0)
                    {
                        itemTable.item.Remove(dropItem);
                        tableJson = JsonConvert.SerializeObject(itemTable);
                        jroot[type] = JToken.Parse(tableJson);
                        File.WriteAllText(sheet_route, jroot.ToString());

                        UpdateList();
                        return;
                    }
                }

            tableJson = JsonConvert.SerializeObject(itemTable);
            jroot[type] = JToken.Parse(tableJson);
            File.WriteAllText(sheet_route, jroot.ToString());
            UpdateList();
            return;
        }

        //아닐경우.
        itemTable.item.Remove(dropItem);
        tableJson = JsonConvert.SerializeObject(itemTable);
        jroot[type] = JToken.Parse(tableJson);
        File.WriteAllText(sheet_route, jroot.ToString());
        UpdateList();
        return;

        //인벤토리 빈칸이 없을 경우. 한번 묻기. 이거는 나중에 구현...
        //ThrowItem(item);
    }

    public void BuyFromShop()
    {
        Debug.Log("click buy " + selected_item.name);
        //selected
        //해당아이템이 복수. 갯수가 많을경우, 단일 구매의 경우도 구현필요. 현재는 묶음구매

        ItemSlot item = selected_item.GetComponent<ItemSlotUi>().itemslot;
        if(playerHealth.player_info.Money >= item.itemData.sell)
        {
            Debug.Log("you can buy");
            //playerHealth.player_info.Money -= item.itemData.sell;
            playerHealth.UpdateData("gold", (-1) * item.itemData.sell);
            Itemlist tmp = new Itemlist(item.itemData.name, item.itemData.type, item.count);
            inventory.AddItem(tmp);
            DropItem(tmp);
        }
    }

    public void SellToShop()
    {
        Debug.Log("sell something" + inventory.selected_item.name);
        ItemSlot item = inventory.selected_item.GetComponent<ItemSlotUi>().itemslot;
        Debug.Log("you selled");

        //playerHealth.player_info.Money += item.itemData.sell / 10; //판매금액을 따로 표기가 될련지... 흠좀무.
        playerHealth.UpdateData("gold", item.itemData.sell / 10);
        Itemlist tmp = new Itemlist(item.itemData.name, item.itemData.type, item.count);
        AddItem(tmp);
        inventory.DropItem(tmp);

    }

    public void Selected(int index)
    {
        if (itemslots[index] == null) return;
        OnItemClick(itemslots[index].gameObject);
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

    public void OnItemClick(GameObject item)
    {
        // 이전에 선택된 아이템의 강조 해제
        if (selected_item != null) DeselectItem(selected_item);
        // 새로 선택된 아이템 강조
        SelectItem(item);
        selected_item = item;
    }


    private void SelectItem(GameObject item)
    {
        item.GetComponent<Outline>().enabled = true;
    }

    private void DeselectItem(GameObject item)
    {
        item.GetComponent<Outline>().enabled = false;
    }
}
