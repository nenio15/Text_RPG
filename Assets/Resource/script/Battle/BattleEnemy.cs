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
    [SerializeField] public Enemy enemy = new Enemy(); //임시
    private BattleManager battleManager;

    public float speed = 400.0f;
    private Transform tr;

    //목적지
    private Vector3 target;
    //NavMeshAgent agent;

    //몬스터 상단의 표기 세팅
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

    private void Awake()
    {
        Player = GameObject.Find("Player").gameObject;
        self = gameObject;
        battleManager = GameObject.FindObjectOfType<BattleManager>();
        //myInfo = gameObject.gameObject.gameObject;
        //button[len].GetComponentInChildren<Text>().text = list.ToString();

        //state = State.Idle;
        tr = GetComponent<Transform>();
        target = Player.transform.position; //갱신도 잇어야함
        //agent.destination = target.transform.position;

        //상호작용 첫 수 둡니다
        ReadyNewSkill();

        //hp bar text setting
        myTextMeshPro = myInfo.GetComponent<TextMeshPro>();
        myInfoTr = myInfo.GetComponent<RectTransform>();



        //state = State.Run;
        //StartCoroutine("UpdateRun");
    }

    private void Update()
    {
       Vector3 infoPos = new Vector3(transform.position.x, transform.position.y + 50, 0);
       myInfoTr.position = infoPos;
    }

    private void ReadyNewSkill()
    {
        enemy.Skill = skills[Random.Range(0, 3)];
        Debug.Log(enemy.Skill + " : is [enemy] skill");
    }

    private void EnemyInteractive()
    {
        //state = State.Attack;
        //한번만 실행해야하는데... 거까진 알고리즘이 기억이 안난다. 프리즈? 
        battleManager.Interaction(self);
        
        //battlemanager.Interaction(self.name.ToString()); //흠
        //GameObject.Find("Desicion").GetComponent<DiceDecision>().DesicionWinner(self.name.ToString()); 
        //tr.position = new Vector3(-300, 300);
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

        StopCoroutine("UpdateRun");
        tr.position = new Vector3(-300, 300);

        //여기다가 죽었는지도 추가 확인. 감소. 디버프. 사망.
        CheckDead();
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
