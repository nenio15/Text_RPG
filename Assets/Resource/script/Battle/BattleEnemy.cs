using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable]
public class Enemy
{
    public int Level = 1;
    public int[] Hp = { 1, 1 };
    public int[] Mp = { 1, 1 };
    public int[] Stat = { 1, 1, 1, 1, 1, 1 };

    public string Type = "Goblin";
    public string Class = "Warrior";
    public string Weapon = "Club";
    public string Skill = "Rock";
}


public class BattleEnemy : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject self;
    [SerializeField] public Enemy enemy = new Enemy(); //�ӽ�
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
        Player = GameObject.Find("Player").gameObject;
        self = gameObject;
        battleManager = GameObject.FindObjectOfType<BattleManager>();
        //myInfo = gameObject.gameObject.gameObject;
        //button[len].GetComponentInChildren<Text>().text = list.ToString();

        //state = State.Idle;
        tr = GetComponent<RectTransform>();
        //target = Player.transform.position; //���ŵ� �վ����
        target = Player.GetComponent<RectTransform>();
        //agent.destination = target.transform.position;

        //��ȣ�ۿ� ù �� �Ӵϴ�
        ReadyNewSkill();

        //hp bar text setting
        myTextMeshPro = myInfo.GetComponent<TextMeshPro>();
        myInfoTr = myInfo.GetComponent<RectTransform>();



        //state = State.Run;
        //StartCoroutine("UpdateRun");
    }

    private void Update()
    {
       //�̰� rect�� anchoredposition���� ����.
       Vector3 infoPos = new Vector3(transform.position.x, transform.position.y + 50, 0);
       myInfoTr.position = infoPos;
    }

    //���� ��ų �غ�
    private void ReadyNewSkill()
    {
        enemy.Skill = skills[Random.Range(0, 3)];
        Debug.Log(enemy.Skill + " : is [enemy] skill");
    }

    //��ȣ�ۿ�
    private void EnemyInteractive()
    {
        //state = State.Attack;
        //�ѹ��� �����ؾ��ϴµ�... �ű��� �˰����� ����� �ȳ���. ������? 
        battleManager.Interaction(self);
        
        //battlemanager.Interaction(self.name.ToString()); //��
        //GameObject.Find("Desicion").GetComponent<DiceDecision>().DesicionWinner(self.name.ToString()); 
        //tr.position = new Vector3(-300, 300);
        
    }

    //��׷� ��󿡰� �����ϱ�.
    IEnumerator UpdateRun()
    {
        float distance = float.MaxValue;
        
        for (; distance >= 200;)
        {
            //Debug.Log(tr + " " + target + " " + distance);

            distance = Vector3.Distance(transform.position, target.position);
            //������ �ൿ
            if (distance < 200) EnemyInteractive();
            Vector3 move = new Vector3(target.position.x - tr.position.x, target.position.y - tr.position.y, 0).normalized * speed * Time.deltaTime;
            tr.position += move;

            yield return null;
        }

    }

    //���⼭ hp+-�����Ŷ� �̰�����.. �׷��� ����, ����� �μ��� �޾ƾ߰���?
    //�� ���� -> IDamageable�� ���� ��.
    public void EndTurn(bool lose)
    {
        if (lose) 
        { 
            //Debug.Log("he loses");
            enemy.Hp[0] -= 1;
        }

        StopCoroutine("UpdateRun");
        tr.position = new Vector3(-300, 300);

        //����ٰ� �׾������� �߰� Ȯ��. ����. �����. ���.
        CheckDead();

        ReadyNewSkill();
    }

    //�������
    private void CheckDead()
    {
        if(enemy.Hp[0] <= 0)
        {
            //Debug.Log("he is dead");
            //������ �ִϸ��̼�.... (��� false��ü�� ���� �ƴ�. ��ü�� �ְų� �Ұ���)
            self.SetActive(false);
        }
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
