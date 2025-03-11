using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityCalculator
{


    public string Probability(string region) {
        EventList eventList = new EventList();
        (string, double)[] pair = eventList.RegionFind(region); //이거 됨? 왜됨?

        //이거 float에 double.. 오버플로우 걱정이 좀 드네..

        //1.총 가중치 합 계산
        double totalweight = 0;
        for(int i = 0; i < pair.Length; i++) totalweight += pair[i].Item2;

        // 2. 주어진 가중치를 백분율로 치환 (가중치 / 총 가중치)pair[i].weight / totalweight; ? 굳이 계산을 그렇게 나아야가하나..?
        // 3. 가중치의 오름차순으로 정렬... -> 이거를... 흠.. 굳이..?


        //0.확률 계산.
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
