using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager instance {
        get
        {
            if (m_instance == null) m_instance = FindObjectOfType<NarrativeManager>();
            return m_instance;
        }
    }

    private static NarrativeManager m_instance;


    private void Awake()
    {
        if(instance != this) Destroy(gameObject);
    }

    //������ ����. 1.�нú� 2.�����ɼ� 3.�������ɼ� 4.��Ƽ��




    //2.���� �ɼ� �Լ���
    /*
     * �� ���̵� �ֳ�. Ÿ�ٿ��� ������ ��ġ�°� ������ ���µ�. ������� ������ ��ġ�°�. �ʵ忡 ������ ��ġ�°�. �׷��� ���簡 ���絥 ������. �ҹ��������� �ʵ�ȿ��?
     * �ʵ�ȿ�� �����ϸ� �Ӹ��� ����������. ������ ��ĸ� ã���� ���߿��� �������θ� ����ϸ� �Ǵϱ� �׳��� �����ѵ�.
     * 
     */
    public void CallByStageSet()
    {

    }

    public void CallByManager(GameObject target)
    {
        //�������� �ٽ��ѹ�. Ÿ���� ��� �ִ�. + ü���� 0 ���ϴ�.
        //�������°��� ����. Ÿ�� ��� �ִ�. + �Ŵ������� wave�� �� �� °���� Ȯ���Ѵ�. Ÿ�ٿ��� ������ ��ģ��.
        // a ? -> Ÿ���� ������ �ο����� ������ ��ģ��. ��? ������. �̰Ŵ� ����� ���������ٵ�..

    }

    public void CallByCombat(GameObject target)
    {
        //�÷��̾ �׷���鿡�� �� �ִ°�. ���⼭ ���� �ݿ��� �� �տ� �ݿ������� ���� �ǹ�.
        //���� �ϴ� - Ÿ�� ��� �ִ� - ���� �ϸ鼭 ȯ���� �Ŵ������Լ�? �޴´�. ȯ���� ����. Ÿ�ٿ��� ����ġ�� �ش�. -> accurate. ����. ���� 0. '�̹���'
        
    }

    public float MeddleInCombat(GameObject target)
    {
        //���� ������ - �� ���� - Ÿ���� ��� - ����Ȯ���� ������. - ������. �� 100�� ������.

        //100�̸� ������. 999�� �� �ٽ� ����. �׷� ��������.
        return 0.0f;
    }

    
    /*
     * 1.�����
     * 2.Ÿ�ݽ�
     * 3.�ǰݽ�
     * 4.���̺� üũ(����? ��?)
     * 5.��� �����
     * 6.��(�ֻ���) ���� ��.
     * 7.
     * 
     * �� ������ ��� ����?
     * 1.������ ���� �ܺ� ��ũ��Ʈ���� ȣ��ȴ�.(�� ����)
     * 2.�ش� ��Ȳ�� �´� ���縦 ���߸���.
     * 3.����� �� ������ ������ �༮���� Ȱ��ȭ�Ѵ�.
     * 
     * ��������. -> player.hit���� ȣ�� ���� / combatcalculator���� ������ �̸� �߻� ȣ�� ����
     * �������°� -> ���̺�. - battlemanager���� ����. + ���縦 ���� ��ü ��ü �� ���� / battlemanager �� ���� �� �� ��ü Ȯ��
     * �������� -> ���� ��ü�� ����. - player.hit / combat damage.
     * �����ǿ�� -> combatcalculator ���̽� �� ���� ȣ�� ����.
     */


}
