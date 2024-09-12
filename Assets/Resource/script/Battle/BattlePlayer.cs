using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BattlePlayer : MonoBehaviour
{
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private CharacterData characterData;

    [SerializeField] private GameObject Enemy;
    public float speed = 300.0f;

    private Transform tr;
    private Vector3 target;


    void Awake()
    {
        characterData = gameObject.GetComponent<CharacterData>();


        tr = GetComponent<Transform>();
        target = Enemy.transform.position;
        //Debug.Log(characterManager.cur_info.Skill + " this is player skill...");
        //StartCoroutine("Run");
    }

    //�÷��̾�� �ڽ��� rock, scissor, paper�� �����ϴµ�.... �װ� select�� �ҷ��;���
    //�װ� ����� �ȳ���..
    public void EndTurn(bool win)
    {
        if (win) Debug.Log("I win");
        else
        {
            characterData.player_info.Hp[0] -= 1;
            playerUiManager.UploadToGame();
        }
        //���⼭ hp+-�����Ŷ� �̰�����.. �׷��� ����, ����� �μ��� �޾ƾ߰���?


        StopCoroutine("UpdateRun");
    }    

    IEnumerator UpdateRun()
    {
        while (true) //trigger is you now... hmm
        {
            Vector3 move = new Vector3(target.x - tr.position.x, target.y - tr.position.y, 0).normalized * speed * Time.deltaTime;
            tr.position += move;

            yield return null;
        }

    }

}
