using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Character
{
    int Level = 1;
    int[] Hp = { 3, 3 };
    int[] Mp = { 0, 0 };
    int[] Stat = { 1, 1, 1, 1, 1, 1 };
    //int money;
    //int exp;

    string Name = "ȫ�浿";
    string Class = "Warrior";
}
// ����Ʈ�� ���������� ���.. �ʿ�..


public class CharacterManager : MonoBehaviour
{
    private void Start()
    {

    }



}

//TextAsset jsonData = Resources.Load("Text/Battle/Player") as TextAsset;
//var _data = JsonUtility.FromJson<Character>(jsonData.ToString());
//var _dd = JsonUtility.ToJson(_data);