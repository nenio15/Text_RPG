using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TextManager : MonoBehaviour, IPointerClickHandler
{
    [Header("TEXTER INFO")]
    //public Text m_TypingText;
    public TextMeshProUGUI m_TypingText;
    public int m_Fontsize;
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
    public string keywords;   //초기화
    [SerializeField] private int robj_i = 0;
    public GameObject[] robj;   //color 클릭 오브제. 삭제할듯?

    private string real_main;

    [Header("REFERENCE")]
    [SerializeField] private GameObject Selection;
    [SerializeField] private TextChanger textchanger;
    private EventInformer eventInformer = new EventInformer();
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform content;

    private float typing_speed = 0.2f;
    
    [SerializeField] private string[] contents;


    public TextMeshProUGUI debugging;

    private void Start()
    {
        real_main = UnityEngine.Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/main.txt";

        //초기화
        m_TypingText.fontSize = m_Fontsize;
        m_TypingText.text = "";
        m_ped = new PointerEventData(null);
        typing_speed = m_Speed;

        //start scene에서 선택 시나리오 받아오기 (디버깅용) -> 이거 나중에 바꿀것.
        cur_scenario = PlayerPrefs.GetString("Cur_scenario");

        //읽기 시작
        textchanger.ReadScenarioParts(idx++, cur_scenario);//, cur_subscenario);    //json
        contents = System.IO.File.ReadAllLines(real_main);
        
        ReadStory(false);
    }

    //모바일 터치
    void Update()
    {
        /*
         * pointerclick으로도 됨. 일단 없애기.
        if (Input.touchCount > 0)
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
                ReadPage();
        */
        debugging.text = "Current Speed: " + typing_speed;
    }

    // text click
    public void OnPointerClick(PointerEventData eventData) { ReadPage(); }

    public void ReadPage()
    {
        Debug.Log("activated");
        //m_TypingText.text = 
        // 읽는 것을 중단.
        if (stop_read) return;

        //null 참조 방지 임시 코드
        if (contents.Length <= current) current = contents.Length - 1;

        // 텍스트가 출력되는 상황이 아니라면.
        if (!reading)
        {
            //현재장 다 읽음                                      (#포함)
            //if (contents.Length == current) EndStoryPart(++page, "", "");

            //#(중단점) 조우
            if (contents[current][0] == '#')
                if (MeetSign()) return;

        }

        //페이지 읽기
        ReadStory(false);
    }

    //중단점 처리
    private bool MeetSign()
    {
        //Debug.Log("fladjfla"); //중단점 진입이 빠른듯하다
        stop_read = true;
        //switch로 바꿀 필요가 있다면 바꿀것.(#key말고 사용도가 있다면.)
        //# 있는 문장
        StopCoroutine("Showing");

        // 현재 읽는 페이지가 끝났음
        switch (contents[current])
        {
            case "#move":
            case "#over":
                textchanger.ReadScenarioParts(textchanger.next_move, textchanger.next_main);
                ClearText();
                return true;
            case "#key":
                //선택지를 보여줘 //Debug.Log("READING[key] : stop and call selection");
                Selection.GetComponent<SelectionManager>().ShowSelection("key", keyi++, 0);
                current++;
                /*
                if (contents[current].Contains("sc"))
                {
                    Debug.Log("READING[sc_key] : ...");
                    sc_keyi++;
                }
                */
                //내용물 변경 유무
                contents = System.IO.File.ReadAllLines(Application.persistentDataPath + "/" + PlayerPrefs.GetString("Char_route") + "/main.txt");
                //contents = System.IO.File.ReadAllLines(real_main);
                //stop_read = true;
                return true;
            case "#btl": //battle로 변경.
                //선택지한테 말하거나, 디시즌한테 말하거나.
                current++; //복귀 시 다음 줄 읽기
                GameObject.Find("Battle").GetComponent<BattleManager>().BattleEntry();
                return true;
            default:
                current++;
                stop_read = false;
                break;

        }


        
        //키워드 블럭들 초기화 -> 이것도 이사 시킬것.
        if (robj_i > 0)
            for (; robj_i > 0; robj_i--)
                robj[robj_i - 1].GetComponent<Keyword>().DelKeyword();
        //stop_read = false;

        return false;

    }

    //스크립트 + 뷰 리셋
    public void ClearText()
    {
        keyi = 0;
        sc_keyi = 0;
        current = 0;
        reading = false;
        stop_read = false;
        m_TypingText.text = "";
        content.offsetMin = new Vector2(0, -800);

        //새로 채우기
        contents = System.IO.File.ReadAllLines(real_main);
    }

    //main 읽어내기
    public void ReadStory(bool changed)
    {
        //contents의 배열이 확정되어버리나? 그럼 안되는데..흠.
        if (changed)
        {
            contents = System.IO.File.ReadAllLines(real_main);
            for (; robj_i > 0; robj_i--)
                robj[robj_i - 1].GetComponent<Keyword>().DelKeyword();
        }

        if (!reading)   //normal reading
        {
            //텍스트 오브젝트 이동 및 늘리기 (spacing은 전 text의 길이를 참조함)
            ExtendContent(spacing);

            //배열 길이 오버 예외처리
            if (current >= contents.Length) { Debug.LogError("[TEXT] Don't exist extra content"); return; }

            //문단 기입
            string cur_text = "  ";
            spacing = 1;
            while (!contents[current].Contains('#'))
            {
                cur_text += STyping(contents[current++] + '\n');
                spacing++;
            }
            cur_text += '\n';

            //타이핑 효과
            typing_speed = m_Speed;
            StartCoroutine(Showing(m_TypingText, cur_text));

            //중복 클릭시 순간 출력으로 전환
            reading = true;
        }
        else
        {            // fast reading
            typing_speed = 0.0f;
        }

    }

    //외부에서 읽기 재시작
    public void Reread()
    {
        reading = false;
        stop_read = false;
        contents = System.IO.File.ReadAllLines(real_main);
    }

    private void ExtendContent(int space)
    {
        //확장은 넣되, 스크롤 이동은 일단 제외.

        if (m_TypingText.text == "") return;

        //text font * 1.47.... -> fontsize를 받아둘게 필요. system에 넣고 파라미터를 get?
        // 34 * 1.47 -> 50
        float height = Mathf.Abs(content.rect.height); // 절대값
        float upheight = height + space * m_Fontsize;
        //Debug.Log("height" + height + " space " + space + "font " + m_Fontsize);
        Vector2 previousPos = scroll.content.anchoredPosition;

        //스크롤 이동.
        //Debug.Log(height + " and " + upheight);
        content.sizeDelta = new Vector2(0, upheight);
        //scroll.content.anchoredPosition = previousPos + new Vector2(0f, space * m_Fontsize);
    }

    
    //타이핑 효과
    IEnumerator Showing(TextMeshProUGUI typingText, string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            //키워드 있을 경우 -> 삭제 예정
            if (message[i] == '|')
            {
                string after_message = message.Substring(i + 1);
                string[] divided_message = after_message.Split('|');
                typingText.text += divided_message[0];
                i += divided_message[0].Length + 1;
                //newKeyword(i, message, divided_message[0]);     //키워드 선택 오브제 생성
                //Debug.Log("키워드의 길이 : " + divided_message[0].Length);
            }
            else
            {
                typingText.text += message[i];
            }

            if(typing_speed <= 0)
            {
                ShowRest(typingText, message, i);
                Debug.Log("skip show all : " + message);
                yield break;
            }
            yield return new WaitForSeconds(typing_speed);
        }
        //첫 출력
        reading = false;
    }

    void ShowRest(TextMeshProUGUI typingText, string message, int i)
    {
        for(int j = i; j < message.Length; j++) typingText.text += message[j];
        reading = false;
    }

    string STyping(string message)        //현재 줄 출력(한 글자 씩)
    {
        for (int i = 0; i < message.Length; i++)
        {
            //키워드 있을 경우
            if (message[i] == '|')
            {
                string after_message = message.Substring(i + 1);
                string[] divided_message = after_message.Split('|');
                i += divided_message[0].Length + 1;
                NewKeyword(i, message, divided_message[0]);     //키워드 선택 오브제 생성
                //sc_keyi++;  // 키워드 수만큼 늘어나는데....?
            }
        }
        return message;
    }

    //sc_key를 위한 view 세팅임. 아마 삭제할듯? 게임성 + 가독성 문제
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
        int position = -407 + (view_position) * 40 + center * 40;        //공백 (띄어쓰기는 크기가 달라), 한 글자 크기 : 40, 띄어쓰기 : 20

        keywords = keyword_message;
        robj[robj_i].GetComponent<Keyword>().GetKeyword(keywords, sc_keyi);
        RectTransform rect = robj[robj_i].GetComponent<RectTransform>();
        //Debug.Log("KEYWORD[obj] : " + robj[robj_i].GetComponent<Keyword>().keyword);
        rect.anchoredPosition = new Vector2(position, -275);
        robj_i = (robj_i + 1) % 5;

    }

}