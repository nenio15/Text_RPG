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
     *  1.각 요소를 지닌다.
        2.클릭 시 이벤트를 실행한다.
        3.이벤트 대부분은 본인의 상위 스크립트에 대해 실행한다.
        ㄴ instance로 하는 경우가 허다하다.
        종류는 다음과 같다.
        1.itemlist 2.classlist 3.equiptlist 4.skilllist

        아니면 부모 클래스. 자식 클래스 방식.
        인터페이스 기능이 편할거같은데..?
        규격이 다른 프리팹은 다음이다.
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

    //외부에서 세팅.// 변수 template같은 자유형 가능?
    public virtual void OnSet()
    {
        //내부 변수 확정
        //battleAction = tmp;

        //이미지 셋
        //icon.sprite = Resources.Load<Sprite>("Picture/Skill/" + battleAction.img);
        //text
    }

    //여기서 공통될 이유가 뭐가있지?
    public virtual void OnClick()
    {
        Debug.Log("base click : " + index);
        //battlemanager.Instance.Clicked(index);
    }

}
