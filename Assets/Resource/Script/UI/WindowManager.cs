using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WindowManager : MonoBehaviour
{
    //��� �� ���̰� �ִ��Ķ�...

    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
    [SerializeField] private GameObject ShopPanel;

    //Json ���� ����
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
        //inventory_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/Inventory.json";
        //UpdateList(itemslots);
    }

    public void CallShop(string shoptype, string route)
    {
        itemsheet = Resources.Load<TextAsset>("Text/Field/Building/Itemlist/" + route).ToString();
        type = shoptype;
        //jroot = JObject.Parse(itemsheet);
        UpdateList(itemslots);
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
        for (int j = 0; j < items.Length; j++)
            items[j].index = j;


        int i = 0;
        //string str = convertJson.MakeJson(itemsheet);
        jroot = JObject.Parse(itemsheet);
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

            items[i].Set(item);
            i++;
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

    //Resources�� ������ �ȵ�. �׷��ٰ� �� �������� ����... ���Ǹ� ���ϴ°ɷ� ����. ��. -> persistent�� ���� �Ǳ��ϴµ� ����? ����..
    //������� �������� ��� json�� �����Ѵ�. �ٷ� json���� ���°� �´����� �� �𸣰ڴ�..��.
    public void AddItem(Itemlist dropItem)
    {
        //Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());
        //���� �� �ִ� ������.
        if (dropItem.type == "Consumption")
        {
            foreach (Itemlist item in itemTable.item)
                if (item.name == dropItem.name) item.count += dropItem.count;

            tableJson = JsonConvert.SerializeObject(itemTable);
            //File.WriteAllText(inventory_route, tableJson.ToString());
            UpdateList(itemslots);
            return;
        }

        //�ƴҰ��.
        itemTable.item.Add(dropItem);
        tableJson = JsonConvert.SerializeObject(itemTable);
        //File.WriteAllText(inventory_route, tableJson);
        UpdateList(itemslots);
        return;

        //�κ��丮 ��ĭ�� ���� ���. �ѹ� ����. �̰Ŵ� ���߿� ����...
        //ThrowItem(item);
    }

    //�̰� �ȴ�����...
    public void Selected(int index)
    {
        //desPanel.GetComponent<DescribePanel>().Set(itemslots[index].itemslot);
//Debug.Log(itemslots[index].itemslot.itemData.name);
        if (itemslots[index] == null) return;
        ItemSlot cur_item = itemslots[index].itemslot;
        
        //Debug.Log(cur_item.itemData.effect[0].name);

    }

}
