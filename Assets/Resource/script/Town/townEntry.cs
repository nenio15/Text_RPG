using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class townEntry : MonoBehaviour
{
    /*
     2024-01-04
     �� ���ذ� ��ҽ��ϴ�. �������̳׿�

    Ÿ���� �����ΰ�?
    �����Դϴ�.
    �۸� ����� ������ �д°�?

    ������ ���� ���� + �������Դϴ�.
    ������ ������, ������ ������ ������ ����.

    �ΰ����� �ǵ�� �˴ϴ�.
    ���� �������� �ǵ鿩�ߵǰ��.
     
     */

    // ����� �Ŵ����� ���ؾ��մϴ�. �Ŵ����� ü������ ������.
    private Textchanger tchanger = new Textchanger();

    void Entry(string jmain, string jsub)
    {
        //jmain jsub�� �ش� ������ ã�ƺ��ô�. ���⼭ ���簡 �ǿ�.
        tchanger.readScenarioParts(0, "town", "plain_town");

        //���� btn�� �˾Ƽ� ã�ŵ��? ����ΰ��
    }
}
