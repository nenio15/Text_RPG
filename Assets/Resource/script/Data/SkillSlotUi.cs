using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUi : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI text;
    public BattleAction battleAction;
    //private Outline outline;

    private void Awake()
    {
        

    }

    //세팅
    public void Set(BattleAction tmp)
    {
        //내부 변수 확정
        battleAction = tmp;

        //이미지 셋
        icon.sprite = Resources.Load<Sprite>("Picture/Skill/" + battleAction.img);

        //필요 마나..?
        text.text = "";

        /*
        if (battleAction.need.isEquipment)
        {
            text.text = "";
        }
        else
        {
            text.text = itemslot.count.ToString();
        }
        */
        
        
    }

    //마우스, 터치를 가져다 둘때도 설명을 표시.


    public void Clicked()
    {
        //player.GetComponent<PlayerAction>().SetAction(btnData.displayText.text);
        //Debug.Log(battleAction.img);
        PlayerAction.Instance.SetAction(battleAction);
    }

}