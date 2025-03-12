using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IconUi : MonoBehaviour
{
    /*
     *  1.�� ��Ҹ� ���Ѵ�.
        2.Ŭ�� �� �̺�Ʈ�� �����Ѵ�.
        3.�̺�Ʈ ��κ��� ������ ���� ��ũ��Ʈ�� ���� �����Ѵ�.
        �� instance�� �ϴ� ��찡 ����ϴ�.
        ������ ������ ����.
        1.itemlist 2.classlist 3.equiptlist 4.skilllist

        �ƴϸ� �θ� Ŭ����. �ڽ� Ŭ���� ���.
        �������̽� ����� ���ҰŰ�����..?
        �԰��� �ٸ� �������� �����̴�.
        1.charlist 2.questlist 3.narrativelist 4.statbuttonlist
     */
    public Button button;
    public Image icon;
    public TextMeshProUGUI text;

    //public ItemSlot itemslot;
    public Outline outline;

    public int start_index = 0;
    public int index {  get; protected set; }


    protected virtual void OnEnable()
    {
        index = start_index;

        //turnEnd = true;
        //BattleAction[] act = { new BattleAction(0), new BattleAction(1), new BattleAction(2) };
        
    }

    //�ܺο��� ����.// ���� template���� ������ ����?
    public virtual void OnSet()
    {
        //���� ���� Ȯ��
        //battleAction = tmp;

        //�̹��� ��
        //icon.sprite = Resources.Load<Sprite>("Picture/Skill/" + battleAction.img);
        //text
    }

    //���⼭ ����� ������ ��������?
    public virtual void OnClick()
    {
        Debug.Log("base click : " + index);
        //battlemanager.Instance.Clicked(index);
    }

}
