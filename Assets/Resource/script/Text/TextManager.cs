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

    private float typing_speed = 0.2f;
    [SerializeField] private string[] contents;

    private void Start()
    {
        real_main = Application.dataPath + @"\Resource\Text\main.txt";
        System.IO.File.WriteAllText(real_main, ""); //reset
        m_TypingText.text = "";
        m_ped = new PointerEventData(null);
        typing_speed = m_Speed;

        //start scene에서 시나리오 받아오기
        cur_scenario = PlayerPrefs.GetString("Cur_scenario");

        //읽기 시작
        textchanger.ReadScenarioParts(idx++, cur_scenario);//, cur_subscenario);    //json
        contents = System.IO.File.ReadAllLines(real_main);
    }

    // text click
    public void OnPointerClick(PointerEventData eventData)
    {
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
        stop_read = true;
        //switch로 바꿀 필요가 있다면 바꿀것.(#key말고 사용도가 있다면.)
        //# 있는 문장
        StopCoroutine("Showing");

        // 현재 읽는 페이지가 끝났음
        switch (contents[current])
        {
            case "#jmp":
            case "#rpl":
                textchanger.ReadScenarioParts(textchanger.pre_move, textchanger.pre_main);
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
                contents = System.IO.File.ReadAllLines(real_main);
                //stop_read = true;
                return true;
            case "#btl":
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
            string cur_text = "";
            while (!contents[current].Contains('#'))
                cur_text += STyping(contents[current++] + '\n');
            cur_text += '\n';

            //타이핑 효과
            typing_speed = m_Speed;
            StartCoroutine(Showing(m_TypingText, cur_text));

            //중복 클릭시 순간 출력으로 전환
            reading = true;
        }
        else            // fast reading
            typing_speed = 0.0f;

    }

    //외부에서 읽기 재시작
    public void Reread()
    {
        reading = false;
        stop_read = false;
        contents = System.IO.File.ReadAllLines(real_main);
    }

    
    //타이핑 효과
    IEnumerator Showing(Text typingText, string message)
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
            yield return new WaitForSeconds(typing_speed);
        }
        //첫 출력
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