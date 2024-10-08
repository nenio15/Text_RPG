using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextManager : MonoBehaviour, IPointerClickHandler
{
    [Header("TEXTER INFO")]
    public Text m_TypingText;
    public float m_Speed = 0.2f;
    private int idx = 0;
    private string m_Message;
    
    [SerializeField] private int current = 0;
    
    public bool stop_read = false;

    [SerializeField] private string cur_scenario = "main_scenario";
    //[SerializeField] private string cur_subscenario = "Main_1";

    [SerializeField] private bool reading = false;
    //[SerializeField] private int page = 0;
    //[SerializeField] public bool eventcall = false;

    private int keyi = 0;
    private int sc_keyi = 0;
    private int spacing = 1;

    [Header("RAYCASYER")]
    public Canvas m_canvas;
    public GraphicRaycaster m_gr;
    public PointerEventData m_ped;

    [Header("C_KEYWORD")]
    [SerializeField] private GameObject clickobj;
    public string keywords;   //段奄鉢
    [SerializeField] private int robj_i = 0;
    public GameObject[] robj;   //color 適遣 神崎薦. 肢薦拝牛?

    private string real_main;

    [Header("REFERENCE")]
    [SerializeField] private GameObject Selection;
    [SerializeField] private TextChanger textchanger;
    private EventInformer eventInformer = new EventInformer();
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform content;

    private float typing_speed = 0.2f;
    //
    [SerializeField] private string[] contents;

    private void Start()
    {
        real_main = Application.dataPath + @"\Resource\Text\main.txt";
        System.IO.File.WriteAllText(real_main, ""); //reset
        m_TypingText.text = "";
        m_ped = new PointerEventData(null);
        typing_speed = m_Speed;

        //start scene拭辞 獣蟹軒神 閤焼神奄
        cur_scenario = PlayerPrefs.GetString("Cur_scenario");

        //石奄 獣拙
        textchanger.ReadScenarioParts(idx++, cur_scenario);//, cur_subscenario);    //json
        contents = System.IO.File.ReadAllLines(real_main);

        m_TypingText.text = "焼巷依亀 蒸柔艦陥たたたたたたたたたたたたたたた";
    }

    // text click
    public void OnPointerClick(PointerEventData eventData)
    {
        // 石澗 依聖 掻舘.
        if (stop_read) return;

        //null 凧繕 号走 績獣 坪球
        if (contents.Length <= current) current = contents.Length - 1;

        // 努什闘亜 窒径鞠澗 雌伐戚 焼艦虞檎.
        if (!reading)
        {
            //薄仙舌 陥 石製                                      (#匂敗)
            //if (contents.Length == current) EndStoryPart(++page, "", "");

            

            //#(掻舘繊) 繕酔
            if (contents[current][0] == '#')
                if (MeetSign()) return; 

        }

        //凪戚走 石奄
        ReadStory(false);
    }

    //掻舘繊 坦軒
    private bool MeetSign()
    {
        //Debug.Log("[MEETSIGN] : " + contents[current]);
        stop_read = true;
        //switch稽 郊蝦 琶推亜 赤陥檎 郊蝦依.(#key源壱 紫遂亀亜 赤陥檎.)
        //# 赤澗 庚舌
        StopCoroutine("Showing");

        // 薄仙 石澗 凪戚走亜 魁概製
        switch (contents[current])
        {
            case "#move":
            case "#over":
                textchanger.ReadScenarioParts(textchanger.pre_move, textchanger.pre_main);
                ClearText();
                return true;
            case "#key":
                //識澱走研 左食操 //Debug.Log("READING[key] : stop and call selection");
                Selection.GetComponent<SelectionManager>().ShowSelection("key", keyi++, 0);
                current++;
                /*
                if (contents[current].Contains("sc"))
                {
                    Debug.Log("READING[sc_key] : ...");
                    sc_keyi++;
                }
                */
                //鎧遂弘 痕井 政巷
                contents = System.IO.File.ReadAllLines(real_main);
                //stop_read = true;
                return true;
            case "#btl": //battle稽 痕井.
                //識澱走廃砺 源馬暗蟹, 巨獣草廃砺 源馬暗蟹.
                current++; //差瑛 獣 陥製 匝 石奄
                GameObject.Find("Battle").GetComponent<BattleManager>().BattleEntry();
                return true;
            default:
                current++;
                stop_read = false;
                break;

        }


        
        //徹趨球 鷺薫級 段奄鉢 -> 戚依亀 戚紫 獣迭依.
        if (robj_i > 0)
            for (; robj_i > 0; robj_i--)
                robj[robj_i - 1].GetComponent<Keyword>().DelKeyword();
        //stop_read = false;

        return false;

    }

    //什滴験闘 + 坂 軒実
    public void ClearText()
    {
        keyi = 0;
        sc_keyi = 0;
        current = 0;
        reading = false;
        stop_read = false;
        m_TypingText.text = "";
        content.offsetMin = new Vector2(0, -800);

        //歯稽 辰酔奄
        contents = System.IO.File.ReadAllLines(real_main);
    }

    //main 石嬢鎧奄
    public void ReadStory(bool changed)
    {
        //contents税 壕伸戚 溌舛鞠嬢獄軒蟹? 益軍 照鞠澗汽..緋.
        if (changed)
        {
            contents = System.IO.File.ReadAllLines(real_main);
            for (; robj_i > 0; robj_i--)
                robj[robj_i - 1].GetComponent<Keyword>().DelKeyword();
        }
        
        if (!reading)   //normal reading
        {
            //努什闘 神崎詮闘 戚疑 貢 潅軒奄 (spacing精 穿 text税 掩戚研 凧繕敗)
            ExtendContent(spacing);

            //壕伸 掩戚 神獄 森須坦軒
            if (current >= contents.Length) { Debug.Log("[ERROR][TEXT] Don't exist extra content"); return; }

            //庚舘 奄脊
            string cur_text = "  ";
            spacing = 1;
            while (!contents[current].Contains('#'))
            {
                cur_text += STyping(contents[current++] + '\n');
                spacing++;
            }
            cur_text += '\n';

            //展戚芭 反引
            typing_speed = m_Speed;
            StartCoroutine(Showing(m_TypingText, cur_text));

            //掻差 適遣獣 授娃 窒径生稽 穿発
            reading = true;
        }
        else            // fast reading
            typing_speed = 0.0f;

    }

    //須採拭辞 石奄 仙獣拙
    public void Reread()
    {
        reading = false;
        stop_read = false;
        contents = System.IO.File.ReadAllLines(real_main);
    }

    private void ExtendContent(int space)
    {
        if (m_TypingText.text == "") return;


        //戚暗 鯵割戚 設公喫. space亜 戚穿 匝聖 閤焼辞 益幻鏑 鎧形醤 馬澗汽, 走榎 戚暗. 陥製 匝税 姐呪幻鏑 鎧顕.
        //蕉段拭 設公吉 獣什奴戚虞 text亀 焼原 痕井吃牛... 戚暗澗 左嫌背醤拝牛. 戚暗 戚訓縦亀 照 限焼
        //訊劃. 什滴継聖 廃 雌殿稽 適遣馬檎, 歯稽 獣拙馬澗 走繊戚 焼艦摂? 益軒壱 諮鉢亀 隔生檎 希 何析襖醤. 戚暗澗 析舘 二奄

        //text font * 1.47.... -> fontsize研 閤焼却惟 琶推. system拭 隔壱 督虞耕斗研 get?
        // 34 * 1.47 -> 50
        int fontsize = 45;

        float height = Mathf.Abs(content.rect.height); // 箭企葵
        float upheight = height + space * fontsize;
        Vector2 previousPos = scroll.content.anchoredPosition;


        //什滴継 戚疑.
        //Debug.Log(height + " and " + upheight);
        content.sizeDelta = new Vector2(0, upheight);
        //岨 希 採球郡惟 崇送心生檎 馬澗汽..
        //什滴継精 析舘 蒸訟猿..?
        scroll.content.anchoredPosition = previousPos + new Vector2(0f, space * fontsize);
    }

    
    //展戚芭 反引
    IEnumerator Showing(Text typingText, string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            //徹趨球 赤聖 井酔 -> 肢薦 森舛
            if (message[i] == '|')
            {
                string after_message = message.Substring(i + 1);
                string[] divided_message = after_message.Split('|');
                typingText.text += divided_message[0];
                i += divided_message[0].Length + 1;
                //newKeyword(i, message, divided_message[0]);     //徹趨球 識澱 神崎薦 持失
                //Debug.Log("徹趨球税 掩戚 : " + divided_message[0].Length);
            }
            else
            {
                typingText.text += message[i];
            }
            yield return new WaitForSeconds(typing_speed);
        }
        //湛 窒径
        reading = false;
    }

    string STyping(string message)        //薄仙 匝 窒径(廃 越切 梢)
    {
        for (int i = 0; i < message.Length; i++)
        {
            //徹趨球 赤聖 井酔
            if (message[i] == '|')
            {
                string after_message = message.Substring(i + 1);
                string[] divided_message = after_message.Split('|');
                i += divided_message[0].Length + 1;
                NewKeyword(i, message, divided_message[0]);     //徹趨球 識澱 神崎薦 持失
                //sc_keyi++;  // 徹趨球 呪幻鏑 潅嬢蟹澗汽....?
            }
        }
        return message;
    }

    //sc_key研 是廃 view 室特績. 焼原 肢薦拝牛? 惟績失 + 亜偽失 庚薦
    void NewKeyword(int real_position, string message, string keyword_message)  //get view pos
    {
        string before_message = message.Substring(0, real_position);
        string[] divided_message = before_message.Split('|');
        int view_position = real_position;
        int center = 0;

        for (int i = 1; i < divided_message.Length;)    //real pos -> view pos
        {
            string[] divided_keyword = divided_message[i].Split('>');
            string[] divided_keyword2 = divided_keyword[1].Split('<');
            center = divided_keyword2.Length / 2;
            view_position -= (divided_keyword[0].Length + divided_keyword2[0].Length + 11);
            i = i + 2;
        }
        int position = -407 + (view_position) * 40 + center * 40;        //因拷 (句嬢床奄澗 滴奄亜 含虞), 廃 越切 滴奄 : 40, 句嬢床奄 : 20

        keywords = keyword_message;
        robj[robj_i].GetComponent<Keyword>().GetKeyword(keywords, sc_keyi);
        RectTransform rect = robj[robj_i].GetComponent<RectTransform>();
        //Debug.Log("KEYWORD[obj] : " + robj[robj_i].GetComponent<Keyword>().keyword);
        rect.anchoredPosition = new Vector2(position, -275);
        robj_i = (robj_i + 1) % 5;

    }

}