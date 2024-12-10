using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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

    [SerializeField] private CombatCalculator combatCalculator;

    private static NarrativeManager m_instance;
    private string narrative_route;
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private string narrative_list;
    private JObject jroot;

    private void Awake()
    {
        if(instance != this) Destroy(gameObject);
        narrative_route = Application.persistentDataPath + "/Info/NarrativeList.json";
        narrative_list = convertJson.MakeJson(narrative_route);
        jroot = JObject.Parse(narrative_list);
    }

    //서사의 종류. 1.패시브 2.전투옵션 3.선택지옵션 4.액티브


    //2.전투 옵션 함수들
    /*
     * 참 많이도 있네. 타겟에게 영향을 끼치는건 문제가 없는데. 상대한테 영향을 끼치는거. 필드에 영향을 끼치는거. 그런거 묘사가 힘든데 말이지. 불물얼음등의 필드효과?
     * 필드효과 생각하면 머리가 아찔해지네. 적당히 양식만 찾으면 나중에는 수학으로만 고민하면 되니깐 그나마 낫긴한데.
     * 흠. 이 지랄이 언제쯤 끝날까. 해피엔딩으로 끝날까 과연. private void OnDestroy() 파괴하라는데? ㅋㅋㅋㅋㅋㅋㅋ 
     * 
     * 1.effect json을 넘긴다. 그리고 manager가 적당히 처리한다. ex) target : enemy, limit : level : 5, effect : damage : 1. 
     * 
     * 흐름도가 필요하겠네 이거. 
     * 
     * 인출
     * 타겟 : 적
     * 효과 : 데미지, 디버프.
     * 저 효과를 읽는게 오래 걸리지 않겠느냐.
     * [effect] in. [type] == "...", [name] == ",,,", [value] * [state]. damage?
     * 명령어 해석기. 가 있으면 편히 하겠지. 왜냐면 그거는 manager의 일은 아니잖. 근데 그렇게 따지면 매니저가 알아야 할까?
     * 알 필요는 없지만. 간섭의 권한 문제 때문에 고민인거였지. 그도그럴게 여기서도 건들고. 저기서도 건들면 좀 그렇잖.
     * 근데 매니저는 원래 간섭 안해. 타겟에.
     * 컴뱃이 원래 직접적인 간섭이 있지.
     * 컴뱃에 '추가'하기에는 좀 애매한 감이 없지않아있긴한데.
     * 그러면 추가적인 hit를 컴뱃에 넣는것도 나쁘진 않지. 
     * 
     */

    private bool CheckCondition(NarrativeSetting setting, GameObject target)
    {
        switch (setting.name)
        {
            // 체력 필요(요구)치.
            case "hp":
                if(target.GetComponent<LivingEntity>().health <= setting.value) return true;
                return false;
            case "mp":
                break;

            // 사용 횟수 제한은 변수를 다시금 생각해보고 짤 것.
            case "use":
                return true;

            //case "turn":

              
        }


        //위에 해당하는 조건이 없다면 참 반환
        return true;

    }

    public void CallByStageSet()
    {

    }


    //type : battle. 한 웨이브 종료/시작 시 물음
    public void CallByManager(GameObject target, BattleManager battleManager)
    {

        if (target.GetComponent<InterAction>() == null) return;

        //턴 단위 처리(임시)
        if (battleManager.turnSequence != 0)
        {
            foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
                ResetStack("turn", narrative);
            return;
        }

        foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
        {
            //새로운 웨이브시
            ResetStack("wave", narrative);
            
            //manager해당 서사만 읽기
            if (narrative.type != "battle") continue;

            bool condition_clear = true;
            foreach (NarrativeSetting con in narrative.condition)
            {
                //모든 조건 부합 확인
                if (!CheckCondition(con, target)) condition_clear = false;


                //웨이브 조건
                if (con.name == "turn" && con.type == "wave")
                    if ((battleManager.turnWave % con.value) != 0) { condition_clear = false; }
                //Debug.Log("checkingg");
            }
            //이거는 유효적임.
            if (condition_clear && !LimitUse(narrative))
            {
                if (narrative.effect == null || target == null) return;

                PopupNarrative(target, narrative);
                foreach (NarrativeSetting effect in narrative.effect) 
                    combatCalculator.ApplyNarrative(target, effect);
                //Debug.Log("active : " + narrative.name);
                
            }
        }
        
    }

    //type : combat. 한 합에서 물음
    public void CallByCombat(GameObject target, bool compete)
    {
        //SetOwnNarrativeRoute(target);

        //플레이어나 그런놈들에게 값 주는것. 여기서 가는 반영이 그 합에 반영될지가 제일 의문.
        //맑은 하늘 - 타겟 들고 있다 - 합을 하면서 환경을 매니저에게서? 받는다. 환경이 맑다. 타겟에게 보정치를 준다. -> accurate. 버프. 턴은 0. '이번만'

    }

    //type : dice. 한 합에서 물음
    public float MeddleInCombat(GameObject target, bool compete)
    {
        if (target.GetComponent<InterAction>() == null) return 0.0f;

        foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
        {
            if (narrative.type != "dice") continue;

            bool condition_clear = true;
            //조건 확인.
            foreach (NarrativeSetting con in narrative.condition)
            {
                //조건이 하나라도 부합하지 않을시, 다음 서사로 넘긴다.
                if (!CheckCondition(con, target)) condition_clear = false;

                //실패조건에서 이겼으면 조건 미달성.
                if (con.name == "compete")
                    if (con.state == "fail" && compete) condition_clear = false;
                //Debug.Log("checkingg");
            }

            //10퍼 확률로 100.0f 반환. 아니면 0.0f 반환.
            if (condition_clear && !LimitUse(narrative))
            {
                //Debug.Log(narrative.battle_use + narrative.overlap_use.ToString());
                //Debug.Log(target.GetComponent<InterAction>().narratives[i].stack + " or " + narrative.stack);

                PopupNarrative(target, narrative);
                //Debug.Log("active : " + narrative.name); 
                return 100.0f;
            }

        }

        //신의 보살핌 - 합 실패 - 타겟이 든다 - 일정확률로 굴린다. - 성공시. 값 100을 돌린다.

        //100이면 무조건. 999면 뭐 다시 리롤. 그런 느낌으로.
        return 0.0f;
    }

    //서사 팝업 임시 생성 후 파괴
    private void PopupNarrative(GameObject target, NarrativeSlot narrative)
    {
        Debug.Log("ldajfljrlk");
        //프리팹 지정
        GameObject prefab = Resources.Load<GameObject>("ribbon");
        Vector3 pos = new Vector3(1, 0.5f, 0); // z좌표를 넣어 말어..

        //인스턴트 생성
        GameObject tmp;
        tmp = Instantiate(prefab, target.transform);
        //player 따로는 일단 미구현 tmp = Instantiate(prefab, new Vector3(x, x, 0);
        tmp.GetComponent<RectTransform>().anchoredPosition = pos;

        TextMeshProUGUI[] text = tmp.GetComponentsInChildren<TextMeshProUGUI>();
        text[0].text = narrative.name;
        text[1].text = narrative.describe;

        //파괴/
        Destroy(tmp, 5f);
    }
    private void SetOwnNarrativeRoute(GameObject target)
    {
        //narrative_route = Application.persistentDataPath + "/Info/NarrativeList.json";
        string route = Application.persistentDataPath + "/Info/NarrativeList.json";

        //각 분류를 어떻게 하면 좋을지 모르겠네.. 흠좀무.
        if(target.name == "Player") route = Application.persistentDataPath + "/Info/NarrativeList.json";
        else if(target.name == "Enemy111") route = Resources.Load<TextAsset>("/Text/Batte/Monster" + target.name).ToString();
        //else if(target.GetComponent<EnemyHealth>() != null) target.GetComponent<EnemyHealth>().nickname = route;
        narrative_list = convertJson.MakeJson(route);
        jroot = JObject.Parse(narrative_list);
    }

    private bool LimitUse(NarrativeSlot narrative)
    {
        //한 턴 중복 & 한 전투 & 한 웨이브. 사용 제한
        if (narrative.overlap_use) return true;
        if(narrative.battle_use >= narrative.max_battle_use) return true;
        if(narrative.turn_use >= narrative.max_turn_use) return true;

        narrative.battle_use++;
        narrative.turn_use++;
        narrative.overlap_use = true;

        if (narrative.can_stack) narrative.stack++;

        return false;
    }
    
    private void ResetStack(string type, NarrativeSlot narrative)
    {
        //type : battle, wave, turn
        if(type == "battle") //stageset
        {
            narrative.battle_use = 0;
            narrative.turn_use = 0;
            narrative.overlap_use = false;
            narrative.stack = 0; //스택은 여전히 애매한 개념이야.. 스택이 터지는 기준이 한 합이냐 한 웨이브냐 한 전투냐 .... 뭐 그런거.
        }
        else if(type == "wave") //manager
        {
            narrative.turn_use = 0;
            narrative.overlap_use = false;
        }
        else if(type == "turn") //combat //1턴인지 2턴인지 3턴인지 구분이 누가 가능한가? manager에서 불러와야함. ..
        {
            narrative.overlap_use = false;
        }

    }

}
