using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Text battle_f;
    [SerializeField] private Text battle_t;


    
    [Header("OBJ_INTERATION")]
    [SerializeField]private int robj_i = 0;
    [SerializeField] private GameObject[] robj;

    [SerializeField] private GameObject clickobj;

    private string battlefield;
    private string monroute;
    private string proute;
    private string path = @"\Resource\Text\Battle\";

    // 대충 배틀 관리자 입니다 예.

    // Start is called before the first frame update
    private void Start()
    {
        battlefield = Application.dataPath + path + @"Field\BattleField.json";
        monroute = Application.dataPath + path + "Monster.json";
        proute = Application.dataPath + path + "Player.json";
    }

    public void CallBattle()
    {

    }

    public void DuringBattle()
    {

    }

    public void EndBattle(string detail) //전투포기 같은거 용도..
    {

    }

}
