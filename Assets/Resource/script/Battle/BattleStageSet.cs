using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleStageSet : MonoBehaviour
{
    /*
     * 환경구성(특수효과)
     * enemy의 state나 level등의 추가
     * 지형지물 프리팹
     */

    private enum ObjType
    {
        Decoration,
        Obstacle,
        Monster,
        Npc
    }

    //고정 지정 개체들
    public SpriteRenderer field_base;
    public GameObject player;
    public GameObject field_frame;
    public GameObject enemylist;
    private Dictionary dictionary;


    [SerializeField] private Inventory inventory;


    [Header("REWARD")]
    [SerializeField] private GameObject reward;
    [SerializeField] private GameObject itemGrid;
    [SerializeField] private TextMeshProUGUI gold;
    [SerializeField] private TextMeshProUGUI exp;
    

    //경로 세팅
    //private string path = @"/Resource/Text/Battle/StageFreeSet/";
    //private string field_free_set;
    private JToken jbase;

    public void Setting(string freeset) //1.string Freeset name.json
    {
        //json로딩
        
        //디버깅..
        if(Resources.Load<TextAsset>("Text/Battle/StageFreeSet/" + freeset) == null) { Debug.LogError("[BATTLESET] : " + freeset + " don't exist"); return; }
        string str = Resources.Load<TextAsset>("Text/Battle/StageFreeSet/" + freeset).ToString();
        jbase = JObject.Parse(str);
        JToken jset = jbase["set"];
        
        //필드 배경 전환
        field_base.sprite = Resources.Load<Sprite>("Picture/" + jset["background"].ToString());
        //Debug.Log(jset["background"].ToString());
        //enemy는 추가로, state라던가 그런게 추가될 예정. level이라던가. 물론 그 개체의 스크립트쪽으로

        //데코, 옵스, 캐릭터 생성 밑 배치
        string[] list = { "decoration", "obstacle", "monster" };
        for(int i = 0; i < 3; i++)
            foreach(JToken jtmp in jset[list[i]])
                Generate(i, jtmp);

        //npc 생성

        //player배치
        //Vector3 pos = new Vector3(700, 150, 0);
        JToken jplayer = jset["player"];
        player.GetComponent<RectTransform>().anchoredPosition = new Vector3((int)jplayer["pos"][0], (int)jplayer["pos"][1] ,0);

        //시스템

        //초기 카메라 세팅
        JToken jcamera = jset["camera"];
        //field_frame.transform.position = new Vector3((int)jcamera["pos"][0], (int)jcamera["pos"][1], 0);

        //스킬셋 세팅 (로드)


    }

    //프리팹 인스턴스 생성 함수
    private void Generate(int type, JToken jobj)
    {
        //프리팹 지정
        GameObject prefab = Resources.Load<GameObject>(jobj["name"].ToString());
        Vector3 pos = new Vector3((int)jobj["pos"][0], (int)jobj["pos"][1], 0); // z좌표를 넣어 말어..

        //인스턴트 생성
        GameObject tmp;
        if(type == 2) tmp = Instantiate(prefab, enemylist.transform);
        else tmp = Instantiate(prefab, field_frame.transform);
        tmp.GetComponent<RectTransform>().anchoredPosition = pos;

        //tmp.GetComponet<BattleEnemy>().SetUp();
    }

    //전투 승리 패배 조건 확인 - 대조.
    public bool JudgeWinner() { return true; }


    public void EndBattle(bool win, GameObject player)
    {

        JToken jget = jbase["reward"];
        //int i = 0;

        NarrativeManager.instance.CallByStageSet();

        //보상화면 출력
        reward.SetActive(true);

        gold.text = jget["gold"].ToString() + "G";
        exp.text = jget["exp"].ToString();

        //획득아이템이 있을시, 아이템 리스트 추가.
        foreach (JToken drop in jget["get"])
        {
            Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());

            //보상화면에 추가.
            GameObject prefab = Resources.Load<GameObject>("itemslot");
            GameObject tmp = Instantiate(prefab, itemGrid.transform);
            //prefab.GetComponent<ItemSlotUi>().itemslot.itemData = dictionary.SetItem(dropItem.name, dropItem.type);
            tmp.GetComponent<ItemSlotUi>().Set(dropItem); //= Resources.Load<Sprite>("Picture/Item/" + drop["name"].ToString());
            Debug.Log(tmp.GetComponent<ItemSlotUi>().itemslot.itemData.img);
            Inventory.Instance.AddItem(dropItem);
        }
        
        
    }

    //전투 종료 후 결산
    public void CalculateBattle()
    {
        JToken jget = jbase["reward"];
        int i = 0;

         // 대충호출.
                                                    //player.GetComponent<PlayerHealth>().UpdateData("gold", i);

        //사실 이런거는 enemy의 드랍테이블을 비교해서 얻기는 하는데. 흠..
        //win 무승부..? 몰라.

        //금화
        i = (int)jget["gold"];
        Debug.Log(i);
        player.GetComponent<PlayerHealth>().UpdateData("gold", i);
        //경험치
        i = (int)jget["exp"];
        player.GetComponent<PlayerHealth>().UpdateData("exp", i);
        //드롭아이템
        //Debug.Log(jget["drop"]);


    }

}
