using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable] //아직 어떤 느낌이 좋을지 모르겠네...
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
 * 이걸 한 스크립트에 쓸까 아니면 달리 나눠야할까
 * ※ 하나의 함수에 하나의 기능을 에 위반하지는 않음.
 * 
 * 
 * 1.적 인적사항
 * 2.적 공격 3종류 선택기 + 보여주는거.
            * 3.적 움직임
 * 4.적이 움직이고 멈추는 트리거
 * 5.적이 플레이어와 가까우면 상호작용하는 트리거
 * 
 * 일단 움직임부터 끝내자구
 * 
 * ㅡ움직임ㅡ
 * 1.아군의 위치와 자신의 위치를 비교
 * 2.그에 맞게 움직일것.
 * 
 */


public class EnemyInfo : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject self;
    [SerializeField] public Enemy enemy = new Enemy(); //임시
    
    public float speed = 400.0f;
    private Transform tr;

    //목적지
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
        target = Player.transform.position; //갱신도 잇어야함
        //agent.destination = target.transform.position;

        //상호작용 첫 수 둡니다

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
        //한번만 실행해야하는데... 거까진 알고리즘이 기억이 안난다. 프리즈? 

        GameObject.Find("Desicion").GetComponent<DiceDecision>().DesicionWinner(self.name.ToString()); //흠
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

    //여기서 hp+-같은거랑 이거저거.. 그러면 물론, 계수를 인수로 받아야겠지?
    public void EndTurn(bool lose)
    {
        if (lose) 
        { 
            //Debug.Log("he loses");
            enemy.Hp[0] -= 1;
        }
        else Debug.Log("he wins");

        
        CheckDead();
        //여기다가 죽었는지도 추가 확인. 감소. 디버프. 사망.
    }

    private void CheckDead()
    {
        if(enemy.Hp[0] <= 0)
        {
            //Debug.Log("he is dead");
            //모종의 애니메이션.... (사실 false자체가 참은 아님. 시체가 있거나 할거임)
            self.SetActive(false);
        }
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
