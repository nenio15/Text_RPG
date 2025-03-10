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
        narrative_route = Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/Info/NarrativeList.json";
        narrative_list = convertJson.MakeJson(narrative_route);
        jroot = JObject.Parse(narrative_list);
    }

    //辞紫税 曽嫌. 1.鳶獣崎 2.穿燈辛芝 3.識澱走辛芝 4.衝銅崎


    //2.穿燈 辛芝 敗呪級
    /*
     * 凧 弦戚亀 赤革. 展為拭惟 慎狽聖 晦帖澗闇 庚薦亜 蒸澗汽. 雌企廃砺 慎狽聖 晦帖澗暗. 琶球拭 慎狽聖 晦帖澗暗. 益訓暗 孔紫亜 毘窮汽 源戚走. 災弘杖製去税 琶球反引?
     * 琶球反引 持唖馬檎 袴軒亜 焼托背走革. 旋雁備 丞縦幻 達生檎 蟹掻拭澗 呪俳生稽幻 壱肯馬檎 鞠艦苑 益蟹原 慨延廃汽.
     * 緋. 戚 走偶戚 情薦鷹 魁劾猿. 背杷殖漁生稽 魁劾猿 引尻. private void OnDestroy() 督雨馬虞澗汽? せせせせせせせ 
     * 
     * 1.effect json聖 角延陥. 益軒壱 manager亜 旋雁備 坦軒廃陥. ex) target : enemy, limit : level : 5, effect : damage : 1. 
     * 
     * 斐硯亀亜 琶推馬畏革 戚暗. 
     * 
     * 昔窒
     * 展為 : 旋
     * 反引 : 汽耕走, 巨獄覗.
     * 煽 反引研 石澗惟 神掘 杏軒走 省畏汗劃.
     * [effect] in. [type] == "...", [name] == ",,,", [value] * [state]. damage?
     * 誤敬嬢 背汐奄. 亜 赤生檎 畷備 馬畏走. 訊劃檎 益暗澗 manager税 析精 焼艦摂. 悦汽 益係惟 魚走檎 古艦煽亜 硝焼醤 拝猿?
     * 硝 琶推澗 蒸走幻. 娃七税 映廃 庚薦 凶庚拭 壱肯昔暗心走. 益亀益群惟 食奄辞亀 闇級壱. 煽奄辞亀 闇級檎 岨 益係摂.
     * 悦汽 古艦煽澗 据掘 娃七 照背. 展為拭.
     * 陳克戚 据掘 送羨旋昔 娃七戚 赤走.
     * 陳克拭 '蓄亜'馬奄拭澗 岨 蕉古廃 姶戚 蒸走省焼赤延廃汽.
     * 益君檎 蓄亜旋昔 hit研 陳克拭 隔澗依亀 蟹孜遭 省走. 
     * 
     */

    private bool CheckCondition(NarrativeSetting setting, GameObject target)
    {
        switch (setting.name)
        {
            // 端径 琶推(推姥)帖.
            case "hp":
                if(target.GetComponent<LivingEntity>().health <= setting.value) return true;
                return false;
            case "mp":
                break;

            // 紫遂 判呪 薦廃精 痕呪研 陥獣榎 持唖背左壱 息 依.
            case "use":
                return true;

            //case "turn":

              
        }


        //是拭 背雁馬澗 繕闇戚 蒸陥檎 凧 鋼発
        return true;

    }

    public void CallByStageSet()
    {

    }


    //type : battle. 廃 裾戚崎 曽戟/獣拙 獣 弘製
    public void CallByManager(GameObject target, BattleManager battleManager)
    {

        if (target.GetComponent<InterAction>() == null) return;

        //渡 舘是 坦軒(績獣)
        if (battleManager.turnSequence != 0)
        {
            foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
                ResetStack("turn", narrative);
            return;
        }

        foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
        {
            //歯稽錘 裾戚崎獣
            ResetStack("wave", narrative);
            
            //manager背雁 辞紫幻 石奄
            if (narrative.type != "battle") continue;

            bool condition_clear = true;
            foreach (NarrativeSetting con in narrative.condition)
            {
                //乞窮 繕闇 採杯 溌昔
                if (!CheckCondition(con, target)) condition_clear = false;


                //裾戚崎 繕闇
                if (con.name == "turn" && con.type == "wave")
                    if ((battleManager.turnWave % con.value) != 0) { condition_clear = false; }
                //Debug.Log("checkingg");
            }
            //戚暗澗 政反旋績.
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

    //type : combat. 廃 杯拭辞 弘製
    public void CallByCombat(GameObject target, bool compete)
    {
        //SetOwnNarrativeRoute(target);

        //巴傾戚嬢蟹 益訓叶級拭惟 葵 爽澗依. 食奄辞 亜澗 鋼慎戚 益 杯拭 鋼慎吃走亜 薦析 税庚.
        //玄精 馬潅 - 展為 級壱 赤陥 - 杯聖 馬檎辞 発井聖 古艦煽拭惟辞? 閤澗陥. 発井戚 玄陥. 展為拭惟 左舛帖研 層陥. -> accurate. 獄覗. 渡精 0. '戚腰幻'

    }

    //type : dice. 廃 杯拭辞 弘製
    public float MeddleInCombat(GameObject target, bool compete)
    {
        if (target.GetComponent<InterAction>() == null) return 0.0f;

        foreach (NarrativeSlot narrative in target.GetComponent<InterAction>().narratives)
        {
            if (narrative.type != "dice") continue;

            bool condition_clear = true;
            //繕闇 溌昔.
            foreach (NarrativeSetting con in narrative.condition)
            {
                //繕闇戚 馬蟹虞亀 採杯馬走 省聖獣, 陥製 辞紫稽 角延陥.
                if (!CheckCondition(con, target)) condition_clear = false;

                //叔鳶繕闇拭辞 戚医生檎 繕闇 耕含失.
                if (con.name == "compete")
                    if (con.state == "fail" && compete) condition_clear = false;
                //Debug.Log("checkingg");
            }

            //10遁 溌懸稽 100.0f 鋼発. 焼艦檎 0.0f 鋼発.
            if (condition_clear && !LimitUse(narrative))
            {
                //Debug.Log(narrative.battle_use + narrative.overlap_use.ToString());
                //Debug.Log(target.GetComponent<InterAction>().narratives[i].stack + " or " + narrative.stack);

                PopupNarrative(target, narrative);
                //Debug.Log("active : " + narrative.name); 
                return 100.0f;
            }

        }

        //重税 左詞破 - 杯 叔鳶 - 展為戚 窮陥 - 析舛溌懸稽 閏鍵陥. - 失因獣. 葵 100聖 宜鍵陥.

        //100戚檎 巷繕闇. 999檎 更 陥獣 軒継. 益訓 汗界生稽.
        return 0.0f;
    }

    //辞紫 橡穣 績獣 持失 板 督雨
    private void PopupNarrative(GameObject target, NarrativeSlot narrative)
    {
        //Debug.Log("ldajfljrlk");
        //覗軒噸 走舛
        GameObject prefab = Resources.Load<GameObject>("ribbon");
        Vector3 pos = new Vector3(1, 0.5f, 0); // z疎妊研 隔嬢 源嬢..

        //昔什渡闘 持失
        GameObject tmp;
        tmp = Instantiate(prefab, target.transform);
        //player 魚稽澗 析舘 耕姥薄 tmp = Instantiate(prefab, new Vector3(x, x, 0);
        tmp.GetComponent<RectTransform>().anchoredPosition = pos;

        TextMeshProUGUI[] text = tmp.GetComponentsInChildren<TextMeshProUGUI>();
        text[0].text = narrative.name;
        text[1].text = narrative.describe;

        //督雨/
        Destroy(tmp, 5f);
    }
    private void SetOwnNarrativeRoute(GameObject target)
    {
        //narrative_route = Application.persistentDataPath + "/Info/NarrativeList.json";
        string route = Application.persistentDataPath + "/Info/NarrativeList.json";

        //唖 歳嫌研 嬢胸惟 馬檎 疏聖走 乞牽畏革.. 緋岨巷.
        if(target.name == "Player") route = Application.persistentDataPath + "/Info/NarrativeList.json";
        else if(target.name == "Enemy111") route = Resources.Load<TextAsset>("/Text/Batte/Monster" + target.name).ToString();
        //else if(target.GetComponent<EnemyHealth>() != null) target.GetComponent<EnemyHealth>().nickname = route;
        narrative_list = convertJson.MakeJson(route);
        jroot = JObject.Parse(narrative_list);
    }

    private bool LimitUse(NarrativeSlot narrative)
    {
        //廃 渡 掻差 & 廃 穿燈 & 廃 裾戚崎. 紫遂 薦廃
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
            narrative.stack = 0; //什澱精 食穿備 蕉古廃 鯵割戚醤.. 什澱戚 斗走澗 奄層戚 廃 杯戚劃 廃 裾戚崎劃 廃 穿燈劃 .... 更 益訓暗.
        }
        else if(type == "wave") //manager
        {
            narrative.turn_use = 0;
            narrative.overlap_use = false;
        }
        else if(type == "turn") //combat //1渡昔走 2渡昔走 3渡昔走 姥歳戚 刊亜 亜管廃亜? manager拭辞 災君人醤敗. ..
        {
            narrative.overlap_use = false;
        }

    }

}
