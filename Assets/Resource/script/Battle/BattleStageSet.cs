using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleStageSet : MonoBehaviour
{
    /*
     * ȯ�汸��(Ư��ȿ��)
     * enemy�� state�� level���� �߰�
     * �������� ������
     */

    private enum ObjType
    {
        Decoration,
        Obstacle,
        Monster,
        Npc
    }

    //���� ���� ��ü��
    public Sprite field_base;
    public GameObject player;
    public GameObject field_frame;
    public GameObject enemylist;

    [SerializeField] private Inventory inventory;

    //��� ����
    //private string path = @"/Resource/Text/Battle/StageFreeSet/";
    //private string field_free_set;
    private JToken jbase;

    public void Setting(string freeset) //1.string Freeset name.json
    {
        //json�ε�
        
        //�����..
        if(Resources.Load<TextAsset>("Text/Battle/StageFreeSet/" + freeset) == null) { Debug.LogError("[BATTLESET] : " + freeset + " don't exist"); return; }
        string str = Resources.Load<TextAsset>("Text/Battle/StageFreeSet/" + freeset).ToString();
        jbase = JObject.Parse(str);
        JToken jset = jbase["set"];
        
        //�ʵ� ��� ��ȯ
        field_base = Resources.Load<Sprite>("Picture/" + jset["background"].ToString());

        //enemy�� �߰���, state����� �׷��� �߰��� ����. level�̶����. ���� �� ��ü�� ��ũ��Ʈ������

        //����, �ɽ�, ĳ���� ���� �� ��ġ
        string[] list = { "decoration", "obstacle", "monster" };
        for(int i = 0; i < 3; i++)
            foreach(JToken jtmp in jset[list[i]])
                Generate(i, jtmp);

        //npc ����
        
        //player��ġ
        //Vector3 pos = new Vector3(700, 150, 0);
        //player.GetComponent<RectTransform>().position += pos;

        //�ý���

        //�ʱ� ī�޶� ����
        JToken jcamera = jset["camera"];
        field_frame.transform.position = new Vector3((int)jcamera["pos"][0], (int)jcamera["pos"][1], 0);

        //��ų�� ���� (�ε�)


    }

    //������ �ν��Ͻ� ���� �Լ�
    private void Generate(int type, JToken jobj)
    {
        //������ ����
        GameObject prefab = Resources.Load<GameObject>(jobj["name"].ToString());
        Vector3 pos = new Vector3((int)jobj["pos"][0], (int)jobj["pos"][1], 0); // z��ǥ�� �־� ����..

        //�ν���Ʈ ����
        GameObject tmp;
        if(type == 2) tmp = Instantiate(prefab, enemylist.transform);
        else tmp = Instantiate(prefab, field_frame.transform);
        tmp.GetComponent<RectTransform>().anchoredPosition = pos;

        //tmp.GetComponet<BattleEnemy>().SetUp();
    }

    //���� �¸� �й� ���� Ȯ�� - ����.
    public bool JudgeWinner() { return true; }

    //���� ���� �� ���
    public void CalculateBattle(bool win, GameObject player)
    {
        JToken jget = jbase["reward"];
        int i = 0;
        //player.GetComponent<PlayerHealth>().UpdateData("gold", i);

        //��� �̷��Ŵ� enemy�� ������̺��� ���ؼ� ���� �ϴµ�. ��..
        //win ���º�..? ����.
        if (win)
        {


            //��ȭ
            i = (int)jget["gold"];
            Debug.Log(i);
            player.GetComponent<PlayerHealth>().UpdateData("gold", i);
            //����ġ
            i = (int)jget["exp"];
            player.GetComponent<PlayerHealth>().UpdateData("exp", i);
            //��Ӿ�����
            Debug.Log(jget["drop"]);
        }

    }


}
