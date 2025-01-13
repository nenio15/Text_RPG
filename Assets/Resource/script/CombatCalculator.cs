using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatCalculator : MonoBehaviour
{

    //[SerializeField] private GameObject attackEffect;
    [SerializeField] private GameObject targetDiceFrame;
    [SerializeField] private GameObject executorDiceFrame;
    [SerializeField] private Animator attackAnimator;


    CombatAdventage combatAdventage;

    public bool compete;

    private void Start()
    {
        combatAdventage = new CombatAdventage();
    }

    //경합
    public void Compete(GameObject executor, GameObject target, int turnStep)
    {
        //합 - 시전자 결정.
        if (MatchSwap(executor.GetComponent<InterAction>(), target.GetComponent<InterAction>(), turnStep))
        {
            GameObject tmp = executor;
            executor = target; 
            target = tmp;
        }

        /*
         * MatchSwap - 첫 시전.
         * MatchAdventage - 서사 + 합 우월성
         * 
         */


        //명중
        string hitting = Hit(executor, turnStep);
        //JudgeNarrative(executor.gameObject);

        //닷지
        hitting = Dodge(target, hitting);
        //JudgeNarrative(executor.gameObject);
        
        //적용
        ApplyResult(hitting, executor, target);

        //Time.timeScale = 0;
    }

    //합 승부
    public bool MatchSwap(InterAction executor, InterAction target, int turnStep)
    {
        //예외처리
        if(executor == null || target == null) return false;

        //한쪽이 아무 행동도 취하지 않는 경우
        if (target.actions[turnStep].name == null) return false;
        else if (executor.actions[turnStep].name == null) return true;

        string executorSkill = executor.actions[turnStep].type;
        string targetSkill = target.actions[turnStep].type;

        //Debug.Log("executor : " + executorSkill + " , targetPos : " +  targetSkill);

        //다이스 - $애니 + img 생성.
        int executorDice = UnityEngine.Random.Range(0, 20);
        int targetDice = UnityEngine.Random.Range(0, 20);

        //Debug.Log("Base dice : " + executorDice + " and target : " + targetDice);
        //안 겹치게 어케둠....
        Vector3 move = new Vector3(3, 3, 0);
        executorDiceFrame.transform.position = executor.transform.position + move;
        targetDiceFrame.transform.position = target.transform.position + move;
        Debug.Log(executor.transform.position + " " + target.transform.position);

        //animator 시동. 결과값 송출. 그리고 다음.
        Debug.Log("dice roll");
        executorDiceFrame.GetComponentInChildren<TextMeshProUGUI>().text = executorDice.ToString();
        targetDiceFrame.GetComponentInChildren<TextMeshProUGUI>().text = targetDice.ToString();

        //중간 프리징이 되나.. 이게.... 흠

        //합 우월성
        if (combatAdventage.Adventages.ContainsKey(executorSkill))
        {
            string[] tmp = combatAdventage.Adventages[executorSkill];
            foreach (string s in tmp)
            {
                if(s == targetSkill)
                {
                    //Debug.Log(executorSkill + " vs " + s + " is executor adventage");
                    executorDice++;
                }
            }

        }
        else if (combatAdventage.Adventages.ContainsKey(targetSkill)) // 타겟이 우위인 경우.
        {
            string[] tmp = combatAdventage.Adventages[targetSkill];
            foreach (string s in tmp)
            {
                if (s == executorSkill)
                {
                    //Debug.Log(targetSkill + " vs " + s + " is target adventage");
                    targetDice++;
                }
            }
        }

        //스킬 스탯 - 1.요구치 확인 2.스텝 업 확인

        //무기 보정치 - 장비

        executorDiceFrame.GetComponentInChildren<TextMeshProUGUI>().text = executorDice.ToString();
        targetDiceFrame.GetComponentInChildren<TextMeshProUGUI>().text = targetDice.ToString();

        //animator 시동.
        //executorDiceFrame.GetComponent<Animator>().enabled = true;
        //targetDiceFrame.GetComponent<Animator>().enabled = true;

        //Debug.Log("Total dice : " + executorDice + " and target : " + targetDice);
        if (executorDice >= targetDice) compete = true;

        //1대실패, 20대성공의 처리. 취급.

        //서사판정. 재굴림.
        float reroll = JudgeNarrative(executor.gameObject, target.gameObject, compete);
        if (reroll == 100.0f) return MatchSwap(executor, target, turnStep);


        //동일한 경우는 어떻게 취급하냐...
        //20다이스 + 보정치 합산 비교 - $애니 + img 생성
        if (compete) return false;
        else return true;
    }

    //명중 판정
    private string Hit(GameObject executor, int turnStep)
    {
        //스킬 명중률
        //executor의 명중 보조들 추가 (장비. 서사. 그리고 ?)
        float accurate = executor.GetComponent<InterAction>().actions[turnStep].accurate;
        float critical = executor.GetComponent<InterAction>().actions[turnStep].criticalProbability;

        float rate = UnityEngine.Random.Range(0, 100); //대충 이런데.. 확률 range이거 int? 흠. 
        //Debug.Log("hit rate : " + rate);


        //명중보다 낮으면
        if (rate >= accurate) { return "miss"; }
        else if (rate < critical) //크리가 터지면
        {
            return "critical";
        }
        else
        {
            return "hit";
        }        
    }

    //회피 판정
    private string Dodge(GameObject target, string hit)
    {
        //1.크리 -> 회피 -> 명중
        //2.명중 -> 회피/찰과상
        //3.미스 -> 그냥 미스.

        //회피율은 타겟의 기본 회피율 + 방어구 + 서사 + ?
        //기본 회피율도 써야겠네..

        float rate = UnityEngine.Random.Range(0, 100); 

        if (hit == "miss") return hit;

        if (rate < 70)
        {
            return hit;
        }
        else
        {
            return "dodge";
        }
        //return "abrasion"; // 찰과상 기준은 일단 버리자.
    }

    //최종 히트 결과값 송출
    private void ApplyResult(string type, GameObject executor, GameObject target)
    {
        int damage = 1;
        //크리티컬, 명중, 찰과상, 회피, 미스. - 를 표시하는 스타일이 다르겠네요~?
        switch (type)
        {
            case "abrasion":
                damage = (damage > 2) ? damage-- : damage;
                break;
            case "critical":
                damage++;
                break;
            case "hit":
                break;
            case "dodge":
                damage = 1;
                break;
            case "miss":
                damage = 1; //임시조치
                break;

        }
        Debug.Log("HITTYPE : " + type);
        
        Vector3 hitPos = target.GetComponent<Collider2D>().ClosestPoint(executor.transform.position);
        Vector3 hitSurface = executor.transform.position - target.transform.position;

        if(target.GetComponent<IEntityEffect>() != null ) target.GetComponent<IEntityEffect>().OnDamage(damage, hitPos, hitSurface);


        attackAnimator.gameObject.transform.position = hitPos;
        attackAnimator.SetTrigger("slash");

        //데미지 스킨
        ShowDamage(damage, target);

        //임의 세팅 1초
        Invoke(nameof(DiceMove), 1.0f);

    }


    public void ShowDamage(float damage, GameObject target)
    {
        GameObject prefab = Resources.Load<GameObject>("damageSkin");
        GameObject popup = Instantiate(prefab, target.transform);
        
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.0f), 0);
        popup.GetComponent<RectTransform>().anchoredPosition = pos;
        popup.GetComponent<DamagePopup>().Setup(damage);
    }

    private void DiceMove()
    {
        Debug.Log("dice moved");
        executorDiceFrame.transform.position = new Vector3(-10, -10, -10);
        targetDiceFrame.transform.position = new Vector3(-10, -10, -10);
    }

    //서사 적용
    public void ApplyNarrative(GameObject target, NarrativeSetting effect)
    {
        if (target.GetComponent<IEntityEffect>() == null) return;
        if (target.GetComponent<InterAction>() == null) return;

        IEntityEffect entityEffect = target.GetComponent<IEntityEffect>();
        InterAction action = target.GetComponent<InterAction>();

        switch (effect.type)
        {
            case "stat": entityEffect.OnStat(effect.name, effect.value, effect.state); break;
            case "buff": entityEffect.OnBuff(effect.name, effect.value, effect.state); break;
            case "adjust": action.OnNumericalAdjust(effect.name, effect.value, effect.state); break;
        }
    }

    private float SumValue(string state, float value)
    {
        switch (state)
        {
            case "none": return value;
            case "rate": return value;
        }
        return value;
    }

    //서로 서사가 간섭 우위 executor 우선순위. 다음 타겟
    private float JudgeNarrative(GameObject executor, GameObject target, bool compete)
    {
        NarrativeManager.instance.CallByCombat(executor, compete);
        float tmp = NarrativeManager.instance.MeddleInCombat(executor, compete);
        if (tmp > 0) { return tmp; }

        NarrativeManager.instance.CallByCombat(target, !compete);
        tmp = NarrativeManager.instance.MeddleInCombat(target, !compete);

        return tmp;
    }

}
