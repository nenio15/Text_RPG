using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;
using UnityEngine.Localization.SmartFormat.Utilities;

public class BattleManager : MonoBehaviour
{
    //[SerializeField] private Text battleField;
    [SerializeField] private Text battleText;
    [SerializeField] private GameObject battleFieldView;
    [SerializeField] private GameObject scrollView;

    [Header("PLAYERS")]
    [SerializeField] public GameObject player;
    [SerializeField] public BattleEnemy[] enemys;
    public GameObject adjacent_enemy;


    [Header("OBJ_INTERATION")]
    //[SerializeField] private int robj_i = 0;
    [SerializeField] private GameObject[] robj;
    [SerializeField] private GameObject clickobj;



    [Header("OTHER_MANAGERS")]
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private BattleStageSet battleStageSet;
    [SerializeField] private Judgement judgement;

    //json route
    private string battlefield;
    private string monroute;
    private string proute;
    private string path = @"\Resource\Text\Battle\";
    private string fieldroute;


    // ���� ��Ʋ ������ �Դϴ� ��.
    private JObject jroot;
    private JToken jfield;

    // �ش� ��ũ��Ʈ�� text '��' �������� �ٲ� ����
    private void Start()
    {
        battlefield = Application.dataPath + path + @"Field\BattleField.json";
        monroute = Application.dataPath + path + "Monster.json";
        proute = Application.dataPath + path + "Player.json";
        //battlefieldview = GameObject.Find("BattleField");
        //Debug.Log(battleFieldView.name);
        //scrollView = GameObject.Find("Scroll View");
        //selectionManager = FindObjectOfType<SelectionManager>();
        //playerUiManager = FindObjectOfType<PlayerUiManager>();
        judgement = FindObjectOfType<Judgement>();
    }
    
    //��Ʋ �ʵ� ������ ���� ������
    public void BattlePreset(string fieldname, string root, int num, int situ) //"goblin", "forest_goblin", 1, 0]},
    {
        fieldroute = Application.dataPath + path + @"Field\" + fieldname + ".json";
        string str = new ConvertJson().MakeJson(fieldroute);
        
        jroot = JObject.Parse(str);
        jfield = jroot[root];

        //�����Ϥ��äø��ͤ�ȸ���Ӥ��̤�
        battleStageSet.Setting(root);
        //battleStageSet.Setting("GoblinForestEvent1"); //�̰� �� �ϵ��ڵ��̾� ��������.
        //�׸��� ���� num���߰� situ���� ���߸� �ɵ�.
    }


    //���⼭ ��Ʋ ���� ���� �� Ȱ��ȭ
    public void BattleEntry()
    {
        //�⺻ �ؽ�Ʈ ��Ȱ��ȭ + ���߿� �߰��� �� ui�� ������.
        scrollView.SetActive(false); 

        //player = GameObject.Find("Player");
        //enemys[0] = GameObject.Find("Enemy1");

        selectionManager.ShowSelection("Action", 0, 1);
        transform.position = new Vector3(400, 700, -1);

        //�̰ɷ� ����� Ž�� ����. �ٸ�, transform��ġ�� �ʹ� �հ� �ƴ��� ���. content�� ���̷�Ʈ? �ع��� ����.
        enemys = transform.GetComponentsInChildren<BattleEnemy>();
        /*
        foreach (BattleEnemy enemy in tmp)
        {
            enemy.transform.position = new Vector3(100, 100, 0);
            Debug.Log(enemy);
        }
        */

    }




    public void TurnStart()
    {
        player.GetComponent<BattlePlayer>().StartCoroutine("UpdateRun");

        foreach (BattleEnemy enemy in enemys)
        {
            enemy.StartCoroutine("UpdateRun");
        }
    }

    public void Interaction(GameObject enemy_name)
    {
        InstantStrategy();
        adjacent_enemy = enemy_name;

        //���߱�
        player.GetComponent<BattlePlayer>().StopCoroutine("UpdateRun");

        foreach (BattleEnemy enemy in enemys)
        {
            enemy.StopCoroutine("UpdateRun");
        }
    }

    private void InstantStrategy() //GameObject enemy ?
    {
        //enemy.Getcomponent<>.weapon.... ���� ����� ����


        //���Ƿ� club
        selectionManager.ShowSelection("Sword", 0, 2);

        //+ enemy ����� ���� �ʵ幮 �߰�(Club + Scissors(swing))
    }

    public void DuringBattle()
    {

    }

    public void BattleEndCheck(GameObject player, GameObject enemy, bool win)
    {
        //üŷ�� enemy imgae rendering�� Ȱ��ȭ ���η�.. // collider



        
        //�ش��ڰ� player�� �ƴҰ��, enemy�� �Ǵ�.   //npc��..?
        if (player.GetComponent<BattlePlayer>() != null) player.GetComponent<BattlePlayer>().EndTurn(win);
        else player.GetComponent<BattleEnemy>().EndTurn(win);
        
        //���ϵ���
        if(enemy.GetComponent<BattleEnemy>() != null) enemy.GetComponent<BattleEnemy>().EndTurn(win);
        else enemy.GetComponent<BattlePlayer>().EndTurn(win);

        //�ݿ�
        playerUiManager.UploadToGame();

        //���� ����
        if (!enemy.activeSelf) { BattleIsEnd("ended"); return; }


        //���� �����
        selectionManager.ShowSelection("Action", 0, 1);

    }
    
    public void BattleIsEnd(string detail) //�������� ������ �뵵..
    {
        //Debug.Log("something ending ...");

        //kill any things... �Ƹ� �ٸ� ��Ʋ �����Ҷ� ���� �߻��ҵ�?

        //���� desicion btn state�� �߰��Ұ�.
        transform.position = new Vector3(-700, 700, -1);
        //battleFieldView.SetActive(false);
        //selectionManager.BtnUnActive();
        scrollView.SetActive(true);

        //������ ���... �ٸ� ������ ������?
        TextManager text = FindObjectOfType<TextManager>();
        text.stop_read = false;

        //�١١ٱ����� �д� �������� �����Ѿ��ϴµ�... �׳� �丸 ��Ȱ��ȭ ��ų��. ����� ���� ã�ƾ��ҵ�?

        //scrollView.GetComponent<SelectionManager>
        
    }

}
