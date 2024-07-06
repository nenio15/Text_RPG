using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System;

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
    private JObject player;

    //player의 메인 루트 데이터
    public Character player_info;

    // Start is called before the first frame update
    private void Start()
    {
        
        //json에서 player 인적정보 끌어오기
        charoute = Application.dataPath + @"\Resource\Text\Player.json";
        string cha = new ConvertJson().MakeJson(charoute); //jsonconverter 있는듯?
        
        // first set
        player = JObject.Parse(cha);
        JToken info = player["Info"];
        JToken bag = player["Backpack"];

        player_info = JsonUtility.FromJson<Character>(info.ToString());
    }

    //Json파일에 반영
    public void UploadToData()
    {
        File.WriteAllText(charoute, player.ToString());
    }

}
