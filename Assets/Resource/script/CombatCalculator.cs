using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CombatCalculator : MonoBehaviour
{
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private BattleManager battleManager;


    private void Start()
    {
        playerUiManager = FindObjectOfType<PlayerUiManager>();
        selectionManager = FindObjectOfType<SelectionManager>();
        battleManager = FindObjectOfType<BattleManager>();
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

        //명중

        //닷지

        //적용. living에서 통일.. 은 안될테고.
        Vector3 hitPos = target.GetComponent<Collider2D>().ClosestPoint(executor.transform.position);
        Vector3 hitSurface = executor.transform.position - target.transform.position;

        if (target.GetComponent<EnemyHealth>() != null) target.GetComponent<EnemyHealth>().OnDamage(1, hitPos, hitSurface);
        if (target.GetComponent<PlayerHealth>() != null) target.GetComponent<PlayerHealth>().OnDamage(1, hitPos, hitSurface);
        // npc

        //임시 적용 - 이래도 되지 않는가..
        //battleManager.IsTurnEnd();
    }

    //합 승부
    public bool MatchSwap(InterAction executor, InterAction target, int turnStep)
    {
        //예외처리
        if(executor == null || target == null) return false;

        //한쪽이 아무 행동도 취하지 않는 경우
        if (target.actions[turnStep].name == null) return false;
        else if (executor.actions[turnStep].name == null) return true;

        string executorSkill = executor.actions[turnStep].name;
        string targetSkill = target.actions[turnStep].name;

        Debug.Log("executor : " + executorSkill + " , targetPos : " +  targetSkill);

        //다이스 - $애니 + img 생성.
        int executorDice = UnityEngine.Random.Range(0, 20);
        int targetDice = UnityEngine.Random.Range(0, 20);

        //합 우월성

        //스킬 스탯 - 1.요구치 확인 2.스텝 업 확인

        //무기 보정치 - 장비


        Debug.Log("Total dice : " + executorDice + " and target : " + targetDice);
        
        //1대실패, 20대성공의 처리. 취급.


        //동일한 경우는 어떻게 취급하냐...
        //20다이스 + 보정치 합산 비교 - $애니 + img 생성
        if (executorDice >= targetDice) return false;
        else return true;

        /*
        //합을 바꿔야겠지 방식을..
        if (executorSkill == targetSkill)
        {
            return false;
        }
        else if ((executorSkill == "Scissors" && targetSkill == "Paper") ||
                     (executorSkill == "Rock" && targetSkill == "Scissors") ||
                     (executorSkill == "Paper" && targetSkill == "Rock"))
        {
            return false;
        }
        else {
            return true;
        }
        */
    }

}
