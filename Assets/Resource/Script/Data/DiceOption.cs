using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceOption
{
    //public string[] optionList = { "muscle", "reaction", ... };

    //���Ȱ��� �����... 1.0�� �Ѵ°� �հ� �ƴѰ� �վ�. �׷��� ���������� �뷱���� �������� ������.
    //1.1.0 ������ ��Ų��. 1.0�� ������ �� �������� ���̵��� �����. �׷���.
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
