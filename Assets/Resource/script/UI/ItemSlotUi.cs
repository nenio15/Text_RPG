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
    //public bool equipped = false; �����ϸ� ������â�� ���� ¥��.

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
        //switch������ ��� ���� ��ũ��Ʈ.instance���� selected�Ǿ������ �ʿ���. ���â������ �κ�â index�� �˻����ݤ�..
        Inventory.Instance.Selected(index); //�̷��� �ϸ� ���� ��� ������ �Ǵ±���...
    }



}
