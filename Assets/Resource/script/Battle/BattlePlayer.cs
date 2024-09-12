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

    //플레이어는 자신이 rock, scissor, paper를 골라야하는데.... 그건 select를 불러와야해
    //그게 기억이 안나네..
    public void EndTurn(bool win)
    {
        if (win) Debug.Log("I win");
        else
        {
            characterData.player_info.Hp[0] -= 1;
            playerUiManager.UploadToGame();
        }
        //여기서 hp+-같은거랑 이거저거.. 그러면 물론, 계수를 인수로 받아야겠지?


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
