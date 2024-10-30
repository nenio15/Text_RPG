using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAction : InterAction
{
    private int setTurn = 0;
    public Action onActionReady;


    private BattleManager battleManager;

    public LayerMask whatIsTarget;
    private LivingEntity targetEntity;
    public Transform targetPos; // 임시 조치

    public Vector3 target;
    private NavMeshAgent agent;

    //public ParticleSystem hitEffect;
    //public AudioClip deathSound;
    //public AudioClip hitSound;

    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (true)
        {
            LivingEntity attactTarget = other.GetComponent<LivingEntity>();

            if (attactTarget != null)// && attactTarget == targetEntity)
            {
                //공격
                Vector3 hitPos = other.ClosestPoint(transform.position);
                Vector3 hitSurface = transform.position - other.transform.position;
                //attactTarget.OnDamage(1, hitPos, hitSurface);
                battleManager.Interaction(gameObject, other.gameObject);
            }

            //turnEnd = true;
            return;
        }
    }

    //액션 세팅
    public void SetAction(string name)
    {
        if(setTurn >= 3) { Debug.Log("turn over. remove before action"); return; }

        //이거 세팅을 여기서 해야할까.. 굳이..?
        //json리딩도 있어.. 그건 나중에 구현.
        battleAction action = new battleAction();
        action.name = name;
        action.index = setTurn++;
        action.img = name; //변경 필...
        action.type = "normal";
        action.damage = 1.0f;
        action.percent = 0.7f;

        base.OnSetAction(action);
        onActionReady();
    }

    //액션 취소
    public void RemoveAction(int index)
    {
        //정렬 후 삭제
        if (index < 2)
        {
            //1 또는 2에서 내리기
            for (int i = index; i < 2; i++)
                actions[i] = actions[i + 1];

            actions[2] = new battleAction();
            actions[2].index = 2;
        }
        else // 3. 마지막 액션 삭제.
        {
            actions[index] = new battleAction();
            actions[index].index = 0;
        }

        setTurn--;
    }

    //턴 종료 후 리셋
    public void ResetAction()
    {
        base.OnEnable();
        setTurn = 0;
        onActionReady();
    }

    private void SetTargetPosition()
    {
        //대충 클릭으로 위치. 이거는 필요없겠네
        //target = 
    }

    public void SetAgentPosition()
    {
        //agent.speed =
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }

    //어그로 대상에게 접근하기.
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
}
