using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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
    //subwindow�� setactive ���ø���. transform���� �ؾ��ҵ�. �׷��� start�� ù �ε��� ������.

    //[SerializeField] private ItemSlot[] items; //�̰��� �� �ʿ��� ���ΰ�. ������?
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
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
        UpdateList();
    }

    public void UpdateList()
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
        for(int j = 0; j < itemslots.Length; j++)
            itemslots[j].index = j;


        int i = 0;
        string str = convertJson.MakeJson(inventory_route);
        jroot = JObject.Parse(str);
        itemTable = JsonUtility.FromJson<ItemTable>(jroot.ToString());

        //�κ��丮.json���� ������ �о �� slot�� �Ҵ�.
        foreach (Itemlist item in itemTable.item)
        {
            /*
            ItemSlot tmp = new ItemSlot();
            tmp.itemData = dictionary.SetItem(item.name, item.type);
            if (tmp.itemData == null) { Debug.LogError(i + " : �ش� ITEM�� Dictionary�� �������� �ʽ��ϴ�."); continue; }
            tmp.isEquipment = (tmp.itemData.type != "Consumption") ? true : false;
            tmp.count = item.count;
            
            itemslots[i].itemslot = tmp;
            */

            itemslots[i].Set(item);
            i++;
        }
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
    public void AddItem(Itemlist dropItem)
    {
        //Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());
        //���� �� �ִ� ������.
        if(dropItem.type == "Consumption")
        {
            foreach(Itemlist item in itemTable.item)
                if(item.name == dropItem.name) item.count += dropItem.count;

            tableJson = JsonConvert.SerializeObject(itemTable);
            File.WriteAllText(inventory_route, tableJson.ToString());
            UpdateList();
            return;
        }

        //�ƴҰ��.
        itemTable.item.Add(dropItem);
        tableJson = JsonConvert.SerializeObject(itemTable); 
        File.WriteAllText(inventory_route, tableJson);
        UpdateList();
        return;

        //�κ��丮 ��ĭ�� ���� ���. �ѹ� ����. �̰Ŵ� ���߿� ����...
        //ThrowItem(item);
    }

    public void Selected(int index)
    {
        desPanel.GetComponent<DescribePanel>().Set(itemslots[index].itemslot);

        //Debug.Log(itemslots[index].itemslot.itemData.name);
        if (itemslots[index] == null) return;

        ItemSlot cur_item = itemslots[index].itemslot;


        //����... panel���׼� �̰����Ÿ� �־���ϴµ�. �� ��Ҹ� ���� �޾�? �׳�?
        //panel�� �����ϴ� ���� ���� �ִ°� ������ ������... ��� �� �غ���

        //Debug.Log(cur_item.itemData.effect[0].name);

    }

}
