using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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


        nickname.text = item.itemData.name;
        type.text = item.itemData.type;
        detail.text = item.itemData.detail;
        sell.text = item.itemData.sell.ToString();

        //value.text = item.item.value.ToString(); // ������ class�� ���� �ٲ�. ���߿� �߰���ų��.
        //limit.text = item.item.limit.ToString();

        usebtn.text = (item.isEquipment) ? "�����Ѵ�" : "����Ѵ�"; // �ѱ�... �ٱ��� ����.
    }

    //tmp ���
    public void LoadScene()
    {
        SceneManager.LoadScene("StartScene");
    }

}
