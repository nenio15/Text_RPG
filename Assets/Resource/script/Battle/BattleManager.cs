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
        enemys = transform.GetComponentsInChildren<BattleEnemy>();
    }

    //�� ����
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

    //��ȣ�ۿ�
    public void Interaction(GameObject self, GameObject target)
    {
        //Debug.Log("interaction call");
        adjacent_target = target;

        //�ߺ� ��ȣ�ۿ� ����.
        if(retarget == self && reself == target) { return; }
        reself = self;
        retarget = target;
        

        //���߱�
        //player.GetComponent<BattlePlayer>().StopCoroutine("UpdateRun");

        foreach (BattleEnemy enemy in enemys)
        {
            enemy.StopCoroutine("UpdateRun");
        }

        battleAudioPlayer.PlayOneShot(hitSound);
        //judgement.DesicionWinner(player, adjacent_enemy, player.GetComponent<PlayerHealth>().player_info.Skill );
        judgement.DesicionWinner(self, adjacent_target, player.GetComponent<PlayerHealth>().player_info.Skill);
    }


    //�� ����
    public void BattleEndCheck(GameObject player, GameObject target, bool win)
    {
        //������ ��ȯ


        //����. Ondamage.
        if (player.GetComponent<PlayerMovement>() != null) player.GetComponent<PlayerMovement>().EndTurn(win);
        else player.GetComponent<BattleEnemy>().EndTurn(win);

        //���ϵ���
        if (target.GetComponent<BattleEnemy>() != null) target.GetComponent<BattleEnemy>().EndTurn(win);
        else target.GetComponent<PlayerMovement>().EndTurn(win);


        //������ �ݿ�
        playerUiManager.UploadToGame();


        bool alldead = true;
        //��� ��� ������.
        foreach (BattleEnemy enemy in enemys)
            if (!enemy.dead) alldead = false;

        //���� ����
        if (alldead) BattleIsEnd("ended");

        //if (!target.activeSelf) { BattleIsEnd("ended"); return; }
        //if (enemy.GetComponent<BattleEnemy>().dead) { BattleIsEnd("ended"); return; }

    }
    
    //���� ���� ���� ���μ���
    public void BattleIsEnd(string detail) //���� �̱���
    {
        //���� desicion btn state�� �߰��Ұ�.
        Debug.Log("battle is ended");

        //�������� �ؽ�Ʈ��
        transform.position = new Vector3(-700, 700, -1);
        //selectionManager.BtnUnActive();
        scrollView.SetActive(true);

        //Ÿ ��ũ��Ʈ�� ��� ����ٰ� ���°� ��..
        TextManager text = FindObjectOfType<TextManager>();
        text.stop_read = false;

        //�١١ٱ����� �д� �������� �����Ѿ��ϴµ�... �׳� �丸 ��Ȱ��ȭ ��ų��. ����� ���� ã�ƾ��ҵ�?
    }

}
