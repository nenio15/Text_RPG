using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleAction
{
    public string name;
    public int index;
    public string type;
    public string img;
    public float damage;
    public float percent;
}

public class InterAction : MonoBehaviour
{
    public battleAction[] actions { get; protected set; }
    public int turnSequence {  get; protected set; }
    public bool turnEnd {  get; protected set; }

    protected virtual void OnEnable()
    {

        turnEnd = true;
        battleAction[] act = {new battleAction(), new battleAction(), new battleAction()};
        actions = act;
        for(int i = 0; i < actions.Length; i++)
        {
            actions[i].index = i;
        }
        turnSequence = 0;
    }

    public virtual void OnSetAction(battleAction action)
    {
        actions[action.index] = action;
        turnSequence = action.index;
    }

    public virtual void SwitchTurn(bool turnE)
    {
        turnEnd = turnE;
    }

}
