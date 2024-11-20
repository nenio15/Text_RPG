using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class CombatCalculator : MonoBehaviour
{
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private BattleManager battleManager;

    CombatAdventage combatAdventage;

    private void Start()
    {
        combatAdventage = new CombatAdventage();
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
        string hitting = Hit(executor, turnStep);
        //JudgeNarrative(executor.gameObject);

        //����
        hitting = Dodge(target, hitting);
        //JudgeNarrative(executor.gameObject);
        
        //����
        ApplyResult(hitting, executor, target);

        //Time.timeScale = 0;
    }

    //�� �º�
    public bool MatchSwap(InterAction executor, InterAction target, int turnStep)
    {
        //����ó��
        if(executor == null || target == null) return false;

        //������ �ƹ� �ൿ�� ������ �ʴ� ���
        if (target.actions[turnStep].name == null) return false;
        else if (executor.actions[turnStep].name == null) return true;

        string executorSkill = executor.actions[turnStep].type;
        string targetSkill = target.actions[turnStep].type;

        Debug.Log("executor : " + executorSkill + " , targetPos : " +  targetSkill);

        //���̽� - $�ִ� + img ����.
        int executorDice = UnityEngine.Random.Range(0, 20);
        int targetDice = UnityEngine.Random.Range(0, 20);

        Debug.Log("Base dice : " + executorDice + " and target : " + targetDice);

        //�� �����
        if (combatAdventage.Adventages.ContainsKey(executorSkill))
        {
            string[] tmp = combatAdventage.Adventages[executorSkill];
            foreach (string s in tmp)
            {
                if(s == targetSkill)
                {
                    Debug.Log(executorSkill + " vs " + s + " is executor adventage");
                    executorDice++;
                }
            }

        }
        else if (combatAdventage.Adventages.ContainsKey(targetSkill)) // Ÿ���� ������ ���.
        {
            string[] tmp = combatAdventage.Adventages[targetSkill];
            foreach (string s in tmp)
            {
                if (s == executorSkill)
                {
                    Debug.Log(targetSkill + " vs " + s + " is target adventage");
                    targetDice++;
                }
            }
        }

        //��ų ���� - 1.�䱸ġ Ȯ�� 2.���� �� Ȯ��

        //���� ����ġ - ���


        Debug.Log("Total dice : " + executorDice + " and target : " + targetDice);

        //1�����, 20�뼺���� ó��. ���.

        //��������.
        JudgeNarrative(executor.gameObject);

        //������ ���� ��� ����ϳ�...
        //20���̽� + ����ġ �ջ� �� - $�ִ� + img ����
        if (executorDice >= targetDice) return false;
        else return true;
    }

    private string Hit(GameObject executor, int turnStep)
    {
        //��ų ���߷�
        //executor�� ���� ������ �߰� (���. ����. �׸��� ?)
        float accurate = executor.GetComponent<InterAction>().actions[turnStep].accurate;
        float critical = executor.GetComponent<InterAction>().actions[turnStep].criticalProbability;

        float rate = UnityEngine.Random.Range(0, 100); //���� �̷���.. Ȯ�� range�̰� int? ��. 
        Debug.Log("hit rate : " + rate);


        //���ߺ��� ������
        if (rate >= accurate) { return "miss"; }
        else if (rate < critical) //ũ���� ������
        {
            return "critical";
        }
        else
        {
            return "hit";
        }        
    }

    private string Dodge(GameObject target, string hit)
    {
        //1.ũ�� -> ȸ�� -> ����
        //2.���� -> ȸ��/������
        //3.�̽� -> �׳� �̽�.

        //ȸ������ Ÿ���� �⺻ ȸ���� + �� + ���� + ?
        //�⺻ ȸ������ ��߰ڳ�..

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
        //return "abrasion"; // ������ ������ �ϴ� ������.
    }

    private void ApplyResult(string type, GameObject executor, GameObject target)
    {
        int damage = 1;
        //ũ��Ƽ��, ����, ������, ȸ��, �̽�. - �� ǥ���ϴ� ��Ÿ���� �ٸ��ڳ׿�~?
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
                damage = 1; //�ӽ���ġ
                break;

        }
        Debug.Log("HITTYPE : " + type);
        
        Vector3 hitPos = target.GetComponent<Collider2D>().ClosestPoint(executor.transform.position);
        Vector3 hitSurface = executor.transform.position - target.transform.position;

        if (target.GetComponent<EnemyHealth>() != null) target.GetComponent<EnemyHealth>().OnDamage(damage, hitPos, hitSurface);
        if (target.GetComponent<PlayerHealth>() != null) target.GetComponent<PlayerHealth>().OnDamage(damage, hitPos, hitSurface);
        //npc�� ��쵵 �߰�.
    }

    private float JudgeNarrative(GameObject target)
    {
        NarrativeManager.instance.CallByCombat(target);
        float tmp = NarrativeManager.instance.MeddleInCombat(target);
        Debug.Log(tmp);
        return tmp;
    }

}
