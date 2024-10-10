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


    // 대충 배틀 관리자 입니다 예.
    private JObject jroot;
    private JToken jfield;

    // 해당 스크립트는 text '뷰' 전용으로 바꿀 예정
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
    
    //배틀 필드 세팅을 위한 프리셋
    public void BattlePreset(string fieldname, string root, int num, int situ) //"goblin", "forest_goblin", 1, 0]},
    {
        fieldroute = Application.dataPath + path + @"Field\" + fieldname + ".json";
        string str = new ConvertJson().MakeJson(fieldroute);
        
        jroot = JObject.Parse(str);
        jfield = jroot[root];

        //ㅇㅁ니ㅏㅓㅓ리익ㄷ회ㅏ머ㅗ이ㅏ
        battleStageSet.Setting(root);
        //battleStageSet.Setting("GoblinForestEvent1"); //이거 왜 하드코딩이야 머저리야.
        //그리고 대충 num맞추고 situ때려 맞추면 될듯.
    }


    //여기서 배틀 진입 시작 및 활성화
    public void BattleEntry()
    {
        //기본 텍스트 비활성화 + 나중에 추가로 위 ui도 날릴것.
        scrollView.SetActive(false); 

        //player = GameObject.Find("Player");
        //enemys[0] = GameObject.Find("Enemy1");

        selectionManager.ShowSelection("Action", 0, 1);
        transform.position = new Vector3(400, 700, -1);

        //이걸로 충분히 탐색 가능. 다만, transform위치가 너무 먼게 아닌지 고민. content로 다이렉트? 해버려 말어.
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

        //멈추기
        player.GetComponent<BattlePlayer>().StopCoroutine("UpdateRun");

        foreach (BattleEnemy enemy in enemys)
        {
            enemy.StopCoroutine("UpdateRun");
        }
    }

    private void InstantStrategy() //GameObject enemy ?
    {
        //enemy.Getcomponent<>.weapon.... 대충 길어질 예정


        //임의로 club
        selectionManager.ShowSelection("Sword", 0, 2);

        //+ enemy 기술에 따른 필드문 추가(Club + Scissors(swing))
    }

    public void DuringBattle()
    {

    }

    public void BattleEndCheck(GameObject player, GameObject enemy, bool win)
    {
        //체킹을 enemy imgae rendering의 활성화 여부로.. // collider



        
        //해당자가 player가 아닐경우, enemy로 판단.   //npc는..?
        if (player.GetComponent<BattlePlayer>() != null) player.GetComponent<BattlePlayer>().EndTurn(win);
        else player.GetComponent<BattleEnemy>().EndTurn(win);
        
        //이하동문
        if(enemy.GetComponent<BattleEnemy>() != null) enemy.GetComponent<BattleEnemy>().EndTurn(win);
        else enemy.GetComponent<BattlePlayer>().EndTurn(win);

        //반영
        playerUiManager.UploadToGame();

        //전투 종료
        if (!enemy.activeSelf) { BattleIsEnd("ended"); return; }


        //다음 재시작
        selectionManager.ShowSelection("Action", 0, 1);

    }
    
    public void BattleIsEnd(string detail) //전투포기 같은거 용도..
    {
        //Debug.Log("something ending ...");

        //kill any things... 아마 다른 배틀 시작할때 오류 발생할듯?

        //여기 desicion btn state도 추가할것.
        transform.position = new Vector3(-700, 700, -1);
        //battleFieldView.SetActive(false);
        //selectionManager.BtnUnActive();
        scrollView.SetActive(true);

        //임의의 방식... 다른 좋은게 있을까?
        TextManager text = FindObjectOfType<TextManager>();
        text.stop_read = false;

        //☆☆☆기존에 읽던 페이지를 상기시켜야하는데... 그냥 뷰만 비활성화 시킬까. 방법을 따로 찾아야할듯?

        //scrollView.GetComponent<SelectionManager>
        
    }

}
