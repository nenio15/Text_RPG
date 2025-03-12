using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    //public Equip equip;
    [SerializeField] private ItemSlotUi[] equipments;

    //Json 관련 선언
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
        //이거... 어차피 고정인데 이렇게 써야하나.
        for (int j = 0; j < equipments.Length; j++)
            equipments[j].index = j;

        int i = -1;
        string str = convertJson.MakeJson(equipment_route);
        jroot = JObject.Parse(str);

        foreach (JToken part in jroot["equip"]) //얘는 8고정이라 foreach가 아니여도 되긴하는데.. 얘만 바꾸기도 그렇고.
        {
            /*
            i++;
            ItemSlot tmp = new ItemSlot();
            tmp.itemData = dictionary.SetItem(part["name"].ToString(), part["type"].ToString());
            if (tmp.itemData == null) {  continue; } //Debug.Log(i + " : 해당 장비의 Dictionary가 참조되지 않습니다.");
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
