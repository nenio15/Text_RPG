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

    public int index; //json���� �� ���������� Ȯ�� ��.

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
    public BattleAction[] actions { get; protected set; } // 3���� ����.
    public List<NarrativeSlot> narratives; // ��� �þ�µ�.. list�?
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

    public virtual void SetNarrative(NarrativeSlot narrative)
    {
        if (!narratives.Contains(narrative) && narrative.name != "") //�� ������ �������� �ǹ�..
        {
            narratives.Add(narrative);
            //Debug.Log(narrative.name);
        }
    }

    //tmp
    public virtual void OnNumericalAdjust(string name, float value, string state)
    {
        int index = 0;

        switch (name)
        {
            case "damage": Calculate(actions[index].damage, value, state); break;
            case "accurate": Calculate(actions[index].accurate, value, state); break;
            case "critical": Calculate(actions[index].criticalRate, value, state); break;
        }
    }

    private void Calculate(float v, float value, string state)
    {
        switch (state)
        {
            case "none": v += value; break;
            case "rate": v += v * value; break;
            default: break;
        }

        return;
    }
    


}
