using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    //public Equip equip;
    [SerializeField] private ItemSlotUi[] equipments;

    //Json ���� ����
    private string equipment_route;
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;


    void Awake()
    {
        equipment_route = Application.persistentDataPath + "/"+ PlayerPrefs.GetString("Char_route") + "/Info/Equipment.json";
        UpdateSet();
    }

    public void UpdateSet()
    {
        //�̰�... ������ �����ε� �̷��� ����ϳ�.
        for (int j = 0; j < equipments.Length; j++)
            equipments[j].index = j;

        int i = -1;
        string str = convertJson.MakeJson(equipment_route);
        jroot = JObject.Parse(str);

        foreach (JToken part in jroot["equip"]) //��� 8�����̶� foreach�� �ƴϿ��� �Ǳ��ϴµ�.. �길 �ٲٱ⵵ �׷���.
        {
            /*
            i++;
            ItemSlot tmp = new ItemSlot();
            tmp.itemData = dictionary.SetItem(part["name"].ToString(), part["type"].ToString());
            if (tmp.itemData == null) {  continue; } //Debug.Log(i + " : �ش� ����� Dictionary�� �������� �ʽ��ϴ�.");
            tmp.isEquipment = true;
            tmp.count = 1;

            equipments[i].itemslot = tmp;
            */
            i++;
            Itemlist tmp = JsonUtility.FromJson<Itemlist>(part.ToString());
            if(tmp.name != null)
                equipments[i].Set(tmp);
            
        }

    }


}
