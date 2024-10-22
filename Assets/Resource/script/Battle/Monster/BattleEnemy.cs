using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
/*
[System.Serializable]
public class Enemy
{
    public int Level = 1;
    public int[] Hp = { 2, 2 };
    public int[] Mp = { 1, 1 };
    public int[] Stat = { 1, 1, 1, 1, 1, 1 };

    public string Type = "Goblin";
    public string Class = "Warrior";
    public string Weapon = "Club";
    public string Skill = "Rock";
}
*/

public class BattleEnemy : LivingEntity
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject self;
    [SerializeField] private GameObject skill;
    [SerializeField] public Character enemy = new Character(); //임시
    private BattleManager battleManager;

    public float speed = 400.0f;
    private RectTransform tr;
    private RectTransform target;
    //NavMeshAgent agent;

    //몬스터 상단의 표기 세팅
    [SerializeField] private GameObject myInfo;
    private TextMeshPro myTextMeshPro;
    private RectTransform myInfoTr;

    private Image[] UiImgae;

    public Animator anim;


    public LayerMask whatIsTarget;

    private LivingEntity targetEntity;
    //private NavMeshAgent

    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;

    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    //private Renderer enemyRenderer; // 흠좀무.

    private bool hasTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.dead) return true;
            else return false;
        }
    }

    enum State
    {
        Idle,
        Run,
        Attack
    }

    //State state;
    string[] skills = { "Rock", "Scissors", "Paper" };

    //세팅
    private void Awake()
    {
        player = GameObject.Find("Player").gameObject; // 수정 필요.
        self = gameObject;
        battleManager = FindObjectOfType<BattleManager>();

        tr = GetComponent<RectTransform>();
        target = player.GetComponent<RectTransform>(); // 변경 필.

        
        //hp bar text setting
        myTextMeshPro = myInfo.GetComponent<TextMeshPro>();
        myInfoTr = myInfo.GetComponent<RectTransform>();


        UiImgae = skill.GetComponentsInChildren<Image>();

        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        //enemyRenderer = GetComponentInChildren<Renderer>();



        //첫 턴 준비
        TurnSetUp();
    }

    /* //onEnable의 대용. 매니저가 enemy생성을 할때 사용하는 함수.
    public void Setup(enemydata data)
    {

    }
    */

    protected override void OnEnable()
    {
        startingHealth = 4f; // enemy.Hp[0];
        startingMana = 1f;  // enemy.Mp[0];

        base.OnEnable();
        
        //navMeshAgent.speed = data.speed;
    }

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
    }

    public override void OnDamage(float damage, Vector3 hitPos, Vector3 hitSurface)
    {
        if (!dead)
        {
            //공격 받은 방향. 위치값에서 파티클
            //hitEffect.transform.position = hitPos;
            //hitEffect.transform.rotation = Quaternion.LookRotation(hitSurface);
            //hitEffect.Play();

            enemyAudioPlayer.PlayOneShot(hitSound);
        }

        base.OnDamage(damage, hitPos, hitSurface);
    }

    public override void Die()
    {
        base.Die();


        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;
        }

        //dfjalkjdlkjfl

        //enemyAnimator.SetTrigger("Die");
        //enemyAudioPlayer.PlayOneShot(deathSound);

        self.SetActive(false);
    }

    public override void Revive(float newHealth)
    {
        base.Revive(newHealth);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //겹침
        if (!dead )//&& !turnEnd)
        {
            LivingEntity attactTarget = other.GetComponent<LivingEntity>();
            //Debug.Log(other.name);

            if (attactTarget != null)// && attactTarget == targetEntity)
            {
                //공격
                Vector3 hitPos = other.ClosestPoint(transform.position);
                Vector3 hitSurface = transform.position - other.transform.position;
                //attactTarget.OnDamage(1, hitPos, hitSurface);
                EnemyInteractive();
            }

            //turnEnd = true;
            return;
        }
    }

    private void Update()
    {
       Vector3 infoPos = new Vector3(transform.position.x, transform.position.y + 50, 0);
       myInfoTr.position = infoPos;
    }

    //턴 세팅
    private void TurnSetUp()
    {
        //사망확인 - . 일단 livingentity가 들고있음.

        //turnEnd = false;

        //자신의 디버프 확인
        


        //스킬 준비
        ReadyNewSkill();

        //UI 셋업
        UpdateUi();
    }


    //다음 스킬 준비
    private void ReadyNewSkill()
    {
        enemy.Skill = skills[Random.Range(0, 3)];
    }

    private void UpdateUi()
    {
        //스킬
        UiImgae[0].sprite = Resources.Load<Sprite>("Picture/" + enemy.Skill.ToString());

        //턴
        string[] num = { "One", "Two", "Three" };
        UiImgae[1].sprite = Resources.Load<Sprite>("Picture/" + num[0]);
    }

    //상호작용
    private void EnemyInteractive()
    {
        battleManager.Interaction(self, player);
    }

    //어그로 대상에게 접근하기.
    IEnumerator UpdateRun()
    {
        float distance = float.MaxValue;
        
        for (; distance >= 0;)
        {
            //Debug.Log(tr + " " + target + " " + distance);

            distance = Vector3.Distance(transform.position, target.position);

            //근접시 행동
            if (distance < 0)
            {
                //EnemyInteractive();
            }
            //이동
            else
            {
                
                Vector3 move = new Vector3(target.position.x - tr.position.x, target.position.y - tr.position.y, 0).normalized * speed * Time.deltaTime;
                tr.position += move;
            }

            yield return null;
        }

    }

    //함수 없앨것.
    public void EndTurn(bool lose)
    {
        Vector3 tmp = Vector3.zero;
        if (lose) OnDamage(1.0f, tmp, tmp);
        
        StopCoroutine("UpdateRun");
        tr.position = new Vector3(-300, 300);

        
        TurnSetUp(); // 매니저에게 전담 필요.
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
