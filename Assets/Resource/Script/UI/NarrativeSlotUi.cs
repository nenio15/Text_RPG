using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class NarrativeSetting
{
    public string type;
    public string name;
    public string state;
    public float value;
}

[Serializable]
public class NarrativeSlot
{
    public string name;
    public string type;
    public string grade;
    public string img;

    public string comment;
    public string describe;

    // 쥰내 길어서 혐이긴한데.... 따로 클래스로 빼자니 참...
    public int max_battle_use;
    public int battle_use;
    public int max_turn_use;
    public int turn_use;
    public bool overlap_use;
    public bool can_stack;
    public int stack;

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
    //public bool equipped = false; 장착하면 아이템창에 없어 짜샤.

    private void Awake()
    {
        //outline = GetComponent<Outline>();


    }

    public void Set()
    {
        //아직 경로 없음.
        icon.sprite = Resources.Load<Sprite>("Picture/Narrative/" + narrativeSlot.img);
        text.text = narrativeSlot.name;
    }

    public void Clicked()
    {
        //Debug.Log(narrativeSlot.describe + narrativeSlot.comment + " i : " + index);

        //얘는 상위모듈을 참조할 이유가 있을까나.? 어차피 내부 변수는 다 들고 있는데. 조건 때문?
        NarrativeList.Instance.Selected(index); 
    }



}
