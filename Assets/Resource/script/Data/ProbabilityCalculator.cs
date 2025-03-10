using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityCalculator
{


    public string Probability(string region) {
        EventList eventList = new EventList();
        (string, double)[] pair = eventList.RegionFind(region); //�̰� ��? �ֵ�?

        //�̰� float�� double.. �����÷ο� ������ �� ���..

        //1.�� ����ġ �� ���
        double totalweight = 0;
        for(int i = 0; i < pair.Length; i++) totalweight += pair[i].Item2;

        // 2. �־��� ����ġ�� ������� ġȯ (����ġ / �� ����ġ)pair[i].weight / totalweight; ? ���� ����� �׷��� ���ƾ߰��ϳ�..?
        // 3. ����ġ�� ������������ ����... -> �̰Ÿ�... ��.. ����..?


        //0.Ȯ�� ���.
        double pivot = UnityEngine.Random.Range(0, (int)totalweight);
        double acc = 0;
        for(int i = 0; i < pair.Length; i++)
        {
          acc += pair[i].Item2;
          if(pivot <= acc) return pair[i].Item1;
        }
        
        return "error";
    }

}
