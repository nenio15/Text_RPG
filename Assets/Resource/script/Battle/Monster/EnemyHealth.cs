using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : LivingEntity
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject self;
    private BattleManager battleManager;

    public float speed = 400.0f;
    private RectTransform tr;
    private RectTransform target;

    //NavMeshAgent agent;

    //���� ����� ǥ�� ����
    [SerializeField] private GameObject myInfo;
    private TextMeshPro myTextMeshPro;
    private RectTransform myInfoTr;

    public Animator anim;


    
    private LivingEntity targetEntity;
    //private NavMeshAgent

    public Slider healthSlider;

    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;

    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    //private Renderer enemyRenderer; // ������.

    enum State
    {
        Idle,
        Run,
        Attack
    }


    //����
    private void Awake()
    {
        player = GameObject.Find("Player").gameObject;
        self = gameObject;
        battleManager = FindObjectOfType<BattleManager>();


        tr = GetComponent<RectTransform>();
        target = player.GetComponent<RectTransform>(); // ���� ��.

        //hp bar text setting
        myTextMeshPro = myInfo.GetComponent<TextMeshPro>();
        //myInfoTr = myInfo.GetComponent<RectTransform>();

        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        //enemyRenderer = GetComponentInChildren<Renderer>();



        //ù �� �غ�
        //TurnSetUp();
    }

    protected override void OnEnable()
    {
        startingHealth = 4f; // enemy.Hp[0]; //���� ��������. �̰͵� �ǵ鿩�߰ڳ�.
        startingMana = 1f;  // enemy.Mp[0];
        healthSlider.maxValue = startingHealth;
        healthSlider.value = startingHealth;

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

            //Debug.Log("pos : " + transform.position);
            //Debug.Log("hit : " + hitPos + hitSurface);

            //tr.position = tr.position - new Vector3(-200, 200);
            tr.position -= hitPos;
        }

        base.OnDamage(damage, hitPos, hitSurface);
        healthSlider.value = health;
    }

    public override void Die()
    {
        base.Die();


        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;
        }

        //enemyAnimator.SetTrigger("Die");
        //enemyAudioPlayer.PlayOneShot(deathSound);

        //self.SetActive(false);
    }

    public override void Revive(float newHealth)
    {
        base.Revive(newHealth);
    }


    private void Update()
    {

    }


}
