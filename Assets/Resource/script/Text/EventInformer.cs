using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EventInformer
{
    // ���⼭ �̺�Ʈ���� ���Ͽ��θ� ����
    // Ȯ���� ��� �׷�. ����
    // nearby�� textchanger���Լ� ����.
    // �װŸ� �ݿ���. �Ŵ�������?
    // �ٸ� ��ũ��Ʈ���� �˷��ִ� �� ����. �ٵ� ���°� �� ��ũ��Ʈ�� ���� �س�����.

    // Start is called before the first frame update
    [SerializeField] private Textchanger textchanger;

    /*
    // �ʿ���� �Լ�. jmp���� ��� �ɷ� ���� �ذ��Ұž�.
    public void getNear() 
    {
        foreach(JToken jnear in textchanger.jbase["nearby"]) // nearby ���� ���س�!!!!
        {
            Debug.Log(jnear.ToString());

        }

    }
    */

    
    private string origin_keyroute;
    private string tmp_keyroute;

    public bool CheckInsertEvent(string main)
    {
        /*
         * 1. ���簡 ���� �������� ����
         * 2. ĳ���Ͱ� ���� �ó�����, ����Ʈ ����
         * 2-1.�������� ��¥ ����
         * 
         * �ٸ� �Լ�?
         * 3. �� �̺�Ʈ���� math.random(float) ��ġȭ
         * 4. �׸��� �����ֱ�.
         *  
        */
        if (main == "region")
            return true;
        else
            return false;

    }

    public void ScenarioSaveTmp()
    {
        /*
     * temporary.json�� ������ main.json�� ī���س��´�.
     * �׸��� �̺�Ʈ ����. (
     * ������ main.json���� �ٽ� ��ȯ.
    */
        origin_keyroute = Application.dataPath + @"\Resource\Text\main.json";
        tmp_keyroute = Application.dataPath + @"\Resource\Text\temporary.json";


        File.WriteAllText(tmp_keyroute, MakeJson(origin_keyroute));

        //Debug.Log(move + " " + jmain + " : " + jsub);
        //string eventroute = Application.dataPath + @"\Resource\Text\Scenario\" + jmain;


    }

    private string MakeJson(string jpath)    //parsing�� �ϰ� �׳� �ִ°Ŵ� �ȵǳ�? ����..?
    {
        string str = null;
        using (StreamReader sr = File.OpenText(jpath))
        using (JsonTextReader reader = new JsonTextReader(sr))
        {
            str = sr.ReadToEnd();
            sr.Close();
        }
        return str;
    }
}