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
using static UnityEditor.Progress;



public class Inventory : MonoBehaviour
{
    // <!-- items�� �Ϻκ���. itemslots�� �����Ǿ��ִ� �Լ����� �������� -->
    //subwindow�� setactive ���ø���. transform���� �ؾ��ҵ�. �׷��� start�� ù �ε��� ������.

    //[SerializeField] private ItemSlot[] items; //�̰��� �� �ʿ��� ���ΰ�. ������?
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private ItemSlotUi[] shoplist;
    [SerializeField] private GameObject desPanel;
    [SerializeField] private GameObject ShopPanel;
    public GameObject selected_item;
    //public itemData itemData;

    //Json ���� ����
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
    }

    public void UpdateAll()
    {
        UpdateList(itemslots);
        UpdateShop();
    }

    public void UpdateShop()
    {
        Debug.Log("update shoppppp");
        shoplist = ShopPanel.GetComponentsInChildren<ItemSlotUi>(true);
        for (int j = 0; j < shoplist.Length; j++)
            shoplist[j].type = "shopplayer";
        UpdateList(shoplist);
    }

    public void UpdateList(ItemSlotUi[] items)
    {
        /*
         * 1.json���� parse�Ѵ�.
         * �ش� list�� foreach�� �����Ͽ�, �� slots�� �����.
         * null�� �ƴҶ�����..? �ƴϾ� foreach���ȸ� update��Ų��.
         * �׷��� �Ѵ�.
         * �߰��� �����.. �װ� ���Կ��� �ݿ���Ű��, json���� �ݿ���Ų��.
         * 
         * update���õ� ���� �̱��� ����.
         */

        //��ȣ ���̱�
        for(int j = 0; j < items.Length; j++)
            items[j].index = j;


        int i = 0;
        string str = convertJson.MakeJson(inventory_route);
        jroot = JObject.Parse(str);
        itemTable = JsonUtility.FromJson<ItemTable>(jroot.ToString());

        //�κ��丮.json���� ������ �о �� slot�� �Ҵ�.
        foreach (Itemlist item in itemTable.item)
        {
            items[i].Set(item);
            i++;
        }

        /*
        //��ĭ�� tmp�� ä���
        for (; i < items.Length; i++)
        {
            Itemlist tmp = new Itemlist();
            items[i].Set(tmp);
        }
        */
    }

    private ItemSlot GetItemStack(ItemSlot item)
    {
        //������ ���̴� ������ ������ ������ â�� ���� ���.
        for(int i = 0; i < itemslots.Length; i++)
            if(itemslots[i].itemslot == item) //&& itemslots[i].itemslot.count) ��������
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
        if(getItem.type == "Consumption")
        {
            foreach(Itemlist item in itemTable.item)
                if(item.name == getItem.name) item.count += getItem.count;

            tableJson = JsonConvert.SerializeObject(itemTable);
            File.WriteAllText(inventory_route, tableJson.ToString());
            UpdateAll();
            return;
        }

        //�ƴҰ��.
        itemTable.item.Add(getItem);
        tableJson = JsonConvert.SerializeObject(itemTable); 
        File.WriteAllText(inventory_route, tableJson);
        UpdateAll();
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
                        Debug.Log("delete");
                        itemTable.item.Remove(dropItem);
                        UpdateAll();
                        return;
                    }
                }

            tableJson = JsonConvert.SerializeObject(itemTable);
            File.WriteAllText(inventory_route, tableJson.ToString());
            UpdateAll();
            return;
        }

        //�ƴҰ��.
        itemTable.item.Remove(dropItem);
        tableJson = JsonConvert.SerializeObject(itemTable);
        File.WriteAllText(inventory_route, tableJson);
        UpdateAll();
        return;

        //�κ��丮 ��ĭ�� ���� ���. �ѹ� ����. �̰Ŵ� ���߿� ����...
        //ThrowItem(item);
    }

    public void Selected(int index, string type)
    {
        if(type == "shopplayer")
        {
            OnItemClick(shoplist[index].gameObject);
            return;
        }

        desPanel.GetComponent<DescribePanel>().Set(itemslots[index].itemslot);
        OnItemClick(itemslots[index].gameObject);

        if (itemslots[index] == null) return;

        //ItemSlot cur_item = itemslots[index].itemslot;
        //����... panel���׼� �̰����Ÿ� �־���ϴµ�. �� ��Ҹ� ���� �޾�? �׳�?
        //panel�� �����ϴ� ���� ���� �ִ°� ������ ������... ��� �� �غ���
        //Debug.Log(cur_item.itemData.effect[0].name);
    }


    //�ӽ� �̸�. ���� �ʿ�...
    public void OnItemClick(GameObject item)
    {
        // ������ ���õ� �������� ���� ����
        if (selected_item != null)
        {
            DeselectItem(selected_item);
        }

        // ���� ���õ� ������ ����
        SelectItem(item);
        selected_item = item;
    }


    private void SelectItem(GameObject item)
    {
        // ���� ȿ�� ���� (��: ���� ����, �׵θ� �߰� ��)
        item.GetComponent<Outline>().enabled = true;
        //item.GetComponent<Image>().color = Color.yellow; // ���÷� ������ ��������� ����
    }

    private void DeselectItem(GameObject item)
    {
        // ���� ȿ�� ����
        item.GetComponent<Outline>().enabled = false;
        //item.GetComponent<Image>().color = Color.white; // �⺻ �������� ����
    }
}
