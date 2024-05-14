using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

[System.Serializable]
public class Character
{
    public int Level = 1;
    public int[] Hp = { 3, 3 };
    public int[] Mp = { 0, 0 };
    public int[] Stat = { 1, 1, 1, 1, 1, 1 };
    //int money;
    //int exp;

    public string Name = "홍길동";
    public string Class = "Warrior";
}
// 퀘스트는 진행정도를 기록.. 필요..


public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject cur_class;  // 현재는 텍스트, 나중엔 이미지

    [Header("TEXT_LIST")]
    [SerializeField] private Text level;
    [SerializeField] private Text stats;
    [SerializeField] private Text bar;
    [SerializeField] private Text classname;

    
    private string  charoute;
    public Character cur_info;
    private JObject player;

    //TextAsset jsonData = Resources.Load("Text/Battle/Player") as TextAsset;
    //var _data = JsonUtility.FromJson<Character>(jsonData.ToString());
    //string jsonstring = JsonUtility.ToJson(object obj);
    //T obj = JsonUtility.FromJson<T>(jsonData);
    private void Start()
    {
        charoute = Application.dataPath + @"\Resource\Text\Player.json";
        string cha = MakeJson(charoute);

        // first set
        player = JObject.Parse(cha);
        JToken info = player["Info"];
        JToken bag = player["Backpack"];

        // "{\"Level\":2,\"Hp\":[3,3],\"Mp\":[1,1],\"Stat\":[4,3,1,1,2,3],\"Name\":\"홍길동\",\"Class\":\"Wariror\"}"
        // 내가 원하는 형식으로 안 나온다. (찾아보면 있을지도...)
        // string newone = JsonUtility.ToJson(list);

        //int level = (int)player["Info"]["Level"]; 이거의 단락화랄까..?
        cur_info = JsonUtility.FromJson<Character>(info.ToString());
        
        //반영
        UploadGame();

        //UploadData();
    }

    // 갱신 형식 고안해볼것. 매개변수 써서 말이지.( or fun("Level", 2) )
    public void Renew()
    {
        player["Info"]["Level"] = 2;
    }

    public void UploadData()
    {
        File.WriteAllText(charoute, player.ToString());
    }

    //게임화면에 반영하기
    public void UploadGame()
    {
        level.text = cur_info.Level.ToString();
        stats.text = cur_info.Stat[0] + "\t\t" + cur_info.Stat[2] + "\t\t" + cur_info.Stat[4] + "\n" + cur_info.Stat[1] + "\t\t" + cur_info.Stat[3] + "\t\t" + cur_info.Stat[5];
        bar.text = "Hp : " + cur_info.Hp[0] + "/" + cur_info.Hp[1] + " Mp : " + cur_info.Mp[0] + "/" + cur_info.Mp[1];
        classname.text = cur_info.Name;
    }


    private string MakeJson(string jpath)    //parsing안 하고 그냥 넣는거는 안되나? 굳이..?
    {
        string str = null;
        using (StreamReader sr = File.OpenText(jpath))
        using (JsonTextReader reader = new JsonTextReader(sr))
        {
            str = sr.ReadToEnd();
            sr.Close();
        }
        return str;
    }

}
