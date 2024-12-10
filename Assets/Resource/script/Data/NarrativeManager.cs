using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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

    [SerializeField] private CombatCalculator combatCalculator;

    private static NarrativeManager m_instance;
    private string narrative_route;
    private Dictionary dictionary = new Dictionary();
    private ConvertJson convertJson = new ConvertJson();
    private string narrative_list;
    private JObject jroot;

    private void Awake()
    {
        if(instance != this) Destroy(gameObject);
        narrative_route = Application.persistentDataPath + "/Info/NarrativeList.json";
        narrative_list = convertJson.MakeJson(narrative_route);
        jroot = JObject.Parse(narrative_list);
    }

    //������ ����. 1.�нú� 2.�����ɼ� 3.�������ɼ� 4.��Ƽ��


    //2.���� �ɼ� �Լ���
    /*
     * �� ���̵� �ֳ�. Ÿ�ٿ��� ������ ��ġ�°� ������ ���µ�. ������� ������ ��ġ�°�. �ʵ忡 ������ ��ġ�°�. �׷��� ���簡 ���絥 ������. �ҹ��������� �ʵ�ȿ��?
     * �ʵ�ȿ�� �����ϸ� �Ӹ��� ����������. ������ ��ĸ� ã���� ���߿��� �������θ� ����ϸ� �Ǵϱ� �׳��� �����ѵ�.
     * ��. �� ������ ������ ������. ���ǿ������� ������ ����. private void OnDestroy() �ı��϶�µ�? �������������� 
     * 
     * 1.effect json�� �ѱ��. �׸��� manager�� ������ ó���Ѵ�. ex) target : enemy, limit : level : 5, effect : damage : 1. 
     * 
     * �帧���� �ʿ��ϰڳ� �̰�. 
     * 
     * ����
     * Ÿ�� : ��
     * ȿ�� : ������, �����.
     * �� ȿ���� �д°� ���� �ɸ��� �ʰڴ���.
     * [effect] in. [type] == "...", [name] == ",,,", [value] * [state]. damage?
     * ��ɾ� �ؼ���. �� ������ ���� �ϰ���. �ֳĸ� �װŴ� manager�� ���� �ƴ���. �ٵ� �׷��� ������ �Ŵ����� �˾ƾ� �ұ�?
     * �� �ʿ�� ������. ������ ���� ���� ������ ����ΰſ���. �׵��׷��� ���⼭�� �ǵ��. ���⼭�� �ǵ�� �� �׷���.
     * �ٵ� �Ŵ����� ���� ���� ����. Ÿ�ٿ�.
     * �Ĺ��� ���� �������� ������ ����.
     * �Ĺ '�߰�'�ϱ⿡�� �� �ָ��� ���� �����ʾ��ֱ��ѵ�.
     * �׷��� �߰����� hit�� �Ĺ �ִ°͵� ������ ����. 
     * 
     */

    private bool CheckCondition(NarrativeSetting setting, GameObject target)
    {
        switch (setting.name)
        {
            // ü�� �ʿ�(�䱸)ġ.
            case "hp":
                if(target.GetComponent<LivingEntity>().health <= setting.value) return true;
                return false;
            case "mp":
                break;

            // ��� Ƚ�� ������ ������ �ٽñ� �����غ��� © ��.
            case "use":
                return true;

            //case "turn":

              
        }


        //���� �ش��ϴ� ������ ���ٸ� �� ��ȯ
        return true;

    }

    public void CallByStageSet()
    {

    }


    //type : battle. �� ���̺� ����/���� �� ����
    public void CallByManager(GameObject target, BattleManager battleManager)
    {

        if (target.GetComponent<InterAction>() == null) return;

        //�� ���� ó��(�ӽ�)
        if (battleManager.turnSequence != 0)
        {
            foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
                ResetStack("turn", narrative);
            return;
        }

        foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
        {
            //���ο� ���̺��
            ResetStack("wave", narrative);
            
            //manager�ش� ���縸 �б�
            if (narrative.type != "battle") continue;

            bool condition_clear = true;
            foreach (NarrativeSetting con in narrative.condition)
            {
                //��� ���� ���� Ȯ��
                if (!CheckCondition(con, target)) condition_clear = false;


                //���̺� ����
                if (con.name == "turn" && con.type == "wave")
                    if ((battleManager.turnWave % con.value) != 0) { condition_clear = false; }
                //Debug.Log("checkingg");
            }
            //�̰Ŵ� ��ȿ����.
            if (condition_clear && !LimitUse(narrative))
            {
                if (narrative.effect == null || target == null) return;

                PopupNarrative(target, narrative);
                foreach (NarrativeSetting effect in narrative.effect) 
                    combatCalculator.ApplyNarrative(target, effect);
                //Debug.Log("active : " + narrative.name);
                
            }
        }
        
    }

    //type : combat. �� �տ��� ����
    public void CallByCombat(GameObject target, bool compete)
    {
        //SetOwnNarrativeRoute(target);

        //�÷��̾ �׷���鿡�� �� �ִ°�. ���⼭ ���� �ݿ��� �� �տ� �ݿ������� ���� �ǹ�.
        //���� �ϴ� - Ÿ�� ��� �ִ� - ���� �ϸ鼭 ȯ���� �Ŵ������Լ�? �޴´�. ȯ���� ����. Ÿ�ٿ��� ����ġ�� �ش�. -> accurate. ����. ���� 0. '�̹���'

    }

    //type : dice. �� �տ��� ����
    public float MeddleInCombat(GameObject target, bool compete)
    {
        if (target.GetComponent<InterAction>() == null) return 0.0f;

        foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
        {
            if (narrative.type != "dice") continue;

            bool condition_clear = true;
            //���� Ȯ��.
            foreach (NarrativeSetting con in narrative.condition)
            {
                //������ �ϳ��� �������� ������, ���� ����� �ѱ��.
                if (!CheckCondition(con, target)) condition_clear = false;

                //�������ǿ��� �̰����� ���� �̴޼�.
                if (con.name == "compete")
                    if (con.state == "fail" && compete) condition_clear = false;
                //Debug.Log("checkingg");
            }

            //10�� Ȯ���� 100.0f ��ȯ. �ƴϸ� 0.0f ��ȯ.
            if (condition_clear && !LimitUse(narrative))
            {
                //Debug.Log(narrative.battle_use + narrative.overlap_use.ToString());
                //Debug.Log(target.GetComponent<InterAction>().narratives[i].stack + " or " + narrative.stack);

                PopupNarrative(target, narrative);
                //Debug.Log("active : " + narrative.name); 
                return 100.0f;
            }

        }

        //���� ������ - �� ���� - Ÿ���� ��� - ����Ȯ���� ������. - ������. �� 100�� ������.

        //100�̸� ������. 999�� �� �ٽ� ����. �׷� ��������.
        return 0.0f;
    }

    //���� �˾� �ӽ� ���� �� �ı�
    private void PopupNarrative(GameObject target, NarrativeSlot narrative)
    {
        Debug.Log("ldajfljrlk");
        //������ ����
        GameObject prefab = Resources.Load<GameObject>("ribbon");
        Vector3 pos = new Vector3(1, 0.5f, 0); // z��ǥ�� �־� ����..

        //�ν���Ʈ ����
        GameObject tmp;
        tmp = Instantiate(prefab, target.transform);
        //player ���δ� �ϴ� �̱��� tmp = Instantiate(prefab, new Vector3(x, x, 0);
        tmp.GetComponent<RectTransform>().anchoredPosition = pos;

        TextMeshProUGUI[] text = tmp.GetComponentsInChildren<TextMeshProUGUI>();
        text[0].text = narrative.name;
        text[1].text = narrative.describe;

        //�ı�/
        Destroy(tmp, 5f);
    }
    private void SetOwnNarrativeRoute(GameObject target)
    {
        //narrative_route = Application.persistentDataPath + "/Info/NarrativeList.json";
        string route = Application.persistentDataPath + "/Info/NarrativeList.json";

        //�� �з��� ��� �ϸ� ������ �𸣰ڳ�.. ������.
        if(target.name == "Player") route = Application.persistentDataPath + "/Info/NarrativeList.json";
        else if(target.name == "Enemy111") route = Resources.Load<TextAsset>("/Text/Batte/Monster" + target.name).ToString();
        //else if(target.GetComponent<EnemyHealth>() != null) target.GetComponent<EnemyHealth>().nickname = route;
        narrative_list = convertJson.MakeJson(route);
        jroot = JObject.Parse(narrative_list);
    }

    private bool LimitUse(NarrativeSlot narrative)
    {
        //�� �� �ߺ� & �� ���� & �� ���̺�. ��� ����
        if (narrative.overlap_use) return true;
        if(narrative.battle_use >= narrative.max_battle_use) return true;
        if(narrative.turn_use >= narrative.max_turn_use) return true;

        narrative.battle_use++;
        narrative.turn_use++;
        narrative.overlap_use = true;

        if (narrative.can_stack) narrative.stack++;

        return false;
    }
    
    private void ResetStack(string type, NarrativeSlot narrative)
    {
        //type : battle, wave, turn
        if(type == "battle") //stageset
        {
            narrative.battle_use = 0;
            narrative.turn_use = 0;
            narrative.overlap_use = false;
            narrative.stack = 0; //������ ������ �ָ��� �����̾�.. ������ ������ ������ �� ���̳� �� ���̺�� �� ������ .... �� �׷���.
        }
        else if(type == "wave") //manager
        {
            narrative.turn_use = 0;
            narrative.overlap_use = false;
        }
        else if(type == "turn") //combat //1������ 2������ 3������ ������ ���� �����Ѱ�? manager���� �ҷ��;���. ..
        {
            narrative.overlap_use = false;
        }

    }

}
