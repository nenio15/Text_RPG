using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAction : InterAction
{
    private int setTurn = 0;
    //ActionList���� ����.
    public Action onActionReady;


    private BattleManager battleManager;

    public LayerMask whatIsTarget;
    private LivingEntity targetEntity;
    public Transform targetPos; // �ӽ� ��ġ

    public Vector3 target;
    private NavMeshAgent agent;

    //public ParticleSystem hitEffect;
    //public AudioClip deathSound;
    //public AudioClip hitSound;

    //skillslotui�� ������ ���� instance
    public static PlayerAction Instance;

    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();

        Instance = this;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        //�̰� ������Ʈ�� ��¼��.,..
        //UpdateNarrative();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!turnEnd)
        {
            LivingEntity attactTarget = other.GetComponent<LivingEntity>();

            if (attactTarget != null)// && attactTarget == targetEntity)
            {
                //����
                Vector3 hitPos = other.ClosestPoint(transform.position);
                Vector3 hitSurface = transform.position - other.transform.position;
                //attactTarget.OnDamage(1, hitPos, hitSurface);
                battleManager.Interaction(gameObject, other.gameObject);
            }

            //turnEnd = true;
            return;
        }
    }

    //�׼� ����
    public void SetAction(BattleAction action)
    {
        if(setTurn >= 3) { Debug.Log("turn over. remove before action"); return; }

        //�̰� ������ ���⼭ �ؾ��ұ�.. ����..?
        //json������ �־�.. �װ� ���߿� ����.
        /*
        BattleAction action = new BattleAction();
        action.name = name;
        action.img = name; //���� ��...
        action.type = "normal";
        action.damage = 1.0f;
        action.accurate = 0.7f;
        */
        action.index = setTurn++;

        base.OnSetAction(action);
        onActionReady();
    }

    //�׼� ���
    public void RemoveAction(int index)
    {
        //���� �� ����
        if (index < 2)
        {
            //1 �Ǵ� 2���� ������
            for (int i = index; i < 2; i++)
                actions[i] = actions[i + 1];

            actions[2] = new BattleAction();
            actions[2].index = 2;
        }
        else // 3. ������ �׼� ����.
        {
            actions[index] = new BattleAction();
            actions[index].index = 0;
        }

        setTurn--;
    }

    //�� ���� �� ����
    public void ResetAction()
    {
        base.OnEnable();
        setTurn = 0;
        onActionReady();
    }

    private void SetTargetPosition()
    {
        //���� Ŭ������ ��ġ. �̰Ŵ� �ʿ���ڳ�
        //target = 
    }

    public void SetAgentPosition()
    {
        //agent.speed =
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }

    //��׷� ��󿡰� �����ϱ�.
    IEnumerator UpdateRun()
    {
        float distance = Vector3.Distance(transform.position, targetPos.position);
        if (distance < 20) { StopCoroutine(UpdateRun()); }

        while (distance >= 20)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos.position, 0.5f);
            yield return new WaitForSeconds(0.001f);
        }
    }

    public void UpdateNarrative(List<NarrativeSlotUi> nar)
    {
        for (int i = 0; i < nar.Count; i++)
            SetNarrative(nar[i].narrativeSlot);
        //for (int i = 0; i < NarrativeList.Instance.narrativeslots.Count; i++)
        //    SetNarrative(NarrativeList.Instance.narrativeslots[i].narrativeSlot);
    }
}
