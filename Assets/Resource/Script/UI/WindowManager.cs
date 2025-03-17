using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Localization.Platform.Android;

public class WindowManager : MonoBehaviour
{
    //��� �� ���̰� �ִ��Ķ�...
    [SerializeField] private TextChanger textChanger;
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
        //onSystemAction(gameObject, player);
        textChanger.onWindowAction += CallShop;
        //inventory_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/Inventory.json";
        //UpdateList(itemslots);
    }

    public void CallShop(string shoptype, string route)
    {
        string per_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Area/Town/" + route + ".json"; //���� ����� /Town�� �������� �ؼ� �� Ȯ���� �ϴ°�? �ƴϸ� �����ϴ°�?
        //ù �湮.
        if (!File.Exists(per_route)) CreateSheet(per_route, route);

        itemsheet = new ConvertJson().MakeJson(per_route);
        type = shoptype;
        UpdateList(itemslots);
        //�ش� ����Ʈ���� ����, �Ǹŵ� ����Ʈ�� ����Ѵ�.
        //jroot = JObject.Parse(itemsheet);

        //itemsheet = Resources.Load<TextAsset>("Text/Field/Building/Itemlist/" + route).ToString();
        //string str = Application.persistentDataPath + \ PlayerPrefs.GetString("Char_route") + \Area\Town/Townlist.json;
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
        itemTable = JsonUtility.FromJson<ItemTable>(jroot[type].ToString());

        //�κ��丮.json���� ������ �о �� slot�� �Ҵ�.
        foreach (Itemlist item in itemTable.item)
        {
            items[i].Set(item);
            i++;
        }

        /*
        //��ĭ�� tmp�� ä���
        for(; i < items.Length; i++)
        {
            Itemlist tmp = new Itemlist();
            items[i].Set(tmp);
        }
        */

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

    public void BuyFromShop()
    {
        Debug.Log("click buy");
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

}
