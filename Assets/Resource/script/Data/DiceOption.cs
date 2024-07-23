using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceOption
{
    //public string[] optionList = { "muscle", "reaction", ... };

    //스탯관련 우려가... 1.0을 넘는게 잇고 아닌게 잇어. 그러면 선택지간의 밸런스가 해쳐진단 말이지.
    //1.1.0 총합을 지킨다. 1.0을 넘으면 그 기준으로 난이도를 맞춘다. 그렇게.
                            //str, dex, int, luc, cha, sen
    public float[] muscle = { 1.0f, 0.3f, 0, 0, 0, 0.1f };
    public float[] reaction = { 0.2f, 1.0f, 0, 0.1f, 0, 0.5f };
    public float[] wisdom = { 0, 0, 1.0f, 0, 0, 0.2f };
    public float[] lucky = { 0, 0, 0, 1.0f, 0.1f, 0 };
    public float[] dignity = { 0, 0, 0, 0.1f, 1.0f, 0.1f };
    public float[] sixsense = { 0, 0.1f, 0.1f, 0.1f, 0, 1.0f };
    public float[] resistance = { 1.0f, 0, 0.1f, 0, 0, 0.3f };
    public float[] breakdown = { 0, 0, 1.0f, 0, 0, 0.3f };
    public float[] chase = { 0, 1.0f, 0.3f, 0, 0.1f, 0.5f };
    public float[] trick = { 0, 0, 1.0f, 0, 0.5f, 0.5f };
    public float[] persuasion = { 0.3f, 0, 0, 0.3f, 1.0f, 0.5f };
    
}
