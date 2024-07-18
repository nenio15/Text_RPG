using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesicionButton : MonoBehaviour
{
    [SerializeField] private string state = "battle";
    private BattleManager battleManager;


    // Start is called before the first frame update
    void Start()
    {
        battleManager = GameObject.Find("Battle").GetComponent<BattleManager>();
    }

    public void Press()
    {
        switch (state)
        {
            case "battle":
                battleManager.TurnStart();
                break;
            case "scenario":
                break;
            default:
                break;
        }
    }

}
