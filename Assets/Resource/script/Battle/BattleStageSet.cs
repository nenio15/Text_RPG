using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleStageSet : MonoBehaviour
{
    /*
     * 4.ȯ�汸��(Ư��ȿ��) - ���߿� ����
     * 6.�� ���� ���� �ý��� ��ȯ(�̰Ŵ�... ����ٰ� ��⿣ btl �Ŵ������� ������ ������� ������? {get; set;} �̰Ÿ� �ɵ�?
     * 
     * �߰� �ǹ�����
     * 1.enemy�� state�� level���� �߰�
     * 2.enemy�� parent����. enemylist��? �� �׷�������? �׷��� deco, obs�� �з��� �δ°� ���� �ʴ°�?
     * 3.��� ��ü�� �������� 
     * -> ���� ������? 
     * -> obstacle�� �� ��ü? 
     * -> ��� ��ü�� ���Ƴ���? 
     * -> box collider�� ���� ��ġ ���ѹ���?
     * 
     * �Ʒ� enum�� ���� type�� ��������.
     * deco�� obstacle�� ���� �ʿ����, monster�� npc�� �̹�.. monster�� �ʿ��Ѱ� �±��ѵ�...������
     */
    private enum ObjType
    {
        Decoration,
        Obstacle,
        Monster,
        Npc
    }

    //���� ���� ��ü��
    public Image field_base;
    public GameObject player;
    public GameObject field_frame;

    //��� ����
    private string path = @"/Resource/Text/Battle/StageFreeSet/";
    private string field_free_set;
    private JToken jbase;

    public void Setting(string freeset) //1.string Freeset name.json
    {
        //foreach�� ����� for? �ƴ� while

        //json�ε�
        field_free_set = Application.dataPath + path + freeset + ".json";
        ConvertJson convertJson = new ConvertJson();
        string str = convertJson.MakeJson(field_free_set);
        jbase = JObject.Parse(str);
        JToken jset = jbase["set"];
        
        //�ʵ� ��� ��ȯ
        //�̰Ÿ�.... �ǹ��ִ� ������.... �� �浹 ���� ����.
        field_base.sprite = Resources.Load<Sprite>("Picture/" + jset["background"].ToString());

        //enemy�� �߰���, state����� �׷��� �߰��� ����. level�̶����. ���� �� ��ü�� ��ũ��Ʈ������

        //����, �ɽ�, ĳ���� ���� �� ��ġ
        string[] list = { "decoration", "obstacle", "monster" };
        for(int i = 0; i < 3; i++)
            foreach(JToken jtmp in jset[list[i]])
                Generate(i, jtmp);

        //npc ����
        
        //player��ġ + Ư��ȿ��..?
        Vector3 pos = new Vector3(700, 150, 0);
        player.GetComponent<RectTransform>().anchoredPosition = pos;

        //�ý��� ..?

        //�ʱ� ī�޶� ����
        JToken jcamera = jset["camera"];
        field_frame.transform.position = new Vector3((int)jcamera["pos"][0], (int)jcamera["pos"][1], 0);
    }

    //������ �ν��Ͻ� ���� �Լ�
    private void Generate(int type, JToken jobj)
    {
        //�� ����..?
        switch (type)
        {
            case (int)ObjType.Monster:
                //tmp.GetComponent<Enemy>().State = awake (jobj["state"].ToString());
                //tmp.GetComponent<Enemy>().Set(jobj["state"].ToString(), //level);
                break;
            case (int)ObjType.Npc:
                break;
            case (int)ObjType.Decoration:
            case (int)ObjType.Obstacle: // ���� �߰����� ����.. �׷� �� ����? ��?��
                break;
        }

        //������ ����
        GameObject prefab = Resources.Load<GameObject>(jobj["name"].ToString());
        Vector3 pos = new Vector3((int)jobj["pos"][0], (int)jobj["pos"][1], 0); // z��ǥ�� �־� ����..

        //�ν���Ʈ ����
        GameObject tmp = Instantiate(prefab, field_frame.transform);
        tmp.GetComponent<RectTransform>().anchoredPosition = pos;
    }


}
