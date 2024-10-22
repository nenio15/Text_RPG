using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemAddition
{
    public string type; //함수?
    public string name;
    public float value;
}

//이거.. 흠.... ㅄ같은데? 같은 양식의 프리팹을 많이 만드는게 아닌이상... 굳이? 나는 유형이 많아질 예정이라..
//[CreateAssetMenu(fileName = "Itme", menuName = "New Item")] : ScriptableObject
[System.Serializable]
public class itemData
{
    [Header("INFO")]
    public string name;
    public string detail;
    public string img;
    public string type;
    //public bool isequipment = false;
    public int sell;

    //[Header("Stacking")]
    //public bool stack = false;
    //public int max_statck;

    [Header("Effect")]
    public ItemAddition[] effect;
    public ItemAddition[] limit;


    //... 이런식으로 만들어 놓아야하는게 맞는감..

}
