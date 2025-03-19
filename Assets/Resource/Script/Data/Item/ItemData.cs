using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Itemlist
{
    public string name;
    public string type;
    public int count;

    public Itemlist(string name, string type, int count)
    {
        this.name = name;
        this.type = type;
        this.count = count;
    }

    public override bool Equals(object obj)
    {
        if (obj is Itemlist other)
        {
            return name == other.name && type == other.type && count == other.count;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (name, type, count).GetHashCode();
    }
}

[Serializable]
public class ItemTable
{
    public List<Itemlist> item;
}

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
    public string img = "";
    public string type;
    //public bool isequipment = false;
    public int sell;
    public int count;
    //[Header("Stacking")]
    //public bool stack = false;
    //public int max_statck;

    [Header("Effect")]
    public ItemAddition[] effect;
    public ItemAddition[] limit;


    //... �̷������� ����� ���ƾ��ϴ°� �´°�..

}

public class ItemSlot
{
    public itemData itemData;
    public int count = 1;
    public bool isEquipment = false;
}