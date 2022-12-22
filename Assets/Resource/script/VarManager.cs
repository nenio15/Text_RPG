using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarManager : MonoBehaviour
{
    public static int[] forest_node = new int[100];

    void Start()        //초기에 호출될 문서들은. 1.지역 2.마을 이다. 상황에 따라 추가시나리오, 던전, 몬스터 따위를 추가로 받는다.(언제받을지는 모르겠다리..)
    {
        
        string forest_regionroute = Application.dataPath + @"\Resource\Text\Field\Forest.txt";
        string[] forest_region = System.IO.File.ReadAllLines(forest_regionroute);
        //int[,] region_node = new int[50, 100];
        
        Search(ref forest_node, forest_region);
        
        
    }
    
    void Update()
    {
        
    }
    
    void Search(ref int[] node, string[] region)
    {
        string[] numbers;
        int num;
        for (int i = 0; i < region.Length; i++)
        {
            
            if (region[i].Contains("@"))
            {
                numbers = region[i].Split(' ');
                num = int.Parse(numbers[2]);            //@ 001 '01' 뒷번호만 사용
                //Debug.Log(numbers[1] + ", "+ numbers[2] +", "+ num);
                node[num] = i;
                string on = node[num].ToString();
                //Debug.Log(node[num]);
                //string mainroute = Application.dataPath + @"\Resource\Text\main.txt";
                Debug.Log("VAR[region] : " + on);
                //System.IO.File.AppendAllText(mainroute, on);
            }
            
        }
    }

    void Select()   //키워드의 선택지리스트 + 선택지로 인한 변화점 호출
    {

    }
    


}
