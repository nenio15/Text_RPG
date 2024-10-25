using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleAction
{
    public battleAction()
    {
        index = 0;
    }

    public battleAction(int idx)
    {
        index = idx;
    }

    public battleAction(string name, int index, string type, string img, float damage, float percent)
    {
        this.name = name;
        this.index = index;
        this.type = type;
        this.img = img;
        this.damage = damage;
        this.percent = percent;
    }

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
        battleAction[] act = {new battleAction(0), new battleAction(1), new battleAction(2)};
        actions = act;
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
