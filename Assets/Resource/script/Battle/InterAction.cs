using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Narrative //서사
{
    [Header("INFO")]
    public string name;
    public string img;
    public string describe;
    public string type; // 4. 3. 흠

    //조건
    //효과
    //발동여부 - 이번턴. - 이거를 어떻게 잴까..
    //스택여부
    //스택.

    [Header("EFFECT")]
    public BattleEffect[] buff;
    public BattleEffect[] debuff;

    [Header("CORRECTION")]
    public BattleEffect[] need;
    public BattleEffect[] more;

    //좀 더 나은 방식이 있을거 같은데.
    public bool stacked = false;
    public int stack = 0;
    public int max_stack = 0;
}

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
    public BattleAction[] actions { get; protected set; } // 3으로 고정.
    public List<Narrative> narratives { get; protected set; } // 얘는 늘어나는데.. list어떰?
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
