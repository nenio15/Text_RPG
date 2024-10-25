using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : InterAction
{
    private int setTurn = 0;
    public Action onActionReady;


    private BattleManager battleManager;

    public LayerMask whatIsTarget;
    private LivingEntity targetEntity;
    public Transform target; // �ӽ� ��ġ
    //private NavMeshAgent

    //public ParticleSystem hitEffect;
    //public AudioClip deathSound;
    //public AudioClip hitSound;

    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (true)
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
    public void SetAction(string name)
    {
        if(setTurn >= 3) { Debug.Log("turn over. remove before action"); return; }

        //�̰� ������ ���⼭ �ؾ��ұ�.. ����..?
        //json������ �־�.. �װ� ���߿� ����.
        battleAction action = new battleAction();
        action.name = name;
        action.index = setTurn++;
        action.img = name; //���� ��...
        action.type = "normal";
        action.damage = 1.0f;
        action.percent = 0.7f;

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

            actions[2] = new battleAction();
            actions[2].index = 2;
        }
        else // 3. ������ �׼� ����.
        {
            actions[index] = new battleAction();
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


    //��׷� ��󿡰� �����ϱ�.
    IEnumerator UpdateRun()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 20) { StopCoroutine(UpdateRun()); }

        while (distance >= 20)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, 0.5f);
            yield return new WaitForSeconds(0.001f);
        }
    }
}
