using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceDecision : MonoBehaviour
{
    public bool player_win = false;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private PlayerUiManager playerUiManager;



    //이런저런게 있지만 암튼 이겻고 결정해줘
    public void DesicionWinner(string enemy_name)
    {
        enemy = GameObject.Find(enemy_name);
        //player = GameObject.Find(player_name);
        //에네미가 누군데. 플레이어가 누군데.

        string pSkill = "Paper";//player.GetComponent<BattlePlayer>().player.Skill;
        string eSkill = enemy.GetComponent<EnemyInfo>().enemy.Skill;

        //모종의 방식(다이스로) 승리자를 결정짓는다.
        //그러면 승리판정이랑 각 계수의 혜택을 돌려준다.
        //if(player is win

        if (pSkill == eSkill)
        {
            Debug.Log("비김");
            //비긴건 뭐가 없는데...
        }
        else if ((pSkill == "Scissors" && eSkill == "Paper") ||
                     (pSkill == "Rock" && eSkill == "Scissors") ||
                     (pSkill == "Paper" && eSkill == "Rock"))
        {
            //Debug.Log("player이김");
            player_win = true;
        }
        else 
        {
            //Debug.Log("enemy이김");
            player_win = false;
        }
        
        //둘다 필요?
        enemy.GetComponent<EnemyInfo>().EndTurn(player_win);
        player.GetComponent<BattlePlayer>().EndTurn(player_win);
        playerUiManager.UploadToGame();
    }

    public void Press()
    {
        //한턴이라는 개념 동안. 거시기를 한다.
        player.GetComponent<BattlePlayer>().StartCoroutine("Run");
        enemy.GetComponent<EnemyInfo>().StartCoroutine("UpdateRun");

        //그럴려면?

    }

    private void Start()
    {
        enemy = GameObject.Find("Enemy1");
    }
}
