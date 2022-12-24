using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{

    public GameObject[] button;

    void ShowSelection()    //선택지 종이 보이기. 
    {
        /*
         * 키워드 오브제로부터 호출된다.
         * 주 기능은 오브제에 따른 선택지를 주는것이다. 그리고 선택지에 따른 리액션. 선택지는 따로 분할...안 할거다 아마도
         * 
         */

        //this.tranform();
    }
    
    void OnClick()
    {
        //예.. 그겁니다. 어느 버튼이든 눌리면 다 비활성화되고, 페이퍼 내려가고 그걸 말하고 싶은 겁니다.
        //다만 이것의 문제는 '어느 버튼'이 눌렸느냐 라는 문제이죠. 이거는 연구가 필요합니다.
    }

}
