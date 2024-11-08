using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleEffect
{
    public string type;
    public int level;
    public int turn;
}

[System.Serializable]
public class BattleAction
{
    public BattleAction() { index = 0; }

    public BattleAction(int idx) { index = idx; }

    public BattleAction(string name, int index, string type, string img, float damage, float percent)
    {
        this.name = name;
        this.index = index;
        this.type = type;
        this.img = img;
        this.damage = damage;
        this.accurate = percent;
    }

    public int index; //json리딩 중 오류나는지 확인 필.

    [Header("INFO")]
    public string name;
    public string img;
    public string describe;
    public string type;

    [Header("FIGURE")]
    public float damage;
    public float accurate;
    public float criticalProbability;
    public float criticalRate;

    [Header("EFFECT")]
    public BattleEffect[] buff;
    public BattleEffect[] debuff;

    [Header("CORRECTION")]
    public BattleEffect[] need;
    public BattleEffect[] more;

}

public class InterAction : MonoBehaviour
{
    public BattleAction[] actions { get; protected set; }
    public int turnSequence {  get; protected set; }
    public bool turnEnd {  get; protected set; }

    protected virtual void OnEnable()
    {

        turnEnd = true;
        BattleAction[] act = {new BattleAction(0), new BattleAction(1), new BattleAction(2)};
        actions = act;
        turnSequence = 0;
    }

    public virtual void OnSetAction(BattleAction action)
    {
        actions[action.index] = action;
        turnSequence = action.index;
    }

    public virtual void SwitchTurn(bool turnE)
    {
        turnEnd = turnE;
    }

}
