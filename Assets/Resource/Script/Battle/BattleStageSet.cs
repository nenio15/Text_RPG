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
        field_base.sprite = Resources.Load<Sprite>("Picture/" + jset["background"].ToString());
        //Debug.Log(jset["background"].ToString());
        //enemy�� �߰���, state����� �׷��� �߰��� ����. level�̶����. ���� �� ��ü�� ��ũ��Ʈ������

        //����, �ɽ�, ĳ���� ���� �� ��ġ
        string[] list = { "decoration", "obstacle", "monster" };
        for(int i = 0; i < 3; i++)
            foreach(JToken jtmp in jset[list[i]])
                Generate(i, jtmp);

        //npc ����

        //player��ġ
        //Vector3 pos = new Vector3(700, 150, 0);
        JToken jplayer = jset["player"];
        player.GetComponent<RectTransform>().anchoredPosition = new Vector3((int)jplayer["pos"][0], (int)jplayer["pos"][1] ,0);

        //�ý���

        //�ʱ� ī�޶� ����
        JToken jcamera = jset["camera"];
        //field_frame.transform.position = new Vector3((int)jcamera["pos"][0], (int)jcamera["pos"][1], 0);

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


    public void EndBattle(bool win, GameObject player)
    {

        JToken jget = jbase["reward"];
        //int i = 0;

        NarrativeManager.instance.CallByStageSet();

        //����ȭ�� ���
        reward.SetActive(true);

        gold.text = jget["gold"].ToString() + "G";
        exp.text = jget["exp"].ToString();

        //ȹ��������� ������, ������ ����Ʈ �߰�.
        foreach (JToken drop in jget["get"])
        {
            Itemlist dropItem = JsonUtility.FromJson<Itemlist>(drop.ToString());

            //����ȭ�鿡 �߰�.
            GameObject prefab = Resources.Load<GameObject>("itemslot");
            GameObject tmp = Instantiate(prefab, itemGrid.transform);
            //prefab.GetComponent<ItemSlotUi>().itemslot.itemData = dictionary.SetItem(dropItem.name, dropItem.type);
            tmp.GetComponent<ItemSlotUi>().Set(dropItem); //= Resources.Load<Sprite>("Picture/Item/" + drop["name"].ToString());
            Debug.Log(tmp.GetComponent<ItemSlotUi>().itemslot.itemData.img);
            Inventory.Instance.AddItem(dropItem);
        }
        
        
    }

    //���� ���� �� ���
    public void CalculateBattle()
    {
        JToken jget = jbase["reward"];
        int i = 0;

         // ����ȣ��.
                                                    //player.GetComponent<PlayerHealth>().UpdateData("gold", i);

        //��� �̷��Ŵ� enemy�� ������̺��� ���ؼ� ���� �ϴµ�. ��..
        //win ���º�..? ����.

        //��ȭ
        i = (int)jget["gold"];
        Debug.Log(i);
        player.GetComponent<PlayerHealth>().UpdateData("gold", i);
        //����ġ
        i = (int)jget["exp"];
        player.GetComponent<PlayerHealth>().UpdateData("exp", i);
        //��Ӿ�����
        //Debug.Log(jget["drop"]);


    }

}
