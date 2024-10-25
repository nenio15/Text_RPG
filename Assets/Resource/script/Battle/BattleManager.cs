using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEditor.Experimental.GraphView;
using System.Drawing.Drawing2D;

public class BattleManager : MonoBehaviour
{
    //���� ������ ����
    [SerializeField] private Text battleText;
    [SerializeField] private GameObject battleFieldView;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject enemylist;

    //�ʵ� ĳ���͵� ����
    [Header("PLAYERS")]
    [SerializeField] public GameObject player;
    [SerializeField] public EnemyAction[] enemyActions;
    [SerializeField] public GameObject[] Npcs; // �̱���
    private GameObject reself, retarget;

    /*
    // ��ũ�� Ű ����. ���.
    [Header("OBJ_INTERATION")]
    //[SerializeField] private int robj_i = 0;
    [SerializeField] private GameObject[] robj;
    [SerializeField] private GameObject clickobj;
    */

    //���� ��ũ��Ʈ ����
    [Header("OTHER_MANAGERS")]
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private BattleStageSet battleStageSet;
    [SerializeField] private CombatCalculator combatCalculator;

    //����, ����Ʈ ����
    private AudioSource battleAudioPlayer;
    public AudioClip hitSound;

    private int turnSequence = 0;


    private void Start()
    {
        combatCalculator = FindObjectOfType<CombatCalculator>();
        battleAudioPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    //��Ʋ �ʵ� ������ ���� ������
    public void BattlePreset(string fieldname, string root, int num, int situ) //"goblin", "forest_goblin", 1, 0]},
    {
        //Action ��� Ȱ�� ���.
        battleStageSet.Setting(root);
    }

    //��Ʋ ����
    public void BattleEntry()
    {
        //�⺻ �ؽ�Ʈ ��Ȱ��ȭ
        scrollView.SetActive(false); 

        //��Ʋ ���� ����
        selectionManager.ShowSelection("Action", 0, 1);
        transform.position = new Vector3(400, 700, -1);

        //enemys ����
        //enemyHealths = enemylist.transform.GetComponentsInChildren<EnemyHealth>(); //�̰� ����.
        enemyActions = enemylist.transform.GetComponentsInChildren<EnemyAction>();

        foreach (EnemyAction action in enemyActions) action.onSystemAction += Interaction;
    }

    //�� ����
    public void TurnStart()
    {
        //�ൿ�¿� ���� ȣ�� �߰�
        //�÷��̾�. ��. �߸�. ����.
        //Debug.Log("start" + turnSequence);

        //player - movement�� ���߿� ���� ��.
        //PlayerMovement battlePlayer = player.GetComponent<PlayerMovement>();
        //battlePlayer.StartCoroutine("UpdateRun", battlePlayer.target);
        player.GetComponent<PlayerAction>().target = enemyActions[0].gameObject.transform;
        player.GetComponent<PlayerAction>().StartCoroutine("UpdateRun");

        //npc

        //enemy
        foreach (EnemyAction enemy in enemyActions)
            if (!enemy.enemyHealth.dead) 
                if(enemy.turnSequence == turnSequence) 
                    enemy.StartCoroutine("UpdateRun"); 
    }

    //��ȣ�ۿ�
    public void Interaction(GameObject self, GameObject target)
    {
        //�ߺ� ��ȣ�ۿ� ����.
        if(retarget == self && reself == target) { return; }
        reself = self;
        retarget = target;


        //���߱�
        player.GetComponent<PlayerAction>().StopCoroutine("UpdateRun");
        foreach (EnemyAction enemy in enemyActions) enemy.StopCoroutine("UpdateRun");
        

        //�º�
        combatCalculator.Compete(self, target, turnSequence);

        //����
        IsTurnEnd();
    }
    /*
    //���� ���� Ȯ��
    public void EndCheck()
    {
        //...? �������� ���� �Լ��� �������� �ʿ�� ���µ�. ��.
        if (IsTurnEnd()) EndTurn();
    }
    */

    //�� ���� Ȯ��
    public void IsTurnEnd()
    {
        //player �� ���� Ȯ��
        if (!player.GetComponent<InterAction>().turnEnd) return;

        //npc �� ���� Ȯ��


        //enemy �� ���� Ȯ��
        foreach (EnemyAction enemy in enemyActions)
            if (enemy.turnSequence == turnSequence) 
                if (!enemy.turnEnd) 
                    return;
        
        EndTurn();
    }

    //�� ����
    public void EndTurn()
    {
        //������ ��ȯ
        //Debug.Log("turnEnd");

        //������ �ݿ�
        playerUiManager.UploadToGame();
        turnSequence++;

        

        bool allDead = true;
        //��� ��� - �¸��� �й� �������� �ٽ� ������.
        foreach (EnemyAction enemy in enemyActions)
            if (!enemy.enemyHealth.dead) allDead = false;

        //���� ����
        if (allDead) BattleShutdown("ended");

        //������ ���� ����
        if (turnSequence == 3)
        {
            turnSequence = 0;
            player.GetComponent<PlayerAction>().ResetAction();

            //���߱�
            player.GetComponent<PlayerAction>().StopCoroutine("UpdateRun");
            foreach (EnemyAction enemy in enemyActions) enemy.StopCoroutine("UpdateRun");
        }
        else
        {
            TurnStart();
        }

    }

    //���� ����
    public void BattleShutdown(string detail) //���� �̱���
    {
        //���� desicion btn state�� �߰��Ұ�.
        Debug.Log("battle is ended");
        player.GetComponent<PlayerAction>().ResetAction(); // Ȥ�� �𸣴� �ӽ���ġ

        //���� ����.

        //�������� �ؽ�Ʈ��
        transform.position = new Vector3(-700, 700, -1);
        selectionManager.BtnUnActive();
        scrollView.SetActive(true);
        //����.
        TextManager text = FindObjectOfType<TextManager>();
        text.stop_read = false;

    }
    
}
