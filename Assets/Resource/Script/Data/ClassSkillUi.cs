using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ClassSkillUi : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI text;
    //public ItemSlot itemslot;
    //private Outline outline;
    private CharacterMake characterMake = new CharacterMake();

    public int index = -1;
    public string class_name;
    //private Dictionary dictionary = new Dictionary();

    private void Awake()
    {
        //outline = GetComponent<Outline>();
        //charlistManager = FindObjectOfType<CharlistManager>();

    }

    public void Set(string name)
    {
        class_name = name;
        /*
        ItemSlot tmp = new ItemSlot();
        tmp.itemData = dictionary.SetItem(item.name, item.type);
        if (tmp.itemData == null) { return; } //Debug.Log("non item dictionary!!"); return; }
        tmp.isEquipment = (tmp.itemData.type != "Consumption") ? true : false;
        tmp.count = item.count;

        itemslot = tmp;
        //Debug.Log(itemslot.itemData.img);

        */

        icon.sprite = Resources.Load<Sprite>("Picture/Class/" + class_name);
        //Canvas.ForceUpdateCanvases();
    }

    /*
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
    */

    public void Clicked()
    {
        characterMake.ClassSelect(class_name);
        //switch문으로 어느 상위 스크립트.instance한테 selected되어야할지 필요함. 장비창에서도 인벤창 index로 검색하잖ㅇ..
        //Inventory.Instance.Selected(index); //이렇게 하면 상위 모듈 참조가 되는구나...

    }



}
