using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class QuestManager : MonoBehaviour
{
    /*
     * �׸��� update����� �������� ����� ����. -> ��� ������ ������ ��ũ��Ʈ��... �Ƴ�.. �̰Ŵ� ������ ����. ���߿� ������ �����ϰԲ�.
     * �ǹ�. -> �׷� ������ ���� ����? ������.
     * 
     * �׷��� ������ ���, ��� ���ð�����?
     * quest�� �з��� �ʿ��ѵ�. ����������? �װ͵� ������ ����. �ǹ�?
     */

    [SerializeField] private GameObject[] questlists;
    [SerializeField] private QuestUi questUi;

    //Json ���� ����
    private string questlist_route;
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;

    public QuestTable questTable; //���⿡�� ������ �޾Ƽ� �ؾ߰�����? ����..
    public static QuestManager Instance;

    private void Awake()
    {
        Instance = this;
        questlist_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/Questlist.json";
        UpdateList();

        //string str = Resources.Load<TextAsset>("Text/QuestForm/" + route).ToString();
        //jquest = JObject.Parse(str);
        //JToken jcomcon = jquest["complete_condition"]; -> �ش� type�� �� case�� ���� Ȯ��
        //���� ����Ʈ�� ���� region. ���� üŷ�Ѵ�.
        //1.condition, 2.complete 3.fail ��� �������� checkout�̴�. state�� ..?

    }

    //����Ʈ ����Ʈ�� ���� ����
    public void UpdateList()
    {
        int i = 0;
        string str = convertJson.MakeJson(questlist_route);
        jroot = JObject.Parse(str);
        questTable = JsonUtility.FromJson<QuestTable>(jroot.ToString());
        
        

        //��ȣ ���̱�
        //for (int j = 0; j < items.Length; j++)
        //    items[j].index = j;
        
        //�κ��丮.json���� ������ �о �� slot�� �Ҵ�.
        foreach (JToken quest in jroot["quest"])
        {
            TextMeshProUGUI title = questlists[i].GetComponentInChildren<TextMeshProUGUI>();
            Image image = questlists[i].GetComponentInChildren<Image>();

            title.text = quest["name"].ToString();
            //image.sprite = questlists[i].
            //�׸��� ��� �ݿ�..? �ƴϾ� �װ� �ƴ���.
            i++;
        }

        //�ӽ� ����. �׽��ÿ�. ���� ���� �Ⱦ�.
        questUi.Set(questTable.quest[0]); //, "Forest/tmp");
    }

    public void OnClick(int index)
    {
        questUi.Set(questTable.quest[index]); //, "Forest/tmp");
    }

}