using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager instance {
        get
        {
            if (m_instance == null) m_instance = FindObjectOfType<NarrativeManager>();
            return m_instance;
        }
    }

    private static NarrativeManager m_instance;


    private void Awake()
    {
        if(instance != this) Destroy(gameObject);
    }

    //서사의 종류. 1.패시브 2.전투옵션 3.선택지옵션 4.액티브




    //2.전투 옵션 함수들
    /*
     * 참 많이도 있네. 타겟에게 영향을 끼치는건 문제가 없는데. 상대한테 영향을 끼치는거. 필드에 영향을 끼치는거. 그런거 묘사가 힘든데 말이지. 불물얼음등의 필드효과?
     * 필드효과 생각하면 머리가 아찔해지네. 적당히 양식만 찾으면 나중에는 수학으로만 고민하면 되니깐 그나마 낫긴한데.
     * 
     */
    public void CallByStageSet()
    {

    }

    public void CallByManager(GameObject target)
    {
        //쓰러져도 다시한번. 타겟이 들고 있다. + 체력이 0 이하다.
        //오래가는것이 좋다. 타겟 들고 있다. + 매니저에게 wave가 몇 턴 째인지 확인한다. 타겟에게 영향을 끼친다.
        // a ? -> 타겟을 제외한 인원에게 영향을 끼친다. 는? 흠좀무. 이거는 명령을 내려야할텐데..

    }

    public void CallByCombat(GameObject target)
    {
        //플레이어나 그런놈들에게 값 주는것. 여기서 가는 반영이 그 합에 반영될지가 제일 의문.
        //맑은 하늘 - 타겟 들고 있다 - 합을 하면서 환경을 매니저에게서? 받는다. 환경이 맑다. 타겟에게 보정치를 준다. -> accurate. 버프. 턴은 0. '이번만'
        
    }

    public float MeddleInCombat(GameObject target)
    {
        //신의 보살핌 - 합 실패 - 타겟이 든다 - 일정확률로 굴린다. - 성공시. 값 100을 돌린다.

        //100이면 무조건. 999면 뭐 다시 리롤. 그런 느낌으로.
        return 0.0f;
    }

    
    /*
     * 1.사망시
     * 2.타격시
     * 3.피격시
     * 4.웨이브 체크(시작? 끝?)
     * 5.상대 사망시
     * 6.합(주사위) 굴림 시.
     * 7.
     * 
     * 자 과정은 어떻게 되지?
     * 1.조건을 물을 외부 스크립트에게 호출된다.(각 조건)
     * 2.해당 상황에 맞는 서사를 간추린다.
     * 3.서사들 중 조건이 부합한 녀석들을 활성화한다.
     * 
     * 쓰러져도. -> player.hit에서 호출 가능 / combatcalculator에서 데미지 미리 추산 호출 가능
     * 오래가는것 -> 웨이브. - battlemanager에서 리딩. + 서사를 지닌 객체 자체 턴 시작 / battlemanager 턴 시작 시 각 객체 확인
     * 장인정신 -> 서사 자체의 스택. - player.hit / combat damage.
     * 역전의용사 -> combatcalculator 다이스 롤 이후 호출 가능.
     */


}
