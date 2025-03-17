using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ItemSlotUi : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI text;
    public ItemSlot itemslot;
    private Outline outline;
    public string type;

    public int index = -1;
    //public bool equipped = false; 장착하면 아이템창에 없어 짜샤.
    private Dictionary dictionary = new Dictionary();

    private void Awake()
    {
        outline = GetComponent<Outline>();


    }

    public void Set(Itemlist item)
    {
        ItemSlot tmp = new ItemSlot();
        tmp.itemData = dictionary.SetItem(item.name, item.type);
        if (tmp.itemData == null) { return; } //Debug.Log("non item dictionary!!"); return; }
        tmp.isEquipment = (tmp.itemData.type != "Consumption") ? true : false;
        tmp.count = item.count;

        itemslot = tmp;
        //Debug.Log(itemslot.itemData.img);

        if (itemslot.isEquipment)
        {
            text.text = "";
        }
        else
        {
            text.text = itemslot.count.ToString();
        }

        LoadSprite("Picture/Item/" + itemslot.itemData.img);
        //icon.sprite = Resources.Load<Sprite>("Picture/Item/" + itemslot.itemData.img);
        //Canvas.ForceUpdateCanvases();
    }

    private void LoadSprite(string key)
    {
        Addressables.LoadAssetAsync<Sprite>(key).Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                icon.sprite = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to load sprite: " + key);
            }
        };
    }

    public void Clicked()
    {
        //Debug.Log(itemslot.itemData.name);
        if(index < 0) { Debug.Log("null index : itemslot"); return; }

        if (type == "window") WindowManager.Instance.Selected(index);
        else Inventory.Instance.Selected(index, type); 
    }



}
