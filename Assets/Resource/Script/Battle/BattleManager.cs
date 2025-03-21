using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;
using UnityEngine.Localization.SmartFormat.Utilities;
//using UnityEditor.Experimental.GraphView;
using System.Drawing.Drawing2D;

public class BattleManager : MonoBehaviour
{
    //사전 오브제 정의 -> 오브제 무브를 매니저가 가지고 있어야할까..?
    [SerializeField] private Text battleText;
    [SerializeField] private GameObject battleFieldView;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject enemylist;
    [SerializeField] private GameObject battlePanel;

    //필드 캐릭터들 정의
    [Header("PLAYERS")]
    [SerializeField] public GameObject player;
    [SerializeField] public EnemyAction[] enemyActions;
    [SerializeField] public GameObject[] Npcs; // 미구현
    private GameObject reself, retarget;

    //사전 스크립트 정의
    [Header("OTHER_MANAGERS")]
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private BattleStageSet battleStageSet;
    [SerializeField] private CombatCalculator combatCalculator;

    //사운드, 이펙트 관련
    private AudioSource battleAudioPlayer;
    public AudioClip hitSound;

    public int turnSequence = 0;
    public int turnWave = 1;

    private void Start()
    {
        combatCalculator = FindObjectOfType<CombatCalculator>();
        battleAudioPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    //배틀 필드 세팅을 위한 프리셋
    public void BattlePreset(string fieldname, string root, int num, int situ) //"
                                                                               //", "forest_goblin", 1, 0]},
    {
        //Action 기능 활용 모색.
        battleStageSet.Setting(root);

    }

    //배틀 진입
    public void BattleEntry()
    {
        //기본 텍스트 비활성화
        scrollView.SetActive(false); 
        battlePanel.SetActive(true);

        //배틀 무대 세팅
        transform.position = new Vector3(400, 630, -1); //selectionManager.ShowSelection("Action", 0, 1);
        //전투용 선택지 따로 소환.

        //임의 첫 세팅
        turnWave = 1;

        //enemys 지정
        //enemyHealths = enemylist.transform.GetComponentsInChildren<EnemyHealth>(); //이거 빼자.
        enemyActions = enemylist.transform.GetComponentsInChildren<EnemyAction>();

        foreach (EnemyAction action in enemyActions) action.onSystemAction += Interaction;
    }

    //턴 시작
    public void TurnStart()
    {
        //행동력에 따른 호출 추가
        //플레이어. 적. 중립. 까지.
        //Debug.Log("start" + turnSequence);

        //player - movement는 나중에 변경 필.
        //PlayerMovement battlePlayer = player.GetComponent<PlayerMovement>();
        //battlePlayer.StartCoroutine("UpdateRun", battlePlayer.target);
        player.GetComponent<PlayerAction>().targetPos = enemyActions[0].gameObject.transform;
        player.GetComponent<PlayerAction>().StartCoroutine("UpdateRun");

        //npc

        //enemy
        foreach (EnemyAction enemy in enemyActions)
            if (!enemy.enemyHealth.dead) 
                if(enemy.turnSequence == turnSequence) 
                    enemy.StartCoroutine("UpdateRun"); 
    }

    //상호작용
    public void Interaction(GameObject self, GameObject target)
    {
        //중복 상호작용 막기.
        if(retarget == self && reself == target) { return; }
        reself = self;
        retarget = target;

        //Debug.Log(self.name + target.name);
        //멈추기
        player.GetComponent<PlayerAction>().StopCoroutine("UpdateRun");
        foreach (EnemyAction enemy in enemyActions) enemy.StopCoroutine("UpdateRun");
        

        //승부
        combatCalculator.Compete(self, target, turnSequence);

        //종료
        IsTurnEnd();
    }
    /*
    //종료 조건 확인
    public void EndCheck()
    {
        //...? 이정도면 굳이 함수가 여러개일 필요는 없는데. 흠.
        if (IsTurnEnd()) EndTurn();
    }
    */

    //턴 종료 확인
    public void IsTurnEnd()
    {
        //player 턴 종료 확인
        if (!player.GetComponent<InterAction>().turnEnd) return;

        //npc 턴 종료 확인


        //enemy 턴 종료 확인
        foreach (EnemyAction enemy in enemyActions)
            if (enemy.turnSequence == turnSequence) 
                if (!enemy.turnEnd) 
                    return;
        
        EndTurn();
    }

    //턴 종료
    public void EndTurn()
    {
        //가독성 전환
        //Debug.Log("turnEnd");

        //데이터 반영
        playerUiManager.UploadToGame();
        turnSequence++;

        
        //전투 종료 판정
        bool allDead = true;
        
        //모두 사망 - 승리랑 패배 판정으로 다시 나눌것.
        foreach (EnemyAction enemy in enemyActions)
            if (!enemy.enemyHealth.dead) allDead = false;
        if (enemyActions == null) Debug.Log("flajdlr");
        //Debug.Log(enemyActions[0].enemyHealth.dead.ToString() + enemyActions[0].enemyHealth.health);
        
        if (allDead) BattleShutdown("ended");


        //턴 서사 정리용 (이거 이렇게 넣어도 될지 모르겠네..)
        CallNarrative(player);
        foreach (EnemyAction enemy in enemyActions) CallNarrative(enemy.gameObject);


        //이거 삭제할 듯.

        //마지막 턴이 종료
        if (turnSequence == 3)
        {
            turnWave++;
            turnSequence = 0;
            player.GetComponent<PlayerAction>().ResetAction();

            //캐릭터 정지 후, 서사 확인.
            player.GetComponent<PlayerAction>().StopCoroutine("UpdateRun");
            CallNarrative(player);

            foreach (EnemyAction enemy in enemyActions) { enemy.StopCoroutine("UpdateRun"); CallNarrative(enemy.gameObject); }

            //npc도 포함. CallNarrative(npc);

        }
        else
        {
            //TurnStart();
        }

    }

    //전투 종료
    public void BattleShutdown(string detail) //승패 미구현. 
    {
        //임시 조치
        Debug.Log("battle is ended");
        player.GetComponent<PlayerAction>().StopCoroutine("UpdateRun");
        player.GetComponent<PlayerAction>().ResetAction(); // 혹시 모르니 임시조치

        //전투 보상.
        battlePanel.SetActive(false);
        battleStageSet.EndBattle(true, player);


        //전투에서 텍스트로
        transform.position = new Vector3(-700, 700, -1);
        //필요?
        selectionManager.BtnUnActive();
        scrollView.SetActive(true);
        //더럽.
        TextManager text = FindObjectOfType<TextManager>();
        text.stop_read = false;

    }

    public void CallNarrative(GameObject target) { NarrativeManager.instance.CallByManager(target, this); }
    
}
