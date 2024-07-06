using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable] //���� � ������ ������ �𸣰ڳ�...
public class Enemy
{
    public int Level = 1;
    public int[] Hp = { 1, 1 };
    public int[] Mp = { 1, 1 };
    public int[] Stat = { 1, 1, 1, 1, 1, 1 };

    public string Type = "Goblin";
    public string Class = "Warrior";
    public string Skill = "Rock";
}

/*
 * �̰� �� ��ũ��Ʈ�� ���� �ƴϸ� �޸� �������ұ�
 * �� �ϳ��� �Լ��� �ϳ��� ����� �� ���������� ����.
 * 
 * 
 * 1.�� ��������
 * 2.�� ���� 3���� ���ñ� + �����ִ°�.
            * 3.�� ������
 * 4.���� �����̰� ���ߴ� Ʈ����
 * 5.���� �÷��̾�� ������ ��ȣ�ۿ��ϴ� Ʈ����
 * 
 * �ϴ� �����Ӻ��� �����ڱ�
 * 
 * �ѿ����Ӥ�
 * 1.�Ʊ��� ��ġ�� �ڽ��� ��ġ�� ��
 * 2.�׿� �°� �����ϰ�.
 * 
 */


public class EnemyInfo : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject self;
    [SerializeField] public Enemy enemy = new Enemy(); //�ӽ�
    
    public float speed = 400.0f;
    private Transform tr;

    //������
    private Vector3 target;
    //NavMeshAgent agent;

    public Animator anim;

    enum State
    {
        Idle,
        Run,
        Attack
    }

    

    State state;
    string[] skills = { "Rock", "Scissors", "Paper" };

    private void Start()
    {
        state = State.Idle;
        tr = GetComponent<Transform>();
        target = Player.transform.position; //���ŵ� �վ����
        //agent.destination = target.transform.position;

        //��ȣ�ۿ� ù �� �Ӵϴ�

        enemy.Skill = skills[Random.Range(0, 3)];
        Debug.Log(enemy.Skill + " : [enemy] is reading...");

        state = State.Run;
        //StartCoroutine("UpdateRun");
    }

    private void Update()
    {
       
    }

    private void ReadyNewSkill()
    {
        enemy.Skill = skills[Random.Range(0, 3)];
        //Debug.Log(enemy.Skill + " : [enemy] is reading...");
    }

    private void EnemyInteractive()
    {
        state = State.Attack;
        //�ѹ��� �����ؾ��ϴµ�... �ű��� �˰����� ����� �ȳ���. ������? 

        GameObject.Find("Desicion").GetComponent<DiceDecision>().DesicionWinner(self.name.ToString()); //��
        tr.position = new Vector3(-300, 300);
        ReadyNewSkill();
    }


    IEnumerator UpdateRun()
    {
        float distance = float.MaxValue;

        for (; distance >= 200;)
        {
            distance = Vector3.Distance(transform.position, target);
            if (distance < 200) EnemyInteractive();
            Vector3 move = new Vector3(target.x - tr.position.x, target.y - tr.position.y, 0).normalized * speed * Time.deltaTime;
            tr.position += move;

            yield return null;
        }

    }

    //���⼭ hp+-�����Ŷ� �̰�����.. �׷��� ����, ����� �μ��� �޾ƾ߰���?
    public void EndTurn(bool lose)
    {
        if (lose) 
        { 
            //Debug.Log("he loses");
            enemy.Hp[0] -= 1;
        }
        else Debug.Log("he wins");

        
        CheckDead();
        //����ٰ� �׾������� �߰� Ȯ��. ����. �����. ���.
    }

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
