using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System;
//using static UnityEditor.LightingExplorerTableColumn;

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
    public string Skill = "Rock";
}

public class CharacterData : MonoBehaviour
{
    //json 루트
    private string charoute;
    public JObject player;

    //player의 메인 루트 데이터
    public Character player_info;


    enum DataType
    {
        Skill,
        Hp,
        Mp,
        Exp
    }

    // Start is called before the first frame update
    private void Start()
    {
        
        //json에서 player 인적정보 끌어오기
        charoute = Application.dataPath + @"\Resource\Text\Info\Player.json";
        string cha = new ConvertJson().MakeJson(charoute); //jsonconverter 있는듯?
        
        // first set
        player = JObject.Parse(cha);
        JToken info = player["Info"];
        JToken bag = player["Backpack"];

        player_info = JsonUtility.FromJson<Character>(info.ToString());
    }

    public void UpdateData(int type, string content)
    {
        switch (type)
        {
            case (int)DataType.Skill: 
                player_info.Skill = content; 
                player["Info"]["Skill"] = content; //json에 반영되나, 중복적인 선언이다. 다만 복잡도를 생각해 일단 이렇게 둔다.
                break;
            case 1:
                break;
            default:
                break;
        }

        UploadToData();

    }

    //Json파일에 반영
    private void UploadToData()
    {
        File.WriteAllText(charoute, player.ToString());
    }

}
