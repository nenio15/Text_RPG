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
    public ItemSlot itemslot; //�̰� ���´� ������ �𸣰ڳ�. �ѹ� �����. �μź��� �˵�?
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

        icon.sprite = Resources.Load<Sprite>("Picture/Item/" + itemslot.item.img);
    }

    public void Clicked()
    {
        //ItemSlotUI. ��... �������� �α����̴�? index����, ui_type�̶�� enum���� �Լ� �ϳ� �� ���� ������?
        //switch������ ��� ���� ��ũ��Ʈ.instance���� selected�Ǿ������ �����ٰ�....
        //���� ���⼭ panel�� �θ��� ������... ������ ���� ����̶� inventory���� ��Ź�Ͻʽÿ�.
        Inventory.Instance.Selected(index); //�̷��� �ϸ� ���� ��� ������ �Ǵ±���...
    }



}
