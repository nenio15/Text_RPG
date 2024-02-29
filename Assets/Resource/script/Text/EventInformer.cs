using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EventInformer
{
    // 여기서 이벤트들의 여하여부를 결정
    // 확률도 얻고 그럼. ㅇㅇ
    // nearby를 textchanger에게서 얻어낸다.
    // 그거를 반영은. 매니저한테?
    // 다른 스크립트한테 알려주는 건 좋아. 근데 얻어내는건 이 스크립트가 직접 해내야지.

    // Start is called before the first frame update
    [SerializeField] private Textchanger textchanger;

    /*
    // 필요없는 함수. jmp에서 얻는 걸로 전부 해결할거야.
    public void getNear() 
    {
        foreach(JToken jnear in textchanger.jbase["nearby"]) // nearby 전부 토해내!!!!
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
         * 1. 현재가 지역 묘사인지 여하
         * 2. 캐릭터가 지닌 시나리오, 퀘스트 여하
         * 2-1.시행할지 진짜 여하
         * 
         * 다른 함수?
         * 3. 각 이벤트들을 math.random(float) 수치화
         * 4. 그리고 돌려주기.
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
     * temporary.json에 기존의 main.json을 카피해놓는다.
     * 그리고 이벤트 삽입. (
     * 끝나면 main.json으로 다시 반환.
    */
        origin_keyroute = Application.dataPath + @"\Resource\Text\main.json";
        tmp_keyroute = Application.dataPath + @"\Resource\Text\temporary.json";


        File.WriteAllText(tmp_keyroute, MakeJson(origin_keyroute));

        //Debug.Log(move + " " + jmain + " : " + jsub);
        //string eventroute = Application.dataPath + @"\Resource\Text\Scenario\" + jmain;


    }

    private string MakeJson(string jpath)    //parsing안 하고 그냥 넣는거는 안되나? 굳이..?
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