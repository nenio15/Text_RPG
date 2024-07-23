using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager
{
    private readonly DiceOption diceOption = new DiceOption();

    private void tmp()
    {
        float sum = 0;

        foreach(float i in diceOption.muscle)
        {
            sum += DiceRoll() * i;
        }

        Debug.Log(sum);
    }


    private int DiceRoll()
    {
        return Random.Range(0, 20);   
    }
    //대실패 0, 실패 1, 성공 2, 대성공 3
    /*
    public int DiceRoll6(int num)
    {
        int number = randint(0, 6);
        if (number == 0)
        {
            return 0;
        }
        else if (number > 0 && number < 3)
        {
            return 1;
        }
        else if (number > 2 && number < 5)
        {
            return 2;
        }
        else if (number == 5)
        {
            return 3;
        }
    }
    */
}
