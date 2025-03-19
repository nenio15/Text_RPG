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
    //��� �� ���̰� �ִ��Ķ�...
    [SerializeField] private TextChanger textChanger;
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
    [SerializeField] private GameObject ShopPanel;
    public GameObject selected_item;

    //Json ���� ����
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
        sheet_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Area/Town/" + route + ".json"; //���� ����� /Town�� �������� �ؼ� �� Ȯ���� �ϴ°�? �ƴϸ� �����ϴ°�?
        //ù �湮.
        if (!File.Exists(sheet_route)) CreateSheet(sheet_route, route);

        itemsheet = new ConvertJson().MakeJson(sheet_route);
        type = shoptype;
        UpdateList();
        
        //itemsheet = Resources.Load<TextAsset>("Text/Field/Building/Itemlist/" + route).ToString();
        //string str = Application.persistentDataPath + \ PlayerPrefs.GetString("Char_route") + \Area\Town/Townlist.json;
    }

    public void UpdateList()//ItemSlotUi[] itemslots)
    {
        //��ȣ ���̱�itemslots �̰� ���⿡ �־���? �ʱⰪ ��������.
        for (int j = 0; j < this.itemslots.Length; j++)
        {
            itemslots[j].index = j;
            itemslots[j].type = "window";
        }

        int i = 0;
        //string str = convertJson.MakeJson(itemsheet);
        jroot = JObject.Parse(itemsheet);
        itemTable = JsonUtility.FromJson<ItemTable>(jroot[type].ToString());

        //�κ��丮.json���� ������ �о �� slot�� �Ҵ�.
        foreach (Itemlist item in itemTable.item)
        {
            itemslots[i].Set(item);
            i++;
        }
        
        //��ĭ�� ��������Ʈ null
        for(; i < itemslots.Length; i++)
        {
            //itemslot�� icon�� null�� ����. �ϵ��ڵ��� �̷��� �ص� ���� ��?��
            itemslots[i].gameObject.GetComponentsInChildren<Image>()[1].sprite = null;
        }

    }

    private ItemSlot GetItemStack(ItemSlot item)
    {
        //������ ���̴� ������ ������ ������ â�� ���� ���.
        for (int i = 0; i < itemslots.Length; i++)
            if (itemslots[i].itemslot == item) //&& itemslots[i].itemslot.count) ��������
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

    //������� �������� ��� json�� �����Ѵ�. �ٷ� json���� ���°� �´����� �� �𸣰ڴ�..��.
    public void AddItem(Itemlist getItem)
    {
        //Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());
        //��ø���� ������.
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

        //json����� �ٸ�����.
        itemTable.item.Add(getItem);
        tableJson = JsonConvert.SerializeObject(itemTable);
        jroot[type] = JToken.Parse(tableJson);
        File.WriteAllText(sheet_route, jroot.ToString());
        UpdateList();
        return;

        //�κ��丮 ��ĭ�� ���� ���. �ѹ� ����. �̰Ŵ� ���߿� ����...
        //ThrowItem(item);
    }

    //�������� ������. �ٵ� �̰� window�� ���ܸ����� ��...
    public void DropItem(Itemlist dropItem)
    {
        //Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());
        //���� �� �ִ� ������.
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

        //�ƴҰ��.
        itemTable.item.Remove(dropItem);
        tableJson = JsonConvert.SerializeObject(itemTable);
        jroot[type] = JToken.Parse(tableJson);
        File.WriteAllText(sheet_route, jroot.ToString());
        UpdateList();
        return;

        //�κ��丮 ��ĭ�� ���� ���. �ѹ� ����. �̰Ŵ� ���߿� ����...
        //ThrowItem(item);
    }

    public void BuyFromShop()
    {
        Debug.Log("click buy " + selected_item.name);
        //selected
        //�ش�������� ����. ������ �������, ���� ������ ��쵵 �����ʿ�. ����� ��������

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

        //playerHealth.player_info.Money += item.itemData.sell / 10; //�Ǹűݾ��� ���� ǥ�Ⱑ �ɷ���... ������.
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

    //ù �湮 ����&���� �����
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
        // ������ ���õ� �������� ���� ����
        if (selected_item != null) DeselectItem(selected_item);
        // ���� ���õ� ������ ����
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
