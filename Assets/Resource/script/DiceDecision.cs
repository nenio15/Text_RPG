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
        //���� ����
        enemy = GameObject.Find("Enemy1");

        selectionManager = GameObject.Find("selectpaper").GetComponent<SelectionManager>();
        //selectionManager.ShowSelection("Action", 0, 1);
    }

    public void Desicion(string enemy_name)
    {
        //�̰� ������ �ߵ��̸�... ��... ��ȹ �ٽ� ����.
        InstantStrategy();
    }


    //�� �º�
    public void DesicionWinner(string enemy_name)
    {
        

        //player = GameObject.Find(player_name);
        enemy = GameObject.Find(enemy_name);

        //���� ��� ����
        string pSkill = player.GetComponent<CharacterData>().player_info.Skill;
        string eSkill = enemy.GetComponent<BattleEnemy>().enemy.Skill;

        //������ ���(���̽���) �¸��ڸ� �������´�.
        //�׷��� �¸������̶� �� ����� ������ �����ش�.
        Debug.Log(eSkill + " vs " + pSkill);

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
            player_win = false;
        }
        
        //�Ѵ� �ʿ�?
        enemy.GetComponent<BattleEnemy>().EndTurn(player_win);
        player.GetComponent<BattlePlayer>().EndTurn(player_win);
        playerUiManager.UploadToGame();


        //���� ���� ������ �ʿ�.
        EndCheck();
    }

    public void Press()
    {
        //�����̶�� ���� ����. �Žñ⸦ �Ѵ�.
        player.GetComponent<BattlePlayer>().StartCoroutine("UpdateRun");
        enemy.GetComponent<BattleEnemy>().StartCoroutine("UpdateRun");

        //�׷�����?

    }

    private void EndCheck()
    {
        //üŷ�� enemy imgae rendering�� Ȱ��ȭ ���η�.. // collider

        selectionManager.ShowSelection("Action", 0, 1);

    }

    private void InstantStrategy()
    {
        //enemy.Getcomponent<>.weapon
        selectionManager.ShowSelection("Club", 0, 2);
    }


}
