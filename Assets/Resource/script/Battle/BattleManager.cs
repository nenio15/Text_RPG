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
    public GameObject adjacent_target;


    [Header("OBJ_INTERATION")]
    //[SerializeField] private int robj_i = 0;
    [SerializeField] private GameObject[] robj;
    [SerializeField] private GameObject clickobj;



    [Header("OTHER_MANAGERS")]
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private BattleStageSet battleStageSet;
    [SerializeField] private Judgement judgement;

    private AudioSource battleAudioPlayer;
    public AudioClip hitSound;

    private GameObject reself, retarget;

    //json route
    /*
    private string battlefield;
    private string monroute;
    private string proute;
    private string path = @"\Resource\Text\Battle\";
    private string fieldroute;
    
    
    private JObject jroot;
    private JToken jfield;
    */

    private void Start()
    {
        judgement = FindObjectOfType<Judgement>();
        battleAudioPlayer = GetComponent<AudioSource>();
    }
    
    //배틀 필드 세팅을 위한 프리셋
    public void BattlePreset(string fieldname, string root, int num, int situ) //"goblin", "forest_goblin", 1, 0]},
    {
        //Action 기능 활용 모색.
        battleStageSet.Setting(root);
    }

    //배틀 진입
    public void BattleEntry()
    {
        //기본 텍스트 비활성화
        scrollView.SetActive(false); 

        //배틀 무대 세팅
        selectionManager.ShowSelection("Action", 0, 1);
        transform.position = new Vector3(400, 700, -1);

        //enemys 지정
        enemys = transform.GetComponentsInChildren<BattleEnemy>();
    }

    //턴 시작
    public void TurnStart()
    {
        PlayerMovement battlePlayer = player.GetComponent<PlayerMovement>();
        battlePlayer.StartCoroutine("UpdateRun", battlePlayer.target);
        //battlePlayer.UpdateRun(battlePlayer.target);

        foreach (BattleEnemy enemy in enemys)
        {
            enemy.StartCoroutine("UpdateRun");
        }
    }

    //상호작용
    public void Interaction(GameObject self, GameObject target)
    {
        //Debug.Log("interaction call");
        adjacent_target = target;

        //중복 상호작용 막기.
        if(retarget == self && reself == target) { return; }
        reself = self;
        retarget = target;
        

        //멈추기
        //player.GetComponent<BattlePlayer>().StopCoroutine("UpdateRun");

        foreach (BattleEnemy enemy in enemys)
        {
            enemy.StopCoroutine("UpdateRun");
        }

        battleAudioPlayer.PlayOneShot(hitSound);
        //judgement.DesicionWinner(player, adjacent_enemy, player.GetComponent<PlayerHealth>().player_info.Skill );
        judgement.DesicionWinner(self, adjacent_target, player.GetComponent<PlayerHealth>().player_info.Skill);
    }


    //턴 종료
    public void BattleEndCheck(GameObject player, GameObject target, bool win)
    {
        //가독성 전환


        //변경. Ondamage.
        if (player.GetComponent<PlayerMovement>() != null) player.GetComponent<PlayerMovement>().EndTurn(win);
        else player.GetComponent<BattleEnemy>().EndTurn(win);

        //이하동문
        if (target.GetComponent<BattleEnemy>() != null) target.GetComponent<BattleEnemy>().EndTurn(win);
        else target.GetComponent<PlayerMovement>().EndTurn(win);


        //데이터 반영
        playerUiManager.UploadToGame();


        bool alldead = true;
        //모두 사망 판정시.
        foreach (BattleEnemy enemy in enemys)
            if (!enemy.dead) alldead = false;

        //전투 종료
        if (alldead) BattleIsEnd("ended");

        //if (!target.activeSelf) { BattleIsEnd("ended"); return; }
        //if (enemy.GetComponent<BattleEnemy>().dead) { BattleIsEnd("ended"); return; }

    }
    
    //전투 실제 종료 프로세스
    public void BattleIsEnd(string detail) //아직 미구현
    {
        //여기 desicion btn state도 추가할것.
        Debug.Log("battle is ended");

        //전투에서 텍스트로
        transform.position = new Vector3(-700, 700, -1);
        //selectionManager.BtnUnActive();
        scrollView.SetActive(true);

        //타 스크립트를 계속 끌어다가 쓰는게 참..
        TextManager text = FindObjectOfType<TextManager>();
        text.stop_read = false;

        //☆☆☆기존에 읽던 페이지를 상기시켜야하는데... 그냥 뷰만 비활성화 시킬까. 방법을 따로 찾아야할듯?
    }

}
