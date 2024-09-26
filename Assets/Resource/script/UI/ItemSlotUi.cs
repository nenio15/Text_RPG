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
    public ItemSlot itemslot; //이거 형태는 아직도 모르겠네. 한번 만들고. 부셔봐야 알듯?
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

        icon.sprite = Resources.Load<Sprite>("Picture/Item/" + itemslot.item.img);
    }

    public void Clicked()
    {
        //ItemSlotUI. 너... 생각보다 인기쟁이다? index말고, ui_type이라는 enum관련 함수 하나 더 얻지 않을래?
        //switch문으로 어느 상위 스크립트.instance한테 selected되어야할지 말해줄게....
        //대충 여기서 panel을 부르고 싶으나... 어차피 하위 모듈이라 inventory에게 부탁하십시오.
        Inventory.Instance.Selected(index); //이렇게 하면 상위 모듈 참조가 되는구나...
    }



}
