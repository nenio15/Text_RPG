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
    [SerializeField] private SelectionManager selectionManager;



    private void Start()
    {
        player = GameObject.Find("Player");
        //임의 세팅
        enemy = GameObject.Find("Enemy1");

        selectionManager = GameObject.Find("selectpaper").GetComponent<SelectionManager>();
        //selectionManager.ShowSelection("Action", 0, 1);
    }

    public void Desicion(string enemy_name)
    {
        //이거 무조건 발동이면... 흠... 기획 다시 볼것.
        InstantStrategy();
    }


    //합 승부
    public void DesicionWinner(string enemy_name)
    {
        

        //player = GameObject.Find(player_name);
        enemy = GameObject.Find(enemy_name);

        //서로 기술 정의
        string pSkill = player.GetComponent<CharacterData>().player_info.Skill;
        string eSkill = enemy.GetComponent<BattleEnemy>().enemy.Skill;

        //모종의 방식(다이스로) 승리자를 결정짓는다.
        //그러면 승리판정이랑 각 계수의 혜택을 돌려준다.
        Debug.Log(eSkill + " vs " + pSkill);

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
            player_win = false;
        }
        
        //둘다 필요?
        enemy.GetComponent<BattleEnemy>().EndTurn(player_win);
        player.GetComponent<BattlePlayer>().EndTurn(player_win);
        playerUiManager.UploadToGame();


        //전투 종료 판정이 필요.
        EndCheck();
    }

    public void Press()
    {
        //한턴이라는 개념 동안. 거시기를 한다.
        player.GetComponent<BattlePlayer>().StartCoroutine("UpdateRun");
        enemy.GetComponent<BattleEnemy>().StartCoroutine("UpdateRun");

        //그럴려면?

    }

    private void EndCheck()
    {
        //체킹을 enemy imgae rendering의 활성화 여부로.. // collider

        selectionManager.ShowSelection("Action", 0, 1);

    }

    private void InstantStrategy()
    {
        //enemy.Getcomponent<>.weapon
        selectionManager.ShowSelection("Club", 0, 2);
    }


}
