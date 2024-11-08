using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUi : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI text;
    public ItemSlot itemslot;
    private Outline outline;

    public int index;
    //public bool equipped = false; 장착하면 아이템창에 없어 짜샤.

    private void Awake()
    {
        outline = GetComponent<Outline>();


    }

    public void Set()
    {
        if (itemslot.isEquipment)
        {
            text.text = "";
        }
        else
        {
            text.text = itemslot.count.ToString();
        }

        icon.sprite = Resources.Load<Sprite>("Picture/Item/" + itemslot.itemData.img);
    }

    public void Clicked()
    {
        //switch문으로 어느 상위 스크립트.instance한테 selected되어야할지 필요함. 장비창에서도 인벤창 index로 검색하잖ㅇ..
        Inventory.Instance.Selected(index); //이렇게 하면 상위 모듈 참조가 되는구나...
    }



}
