using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CharlistManager : MonoBehaviour
{
    /*
     * �ϴ�. json questlist�� ������.
     * �׸��� quest������ ��Ƴ��� ���� json�� �ʿ�.
     * �׸��� update����� �������� ����� ����. -> ��� ������ ������ ��ũ��Ʈ��... �Ƴ�.. �̰Ŵ� ������ ����. ���߿� ������ �����ϰԲ�.
     * �ǹ�. -> �׷� ������ ���� ����? ������.
     */

    [SerializeField] private GameObject[] charlists;

    //Json ���� ����
    private string charlist_route;
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;

    /*
     * ���߿� ����Ʈ �ڵ� ���� ��. �ڵ� ���� �� ������ ����Ұ�. - 
     * !ĭ ��ü�� ������ ����
    */

    public static CharlistManager Instance;
    private CharacterScenePass moveStartScene;
    private int select_idx = -1;
    private string select_id;

    private void Awake()
    {
        Instance = this;
        charlist_route = Application.persistentDataPath + "/Charlist.json";
        UpdateList();

        moveStartScene = GetComponent<CharacterScenePass>();
    }

    //list fixed but, flexible update needed
    public void UpdateList()
    {
        //�� ����Ʈ���� �����ؼ� �����ϱ�.
        //questlists[j].GetComponentInChildren<TextMeshProUGUI>();
        
        
        string str = convertJson.MakeJson(charlist_route);
        jroot = JObject.Parse(str);

        int i = 0;
        foreach (JToken character in jroot["List"])
        {
            //���� ������.
            if (character["name"] == null) { Debug.LogError("List has non foramt one"); continue; }
            charlists[i].GetComponent<CharlistSlot>().Set(character["name"].ToString(), character["id"].ToString(), i);

            i++;
        }
        //list�� �������� �ּ�ġ�� ����ġ ���� ���
        for (; i < charlists.Length; i++) charlists[i].GetComponent<CharlistSlot>().Set("", "", i);

    }

    public void DeleteOne()
    {
        if (select_idx < 0) return;
        
        JArray list = (JArray)jroot["List"];
        var itemToRemove = list.FirstOrDefault(obj => obj["id"]?.ToString() == select_id);
        if (itemToRemove != null) list.Remove(itemToRemove);

        //���� �����
        string route = UnityEngine.Application.persistentDataPath;
        File.WriteAllText(route + "/Charlist.json", jroot.ToString());
        Debug.Log(jroot["List"].ToString());

        //persistent�� ���̵����� �������׳� ������ ���´٤����� - �̴� ���� ó�����־���Ѵ�.
        UpdateList();
    }

    public void Selected(string id, int i)
    {
        select_idx = i;
        select_id = id;
        moveStartScene.IdSelect(id);
    }

    
}