using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarManager : MonoBehaviour
{
    public static int[] forest_node = new int[100];

    void Start()        //�ʱ⿡ ȣ��� ��������. 1.���� 2.���� �̴�. ��Ȳ�� ���� �߰��ó�����, ����, ���� ������ �߰��� �޴´�.(������������ �𸣰ڴٸ�..)
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
                num = int.Parse(numbers[2]);            //@ 001 '01' �޹�ȣ�� ���
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

    void Select()   //Ű������ ����������Ʈ + �������� ���� ��ȭ�� ȣ��
    {

    }
    


}
