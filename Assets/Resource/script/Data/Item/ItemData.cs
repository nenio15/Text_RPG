using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemAddition
{
    public string type; //�Լ�?
    public string name;
    public float value;
}

//�̰�.. ��.... ��������? ���� ����� �������� ���� ����°� �ƴ��̻�... ����? ���� ������ ������ �����̶�..
//[CreateAssetMenu(fileName = "Itme", menuName = "New Item")] : ScriptableObject
[System.Serializable]
public class itemData
{
    [Header("INFO")]
    public string name;
    public string detail;
    public string img;
    public string type;
    //public bool isequipment = false;
    public int sell;

    //[Header("Stacking")]
    //public bool stack = false;
    //public int max_statck;

    [Header("Effect")]
    public ItemAddition[] effect;
    public ItemAddition[] limit;


    //... �̷������� ����� ���ƾ��ϴ°� �´°�..

}
