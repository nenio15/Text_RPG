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
        //방어처리(의미있음?)
        if(executor == null || target == null) return false;

        //target이 합을 맞출 수 없는 경우.
        if (target.actions[turnStep].name == null) return false;

        string executorSkill = executor.actions[turnStep].name;
        string targetSkill = target.actions[turnStep].name;

        Debug.Log("executor : " + executorSkill + " , target : " +  targetSkill);

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
    }

}
