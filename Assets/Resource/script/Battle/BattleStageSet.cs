using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleStageSet : MonoBehaviour
{
    /*
     * 4.환경구성(특수효과) - 나중에 구현
     * 6.턴 제한 등의 시스템 반환(이거는... 여기다가 라기엔 btl 매니저한테 변수로 줘야하지 않을까? {get; set;} 이거면 될듯?
     * 
     * 추가 의문사항
     * 1.enemy의 state나 level등의 추가
     * 2.enemy의 parent설정. enemylist로? 왜 그래야하지? 그러면 deco, obs도 분류를 두는게 낫지 않는가?
     * 3.배경 자체의 지형지물 
     * -> 배경들 프리팹? 
     * -> obstacle로 걍 대체? 
     * -> 배경 자체를 갈아끼워? 
     * -> box collider를 여럿 배치 시켜버려?
     * 
     * 아래 enum에 따른 type별 대응사항.
     * deco랑 obstacle은 별로 필요없고, monster랑 npc는 미묘.. monster는 필요한게 맞긴한데...흠좀무
     */
    private enum ObjType
    {
        Decoration,
        Obstacle,
        Monster,
        Npc
    }

    //고정 지정 개체들
    public Image field_base;
    public GameObject player;
    public GameObject field_frame;

    //경로 세팅
    private string path = @"/Resource/Text/Battle/StageFreeSet/";
    private string field_free_set;
    private JToken jbase;

    public void Setting(string freeset) //1.string Freeset name.json
    {
        //foreach의 대용을 for? 아님 while

        //json로딩
        field_free_set = Application.dataPath + path + freeset + ".json";
        ConvertJson convertJson = new ConvertJson();
        string str = convertJson.MakeJson(field_free_set);
        jbase = JObject.Parse(str);
        JToken jset = jbase["set"];
        
        //필드 배경 전환
        //이거를.... 건물있는 지형은.... 흠 충돌 판정 먼저.
        field_base.sprite = Resources.Load<Sprite>("Picture/" + jset["background"].ToString());

        //enemy는 추가로, state라던가 그런게 추가될 예정. level이라던가. 물론 그 개체의 스크립트쪽으로

        //데코, 옵스, 캐릭터 생성 밑 배치
        string[] list = { "decoration", "obstacle", "monster" };
        for(int i = 0; i < 3; i++)
            foreach(JToken jtmp in jset[list[i]])
                Generate(i, jtmp);

        //npc 생성
        
        //player배치 + 특수효과..?
        Vector3 pos = new Vector3(700, 150, 0);
        player.GetComponent<RectTransform>().anchoredPosition = pos;

        //시스템 ..?

        //초기 카메라 세팅
        JToken jcamera = jset["camera"];
        field_frame.transform.position = new Vector3((int)jcamera["pos"][0], (int)jcamera["pos"][1], 0);
    }

    //프리팹 인스턴스 생성 함수
    private void Generate(int type, JToken jobj)
    {
        //왜 만듦..?
        switch (type)
        {
            case (int)ObjType.Monster:
                //tmp.GetComponent<Enemy>().State = awake (jobj["state"].ToString());
                //tmp.GetComponent<Enemy>().Set(jobj["state"].ToString(), //level);
                break;
            case (int)ObjType.Npc:
                break;
            case (int)ObjType.Decoration:
            case (int)ObjType.Obstacle: // 따로 추가사항 없음.. 그럼 왜 만듦? ㅁ?ㄹ
                break;
        }

        //프리팹 지정
        GameObject prefab = Resources.Load<GameObject>(jobj["name"].ToString());
        Vector3 pos = new Vector3((int)jobj["pos"][0], (int)jobj["pos"][1], 0); // z좌표를 넣어 말어..

        //인스턴트 생성
        GameObject tmp = Instantiate(prefab, field_frame.transform);
        tmp.GetComponent<RectTransform>().anchoredPosition = pos;
    }


}
