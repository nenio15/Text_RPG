using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class townEntry : MonoBehaviour
{
    /*
     2024-01-04
     자 새해가 밝았습니다. 오랜만이네요

    타운은 무엇인가?
    마을입니다.
    글면 여기는 무엇을 읽는가?

    마을의 초입 묘사 + 선택지입니다.
    마을로 들어가던가, 나가서 맵으로 가던가 이죠.

    두가지만 건들면 됩니다.
    기존 오브제를 건들여야되고요.
     
     */

    // 묘사는 매니저를 통해야합니다. 매니저는 체인저를 통하죠.
    private Textchanger tchanger = new Textchanger();

    void Entry(string jmain, string jsub)
    {
        //jmain jsub로 해당 마을을 찾아봅시다. 여기서 묘사가 되요.
        tchanger.readScenarioParts(0, "town", "plain_town");

        //다음 btn은 알아서 찾거든요? 내비두고요
    }
}
