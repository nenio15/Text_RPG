using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    public itemData itemData;
    public int count = 1;
    public bool isEquipment = false;
}

public class Inventory : MonoBehaviour
{
    //subwindow�� setactive ���ø���. transform���� �ؾ��ҵ�. �׷��� start�� ù �ε��� ������.

    //[SerializeField] private ItemSlot[] items;
    [SerializeField] private ItemSlotUi[] itemslots;
    [SerializeField] private GameObject desPanel;
    //public itemData itemData;

    //Json ���� ����
    private string inventory_route;
    private static string k = "item";
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;
    //private JToken jinventory;

    public static Inventory Instance;

    private void Awake()
    {
        Instance = this;
        inventory_route = Application.persistentDataPath + "/Info/Inventory.json";
        //Debug.Log(inventory_route);
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

        //�κ��丮.json���� ������ �о �� slot�� �Ҵ�.
        foreach (JToken item in jroot[k])
        {
            ItemSlot tmp = new ItemSlot();
            tmp.itemData = dictionary.SetItem(item["name"].ToString(), item["type"].ToString());
            if (tmp.itemData == null) { Debug.LogError(i + " : �ش� ITEM�� Dictionary�� �������� �ʽ��ϴ�."); continue; }
            tmp.isEquipment = (tmp.itemData.type != "Consumption") ? true : false;
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
        
        
        //����... panel���׼� �̰����Ÿ� �־���ϴµ�. �� ��Ҹ� ���� �޾�? �׳�?
        //panel�� �����ϴ� ���� ���� �ִ°� ������ ������... ���� �� �غ���

        //Debug.Log(cur_item.item.name);

    }

}