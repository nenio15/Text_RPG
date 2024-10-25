using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyAction : InterAction
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject skill;
    private Image[] UiImage;

    public float speed = 400.0f;
    private RectTransform tr;
    private RectTransform target;


    public EnemyHealth enemyHealth;
    public Action<GameObject, GameObject> onSystemAction;

    public LayerMask whatIsTarget;
    private LivingEntity targetEntity;
    //private NavMeshAgent

    public ParticleSystem hitEffect;


    //State state;
    string[] skills = { "Rock", "Scissors", "Paper" };

    void Start()
    {
        player = GameObject.Find("Player").gameObject; // 수정 필요.
        
        enemyHealth = GetComponent<EnemyHealth>();


        tr = GetComponent<RectTransform>();
        target = player.GetComponent<RectTransform>(); // 변경 필.
        UiImage = skill.GetComponentsInChildren<Image>();



        //첫 세팅
        base.OnEnable();
        TurnSetUp();
    }


    private bool hasTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.dead) return true;
            else return false;
        }
    }

    //턴 세팅
    private void TurnSetUp()
    {
        SwitchTurn(false);
        //사망확인 - . 일단 livingentity가 들고있음.

        //turnEnd = false;

        //자신의 디버프 확인



        //스킬 준비
        ReadyNewSkill();

        //UI 셋업
        UpdateUi();
        //enemyHealth.UpdateUi();
    }


    //다음 스킬 준비
    private void ReadyNewSkill()
    {
        string skill = skills[UnityEngine.Random.Range(0, 3)];
        //enemyHealth.enemy.Skill = skill;
        
        battleAction tmp = new battleAction(skill, 0, "attack", skill, 1.0f, 1.0f);
        OnSetAction(tmp);


    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(enemyHealth == null) return;

        //겹침
        if (!enemyHealth.dead && !turnEnd)
        {
            LivingEntity attactTarget = other.GetComponent<LivingEntity>();
            //Debug.Log(other.name);

            if (attactTarget != null)// && attactTarget == targetEntity)
            {
                //공격
                //Vector3 hitPos = other.ClosestPoint(transform.position);
                //Vector3 hitSurface = transform.position - other.transform.position;
                //attactTarget.OnDamage(1, hitPos, hitSurface);

                EnemyInteractive();
            }
            return;
        }
    }


    //어그로 대상에게 접근하기.
    IEnumerator UpdateRun()
    {
        float distance = float.MaxValue;

        for (; distance >= 20;)
        {
            //Debug.Log(tr + " " + target + " " + distance);

            distance = Vector3.Distance(transform.position, target.position);

            Vector3 move = new Vector3(target.position.x - tr.position.x, target.position.y - tr.position.y, 0).normalized * speed * Time.deltaTime;
            tr.position += move;


            yield return null;
        }

    }

    //상호작용
    public void EnemyInteractive()
    {
        SwitchTurn(true);
        StopCoroutine("UpdateRun");
        onSystemAction(gameObject, player); //될려나.


        //tr.position = new Vector3(-300, 300);
        TurnSetUp(); // 매니저에게 전담 필요.
    }

    public void UpdateUi()
    {
        //스킬
        UiImage[0].sprite = Resources.Load<Sprite>("Picture/" + actions[turnSequence].img);

        //턴
        string[] num = { "One", "Two", "Three" };
        UiImage[1].sprite = Resources.Load<Sprite>("Picture/" + num[turnSequence]);
    }


    /*
     * Nav Mesh Agent를 사용 가능하면 쓸것..
    private void Start()
    {
        state = State.Run;//Idle;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        target = Player.transform.position; //갱신도 잇어야함
        //agent.destination = target.transform.position;
    }
    private void Update()
    {
        if (state == State.Run) UpdateRun();
        else if (state == State.Attack) UpdateAttack();
    }

    private void UpdateRun()
    {
        float distance = Vector3.Distance(transform.position, target);
        Debug.Log(distance);

        if(distance <= 2)
        {
            state = State.Attack;
            //anim.SetTrigger("Attack");
        }

        agent.speed = 3.5f;
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }

    private void UpdateAttack()
    {
        agent.speed = 0;

        float distance = Vector3.Distance(transform.position, target);
        if (distance > 2)
        {
            state = State.Run;
            //anim.SetTrigger("Run");
        }

        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }
    */

}
