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

    //����
    public void Set(BattleAction tmp)
    {
        //���� ���� Ȯ��
        battleAction = tmp;

        //�̹��� ��
        icon.sprite = Resources.Load<Sprite>("Picture/Skill/" + battleAction.img);

        //�ʿ� ����..?
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

    //���콺, ��ġ�� ������ �Ѷ��� ������ ǥ��.


    public void Clicked()
    {
        //player.GetComponent<PlayerAction>().SetAction(btnData.displayText.text);
        //Debug.Log(battleAction.img);
        PlayerAction.Instance.SetAction(battleAction);
    }

}