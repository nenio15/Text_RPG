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
    
    public string Name = "ȫ�浿";
    public string Class = "Warrior";
    public string Skill = "Rock";
}

public class CharacterData : MonoBehaviour
{
    //json ��Ʈ
    private string charoute;
    public JObject player;

    //player�� ���� ��Ʈ ������
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
        
        //json���� player �������� �������
        charoute = Application.dataPath + @"\Resource\Text\Info\Player.json";
        string cha = new ConvertJson().MakeJson(charoute); //jsonconverter �ִµ�?
        
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
                player["Info"]["Skill"] = content; //json�� �ݿ��ǳ�, �ߺ����� �����̴�. �ٸ� ���⵵�� ������ �ϴ� �̷��� �д�.
                break;
            case 1:
                break;
            default:
                break;
        }

        UploadToData();

    }

    //Json���Ͽ� �ݿ�
    private void UploadToData()
    {
        File.WriteAllText(charoute, player.ToString());
    }

}
