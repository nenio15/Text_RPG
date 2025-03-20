using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditor.PackageManager.Requests;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
    public Questlist cur_quest;

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

        //�ӽ� ����. �׽��ÿ�. ���� ���� �Ⱦ�.
        string str = convertJson.MakeJson(questlist_route);
        jroot = JObject.Parse(str);
        questTable = JsonUtility.FromJson<QuestTable>(jroot.ToString());
        cur_quest = questTable.quest[0];
        Debug.Log(cur_quest.name + cur_quest.region);
        questUi.Set(questTable.quest[0]); //, "Forest/tmp");

        UpdateList();

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
        
        foreach (JToken quest in jroot["quest"])
        {
            TextMeshProUGUI title = questlists[i].GetComponentInChildren<TextMeshProUGUI>();
            Image image = questlists[i].GetComponentInChildren<Image>();
            title.text = quest["name"].ToString();
            //image.sprite = questlists[i].
            i++;
        }

        //��ĭ�� null // ������ ����Ʈ ������ ����.
        for (; i < questlists.Length; i++)
        {
            questlists[i].GetComponentInChildren<TextMeshProUGUI>().text = " ";
        }

    }

    //Accept, complete, drop
    //state : available, progress, complete, fail
    //state����� �Բ� �ٸ� json���� �����ϴµ�. �׿� ���� ���� �߰� �ʿ�.
    public void CompleteQuest()
    {
        //reward.
        Debug.Log(cur_quest.name);
        JToken reward = new Dictionary().GetCondition("reward", cur_quest);
        Debug.Log("���� reward" + reward["gold"]);
        DropQuest();
    }

    //���� �� �ִ� ����Ʈ�� �Ѱ� �ʿ�..?
    public void AcceptQuest()
    {
        //���� �߻���, windowManger�� Add�� ����
        questTable.quest.Add(cur_quest); //������ ��ġ�� �׳�?
        Debug.Log("111");
        string tableJson = JsonConvert.SerializeObject(questTable);
        File.WriteAllText(questlist_route, tableJson);
        UpdateList();
    }

    public void DropQuest()
    {
        questTable.quest.Remove(cur_quest);
        string tableJson = JsonConvert.SerializeObject(questTable);
        File.WriteAllText(questlist_route, tableJson);        
        UpdateList();
        //view�� ���� ��.
    }

    public void OnClick(int index)
    {
        cur_quest = questTable.quest[index];
        questUi.Set(cur_quest); //, "Forest/tmp");
    }

}