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

        /*
         * MatchSwap - ù ����.
         * MatchAdventage - ���� + �� �����
         * 
         */


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

        //Debug.Log("executor : " + executorSkill + " , targetPos : " +  targetSkill);

        //���̽� - $�ִ� + img ����.
        int executorDice = UnityEngine.Random.Range(0, 20);
        int targetDice = UnityEngine.Random.Range(0, 20);

        //Debug.Log("Base dice : " + executorDice + " and target : " + targetDice);
        //�� ��ġ�� ���ɵ�....
        Vector3 move = new Vector3(3, 3, 0);
        executorDiceFrame.transform.position = executor.transform.position + move;
        targetDiceFrame.transform.position = target.transform.position + move;
        Debug.Log(executor.transform.position + " " + target.transform.position);

        //animator �õ�. ����� ����. �׸��� ����.
        Debug.Log("dice roll");
        executorDiceFrame.GetComponentInChildren<TextMeshProUGUI>().text = executorDice.ToString();
        targetDiceFrame.GetComponentInChildren<TextMeshProUGUI>().text = targetDice.ToString();

        //�߰� ����¡�� �ǳ�.. �̰�.... ��

        //�� �����
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
        else if (combatAdventage.Adventages.ContainsKey(targetSkill)) // Ÿ���� ������ ���.
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

        //��ų ���� - 1.�䱸ġ Ȯ�� 2.���� �� Ȯ��

        //���� ����ġ - ���

        executorDiceFrame.GetComponentInChildren<TextMeshProUGUI>().text = executorDice.ToString();
        targetDiceFrame.GetComponentInChildren<TextMeshProUGUI>().text = targetDice.ToString();

        //animator �õ�.
        //executorDiceFrame.GetComponent<Animator>().enabled = true;
        //targetDiceFrame.GetComponent<Animator>().enabled = true;

        //Debug.Log("Total dice : " + executorDice + " and target : " + targetDice);
        if (executorDice >= targetDice) compete = true;

        //1�����, 20�뼺���� ó��. ���.

        //��������. �籼��.
        float reroll = JudgeNarrative(executor.gameObject, target.gameObject, compete);
        if (reroll == 100.0f) return MatchSwap(executor, target, turnStep);


        //������ ���� ��� ����ϳ�...
        //20���̽� + ����ġ �ջ� �� - $�ִ� + img ����
        if (compete) return false;
        else return true;
    }

    //���� ����
    private string Hit(GameObject executor, int turnStep)
    {
        //��ų ���߷�
        //executor�� ���� ������ �߰� (���. ����. �׸��� ?)
        float accurate = executor.GetComponent<InterAction>().actions[turnStep].accurate;
        float critical = executor.GetComponent<InterAction>().actions[turnStep].criticalProbability;

        float rate = UnityEngine.Random.Range(0, 100); //���� �̷���.. Ȯ�� range�̰� int? ��. 
        //Debug.Log("hit rate : " + rate);


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

    //ȸ�� ����
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

    //���� ��Ʈ ����� ����
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

        if(target.GetComponent<IEntityEffect>() != null ) target.GetComponent<IEntityEffect>().OnDamage(damage, hitPos, hitSurface);


        attackAnimator.gameObject.transform.position = hitPos;
        attackAnimator.SetTrigger("slash");

        //������ ��Ų
        ShowDamage(damage, target);

        //���� ���� 1��
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

    //���� ����
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

    //���� ���簡 ���� ���� executor �켱����. ���� Ÿ��
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
