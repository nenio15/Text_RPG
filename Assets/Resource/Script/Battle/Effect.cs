using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    // Update is called once per frame
    void Attack()
    {
        if(battleManager.turnSequence != 0) battleManager.TurnStart();
        gameObject.transform.position = new Vector3(-10, -10, -10);
    }
}
