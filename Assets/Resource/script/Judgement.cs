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

        //���� ����
        //enemy = GameObject.Find("Enemy1");
        battleManager = FindObjectOfType<BattleManager>();
        //selectionManager.ShowSelection("Action", 0, 1);
    }

    //�� �º� //�� ����ؼ� ������.
    public void DesicionWinner(GameObject player, GameObject enemy, string strategy)
    {
        Debug.Log(strategy);

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
            //�������� string�̳� enum���� �ٲܰ�. (-1 0 1)
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
        
        //�Ѵ� �ʿ�? ... �̰� battle���� �����Ұ�.
        //enemy.GetComponent<BattleEnemy>().EndTurn(player_win);
        //player.GetComponent<BattlePlayer>().EndTurn(player_win);
        //playerUiManager.UploadToGame();

        //���� ���� ����
        battleManager.BattleEndCheck(player, enemy, player_win);
    }

    //�������� Ȱ��ȭ
    private void InstantStrategy()
    {
        //enemy.Getcomponent<>.weapon
        //���Ƿ� club
        selectionManager.ShowSelection("Club", 0, 2);
    }


}