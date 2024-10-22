using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judgement : MonoBehaviour
{
    public bool player_win = false;
    //[SerializeField] private GameObject player;
    //[SerializeField] private GameObject enemy;
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private BattleManager battleManager;


    private void Start()
    {
        //player = GameObject.Find("Player");
        playerUiManager = FindObjectOfType<PlayerUiManager>();
        selectionManager = FindObjectOfType<SelectionManager>();

        //임의 세팅
        //enemy = GameObject.Find("Enemy1");
        battleManager = FindObjectOfType<BattleManager>();
        //selectionManager.ShowSelection("Action", 0, 1);
    }

    //합 승부 //값 계산해서 돌려줘.
    public void DesicionWinner(GameObject player, GameObject target, string strategy)
    {
        //Debug.Log(strategy);

        //서로 기술 정의

        string pSkill;
        string eSkill;
        if (player.GetComponent<PlayerHealth>() != null) {
            pSkill = player.GetComponent<PlayerHealth>().player_info.Skill;
            eSkill = target.GetComponent<BattleEnemy>().enemy.Skill;
        } else {
            pSkill = player.GetComponent<BattleEnemy>().enemy.Skill;
            eSkill = target.GetComponent<PlayerHealth>().player_info.Skill;
        }

        //모종의 방식(다이스로) 승리자를 결정짓는다.
        //그러면 승리판정이랑 각 계수의 혜택을 돌려준다.
        //Debug.Log(eSkill + " vs " + pSkill);

        if (pSkill == eSkill)
        {
            //Debug.Log("비김");
            //비긴건 뭐가 없는데...
            //변수형을 string이나 enum으로 바꿀것. (-1 0 1)
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
        
        //둘다 필요? ... 이거 battle에게 전담할것.
        //enemy.GetComponent<BattleEnemy>().EndTurn(player_win);
        //player.GetComponent<BattlePlayer>().EndTurn(player_win);
        //playerUiManager.UploadToGame();

        //전투 종료 판정
        battleManager.BattleEndCheck(player, target, player_win);
    }

    //순간전략 활성화
    private void InstantStrategy()
    {
        //enemy.Getcomponent<>.weapon
        //임의로 club
        selectionManager.ShowSelection("Club", 0, 2);
    }


}
