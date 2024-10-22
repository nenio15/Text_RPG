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
    [SerializeField] public Character enemy = new Character(); //�ӽ�
    private BattleManager battleManager;

    public float speed = 400.0f;
    private RectTransform tr;
    private RectTransform target;
    //NavMeshAgent agent;

    //���� ����� ǥ�� ����
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
    //private Renderer enemyRenderer; // ������.

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

    //����
    private void Awake()
    {
        player = GameObject.Find("Player").gameObject; // ���� �ʿ�.
        self = gameObject;
        battleManager = FindObjectOfType<BattleManager>();

        tr = GetComponent<RectTransform>();
        target = player.GetComponent<RectTransform>(); // ���� ��.

        
        //hp bar text setting
        myTextMeshPro = myInfo.GetComponent<TextMeshPro>();
        myInfoTr = myInfo.GetComponent<RectTransform>();


        UiImgae = skill.GetComponentsInChildren<Image>();

        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        //enemyRenderer = GetComponentInChildren<Renderer>();



        //ù �� �غ�
        TurnSetUp();
    }

    /* //onEnable�� ���. �Ŵ����� enemy������ �Ҷ� ����ϴ� �Լ�.
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
            //���� ���� ����. ��ġ������ ��ƼŬ
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
        //��ħ
        if (!dead )//&& !turnEnd)
        {
            LivingEntity attactTarget = other.GetComponent<LivingEntity>();
            //Debug.Log(other.name);

            if (attactTarget != null)// && attactTarget == targetEntity)
            {
                //����
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

    //�� ����
    private void TurnSetUp()
    {
        //���Ȯ�� - . �ϴ� livingentity�� �������.

        //turnEnd = false;

        //�ڽ��� ����� Ȯ��
        


        //��ų �غ�
        ReadyNewSkill();

        //UI �¾�
        UpdateUi();
    }


    //���� ��ų �غ�
    private void ReadyNewSkill()
    {
        enemy.Skill = skills[Random.Range(0, 3)];
    }

    private void UpdateUi()
    {
        //��ų
        UiImgae[0].sprite = Resources.Load<Sprite>("Picture/" + enemy.Skill.ToString());

        //��
        string[] num = { "One", "Two", "Three" };
        UiImgae[1].sprite = Resources.Load<Sprite>("Picture/" + num[0]);
    }

    //��ȣ�ۿ�
    private void EnemyInteractive()
    {
        battleManager.Interaction(self, player);
    }

    //��׷� ��󿡰� �����ϱ�.
    IEnumerator UpdateRun()
    {
        float distance = float.MaxValue;
        
        for (; distance >= 0;)
        {
            //Debug.Log(tr + " " + target + " " + distance);

            distance = Vector3.Distance(transform.position, target.position);

            //������ �ൿ
            if (distance < 0)
            {
                //EnemyInteractive();
            }
            //�̵�
            else
            {
                
                Vector3 move = new Vector3(target.position.x - tr.position.x, target.position.y - tr.position.y, 0).normalized * speed * Time.deltaTime;
                tr.position += move;
            }

            yield return null;
        }

    }

    //�Լ� ���ٰ�.
    public void EndTurn(bool lose)
    {
        Vector3 tmp = Vector3.zero;
        if (lose) OnDamage(1.0f, tmp, tmp);
        
        StopCoroutine("UpdateRun");
        tr.position = new Vector3(-300, 300);

        
        TurnSetUp(); // �Ŵ������� ���� �ʿ�.
    }

    /*
     * Nav Mesh Agent�� ��� �����ϸ� ����..
    private void Start()
    {
        state = State.Run;//Idle;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        target = Player.transform.position; //���ŵ� �վ����
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
