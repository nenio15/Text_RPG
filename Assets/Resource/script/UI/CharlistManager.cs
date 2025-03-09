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
     * 일단. json questlist를 만들자.
     * 그리고 quest형식을 담아내는 개별 json도 필요.
     * 그리고 update방식은 아이템의 방식을 차용. -> 사실 구조가 같으면 스크립트도... 아냐.. 이거는 구분이 편해. 나중에 수정이 용이하게끔.
     * 의문. -> 그럼 모듈식이 낫지 않음? 흠좀무.
     */

    [SerializeField] private GameObject[] charlists;

    //Json 관련 선언
    private string charlist_route;
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private JObject jroot;

    /*
     * 나중에 리스트 자동 생성 및. 자동 삭제 열 줄임을 고려할것. - 
     * !칸 자체의 삭제가 없음
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
        //각 리스트마다 참조해서 수정하기.
        //questlists[j].GetComponentInChildren<TextMeshProUGUI>();
        
        
        string str = convertJson.MakeJson(charlist_route);
        jroot = JObject.Parse(str);

        int i = 0;
        foreach (JToken character in jroot["List"])
        {
            //예외 날리기.
            if (character["name"] == null) { Debug.LogError("List has non foramt one"); continue; }
            charlists[i].GetComponent<CharlistSlot>().Set(character["name"].ToString(), character["id"].ToString(), i);

            i++;
        }
        //list의 가짓수가 최소치를 만족치 못한 경우
        for (; i < charlists.Length; i++) charlists[i].GetComponent<CharlistSlot>().Set("", "", i);

    }

    public void DeleteOne()
    {
        if (select_idx < 0) return;
        
        JArray list = (JArray)jroot["List"];
        var itemToRemove = list.FirstOrDefault(obj => obj["id"]?.ToString() == select_id);
        if (itemToRemove != null) list.Remove(itemToRemove);

        //파일 덮어쓰기
        string route = UnityEngine.Application.persistentDataPath;
        File.WriteAllText(route + "/Charlist.json", jroot.ToString());
        Debug.Log(jroot["List"].ToString());

        //persistent에 더미데이터 ㅁㅁㅁ그냥 폴더는 남는다ㅁㅁㅁ - 이는 따로 처리해주어야한다.
        UpdateList();
    }

    public void Selected(string id, int i)
    {
        select_idx = i;
        select_id = id;
        moveStartScene.IdSelect(id);
    }

    
}