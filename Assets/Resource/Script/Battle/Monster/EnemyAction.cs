using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
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


    //스킬 세팅 관련 변수
    TextAsset skill_route;
    JObject skill_jroot;
    List<JToken> skillsets = new List<JToken>();
    //string[] skills = { "base_attack", "base_dodge", "strong_attack" };

    void Start()
    {
        //skill_route = Application.dataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/Skill.json";
        skill_route = Resources.Load<TextAsset>("Text/Battle/Monster/" + enemyHealth.nickname);
        player = GameObject.Find("Player").gameObject; // 수정 필요.
        
        enemyHealth = GetComponent<EnemyHealth>();
        tr = GetComponent<RectTransform>();
        target = player.GetComponent<RectTransform>(); // 변경 필.
        UiImage = skill.GetComponentsInChildren<Image>();

        //Debug.Log(skill_route.text);
        skill_jroot = JObject.Parse(skill_route.text);

        foreach (JToken skill in skill_jroot["Attack"])
            skillsets.Add(skill);

        //for(int i = 0; i < skillsets.Count; i++) Debug.Log(skillsets[i]);

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
        //액션 세팅. skillsets중 랜덤 하나 (ai 적용 x).
        string cur_skill = skillsets[UnityEngine.Random.Range(0, skillsets.Count)].ToString();
        Debug.Log("ready cur_skill _ from enemy : " + cur_skill);
        //enemyHealth.enemy.Skill = skill;
        BattleAction tmp = JsonUtility.FromJson<BattleAction>(cur_skill); //json양식을 클래스 형식으로 변형.
        OnSetAction(tmp);
        skill.GetComponent<Image>().sprite = Resources.Load<Sprite>("Picture/Skill/" + tmp.img);
    }


    private void OnTriggerStay2D(Collider2D other)
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
        //Debug.Log(player.name);
        onSystemAction(gameObject, player); //call BattleManager.Interaction


        //tr.position = new Vector3(-300, 300);
        TurnSetUp(); // 매니저에게 전담 필요.
    }

    public void UpdateUi()
    {
        //스킬
        UiImage[0].sprite = Resources.Load<Sprite>("Picture/Skill/" + actions[turnSequence].img);

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
