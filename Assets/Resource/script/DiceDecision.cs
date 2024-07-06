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



    //�̷������� ������ ��ư �̰�� ��������
    public void DesicionWinner(string enemy_name)
    {
        enemy = GameObject.Find(enemy_name);
        //player = GameObject.Find(player_name);
        //���׹̰� ������. �÷��̾ ������.

        string pSkill = "Paper";//player.GetComponent<BattlePlayer>().player.Skill;
        string eSkill = enemy.GetComponent<EnemyInfo>().enemy.Skill;

        //������ ���(���̽���) �¸��ڸ� �������´�.
        //�׷��� �¸������̶� �� ����� ������ �����ش�.
        //if(player is win

        if (pSkill == eSkill)
        {
            Debug.Log("���");
            //���� ���� ���µ�...
        }
        else if ((pSkill == "Scissors" && eSkill == "Paper") ||
                     (pSkill == "Rock" && eSkill == "Scissors") ||
                     (pSkill == "Paper" && eSkill == "Rock"))
        {
            //Debug.Log("player�̱�");
            player_win = true;
        }
        else 
        {
            //Debug.Log("enemy�̱�");
            player_win = false;
        }
        
        //�Ѵ� �ʿ�?
        enemy.GetComponent<EnemyInfo>().EndTurn(player_win);
        player.GetComponent<BattlePlayer>().EndTurn(player_win);
        playerUiManager.UploadToGame();
    }

    public void Press()
    {
        //�����̶�� ���� ����. �Žñ⸦ �Ѵ�.
        player.GetComponent<BattlePlayer>().StartCoroutine("Run");
        enemy.GetComponent<EnemyInfo>().StartCoroutine("UpdateRun");

        //�׷�����?

    }

    private void Start()
    {
        enemy = GameObject.Find("Enemy1");
    }
}
