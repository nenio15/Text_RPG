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

    //����
    public void Compete(GameObject executor, GameObject target, int turnStep)
    {
        //�� - ������ ����.
        if (MatchSwap(executor.GetComponent<InterAction>(), target.GetComponent<InterAction>(), turnStep))
        {
            GameObject tmp = executor;
            executor = target; 
            target = tmp;
        }

        //����

        //����

        //����. living���� ����.. �� �ȵ��װ�.
        Vector3 hitPos = target.GetComponent<Collider2D>().ClosestPoint(executor.transform.position);
        Vector3 hitSurface = executor.transform.position - target.transform.position;

        if (target.GetComponent<EnemyHealth>() != null) target.GetComponent<EnemyHealth>().OnDamage(1, hitPos, hitSurface);
        if (target.GetComponent<PlayerHealth>() != null) target.GetComponent<PlayerHealth>().OnDamage(1, hitPos, hitSurface);
        // npc

        //�ӽ� ���� - �̷��� ���� �ʴ°�..
        //battleManager.IsTurnEnd();
    }

    //�� �º�
    public bool MatchSwap(InterAction executor, InterAction target, int turnStep)
    {
        //���ó��(�ǹ�����?)
        if(executor == null || target == null) return false;

        //target�� ���� ���� �� ���� ���.
        if (target.actions[turnStep].name == null) return false;

        string executorSkill = executor.actions[turnStep].name;
        string targetSkill = target.actions[turnStep].name;

        Debug.Log("executor : " + executorSkill + " , target : " +  targetSkill);

        //���� �ٲ�߰��� �����..
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
