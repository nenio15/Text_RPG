using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeSetting
{
    public string type;
    public string name;
    public string state;
    public float value;
}

public class NarrativeSlot
{
    public string name;
    public string type;
    public string grade;
    public string img;

    public string comment;
    public string describe;

    public NarrativeSetting[] effect;
    public NarrativeSetting[] condition;
}


public class NarrativeSlotUi : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI text;
    public NarrativeSlot narrativeSlot;
    private Outline outline;

    public int index;
    //public bool equipped = false; �����ϸ� ������â�� ���� ¥��.

    private void Awake()
    {
        //outline = GetComponent<Outline>();


    }

    public void Set()
    {
        //���� ��� ����.
        icon.sprite = Resources.Load<Sprite>("Picture/Narrative/" + narrativeSlot.img);
        text.text = narrativeSlot.name;
    }

    public void Clicked()
    {
        //Debug.Log(narrativeSlot.describe + narrativeSlot.comment + " i : " + index);

        //��� ��������� ������ ������ �����.? ������ ���� ������ �� ��� �ִµ�. ���� ����?
        NarrativeList.Instance.Selected(index); 
    }



}
