using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EventInformer : MonoBehaviour
{
    // ���⼭ �̺�Ʈ���� ���Ͽ��θ� ����
    // Ȯ���� ��� �׷�. ����
    // nearby�� textchanger���Լ� ����.
    // �װŸ� �ݿ���. �Ŵ�������?
    // �ٸ� ��ũ��Ʈ���� �˷��ִ� �� ����. �ٵ� ���°� �� ��ũ��Ʈ�� ���� �س�����.

    // Start is called before the first frame update
    [SerializeField] private Textchanger textchanger;


    // �ʿ���� �Լ�. jmp���� ��� �ɷ� ���� �ذ��Ұž�.
    public void getNear() // �� ��ũ��Ʈ�� ���� �˷���? �ƴ�, �Ŵ����� �ʿ��Ҷ� �ҷ�?
    {
        foreach(JToken jnear in textchanger.jbase["nearby"]) // nearby ���� ���س�!!!!
        {
            Debug.Log(jnear.ToString());

        }

    }
}
