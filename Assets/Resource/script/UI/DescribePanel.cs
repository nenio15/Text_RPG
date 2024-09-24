using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescribePanel : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI type;
    public TextMeshProUGUI value;
    public TextMeshProUGUI limit;
    public TextMeshProUGUI detail;
    public TextMeshProUGUI sell;

    public TextMeshProUGUI usebtn;

    private void Awake()
    {
        
    }

    public void Set(ItemSlot item) // pos�� data��.
    {
        //�̵����� �ʿ��մϴ٤���������
        if(item == null) return;


        nickname.text = item.item.name;
        type.text = item.item.type;
        detail.text = item.item.detail;
        sell.text = item.item.sell.ToString();

        value.text = item.item.value.ToString(); // ������
        limit.text = item.item.limit.ToString();

        usebtn.text = (item.isEquipment) ? "�����Ѵ�" : "����Ѵ�"; // �ѱ�... �ٱ��� ����.
    }

}
