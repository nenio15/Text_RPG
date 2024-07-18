using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// 이게 옛날 main.json 호출 클래스
public class texttest : MonoBehaviour, IPointerClickHandler
{
    public Text m_TypingText;
    public float m_Speed = 0.2f;
    private string m_Message;

    [SerializeField]
    private GameObject clickobj;

    [Header("RAYCASYER")]
    public Canvas m_canvas;
    public GraphicRaycaster m_gr;
    public PointerEventData m_ped;

    [Header("C_KEYWORD")]
    public string keywords;   //초기화
    [SerializeField] private int robj_i = 0;
    public GameObject[] robj;

    private string avoid;
    TextChanger once = new TextChanger();
    //Keyword key = new Keyword();

    int current = 0;
    float typing_speed = 0.2f;
    bool reading = false;
    string[] contents;
    

    void Start()
    {
        avoid = Application.dataPath + @"\Resource\Text\main.txt";
        System.IO.File.WriteAllText(avoid, ""); //reset
        m_TypingText.text = "";
        m_ped = new PointerEventData(null);
        typing_speed = m_Speed;

        //once.Organize();    //json
        contents = System.IO.File.ReadAllLines(avoid);

        /*
        for(int i = 0; i < 5; i++)
            Instantiate(clickobj, GameObject.FindWithTag("InScroll").transform);   //키워드 클릭 오브제 생성(+위치)
        */
        //robj = GameObject.FindGameObjectsWithTag("click");
        

        /*
        foreach(string line in contents)
        {
            m_Message += line + '\n';
        }
        m_TypingText.text = "";
        //StartCoroutine(Typing(m_TypingText, contents[current], m_Speed));
        */
    }

    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!reading)
        {
            reading = true;
            typing_speed = m_Speed;

            if (contents.Length == current)
            {
                Debug.Log("READING : 다 읽었습니다."); 
                reading = false;
                return;
            }
            StartCoroutine(Typing(m_TypingText, contents[current++] + '\n'));
            //if (contents.Length > current + 1) current++

            //deletekeyword(robj_i);    //이전 문장의 클릭 오브제 비활성화(추후 수정)
        }
        else
            typing_speed = 0.0f;
    }

    IEnumerator Typing(Text typingText, string message)        //현재 줄 출력(한 글자 씩)
    {
        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] == '|')              //키워드 있을 경우(이것도 한 글자 씩 출력?)
            {
                string after_message = message.Substring(i + 1);
                string[] divided_message = after_message.Split('|');
                typingText.text += divided_message[0];
                i += divided_message[0].Length + 1;
                newKeyword(i, message, divided_message[0]);     //키워드 선택 오브제 생성
                //Debug.Log("키워드의 길이 : " + divided_message[0].Length);
            }
            else
            {
                typingText.text += message[i];
            }
            yield return new WaitForSeconds(typing_speed);
        }
        reading = false;
    }

    void newKeyword(int real_position, string message, string keyword_message)  //get view pos
    {
        string before_message = message.Substring(0, real_position);
        string[] divided_message = before_message.Split('|');
        int view_position = real_position;
        int center = 0;

        for(int i = 1; i < divided_message.Length;)    //real pos -> view pos
        {
            string[] divided_keyword = divided_message[i].Split('>');
            string[] divided_keyword2 = divided_keyword[1].Split('<');
            center = divided_keyword2.Length / 2;
            view_position -= (divided_keyword[0].Length + divided_keyword2[0].Length + 11);
            i = i + 2;
        }
        int position = -407 + (view_position) * 40 + center * 40;        //공백 (띄어쓰기는 크기가 달라), 한 글자 크기 : 40, 띄어쓰기 : 20

        keywords = keyword_message;
        robj[robj_i].GetComponent<Keyword>().GetKeyword(keywords, 0);
        RectTransform rect = robj[robj_i].GetComponent<RectTransform>();
        Debug.Log(robj[robj_i].GetComponent<Keyword>().keyword);
        rect.anchoredPosition = new Vector2(position, -275);
        robj_i = (robj_i + 1) % 5;

    }

}
/*
public Text m_TypingText;
public string m_Message;
public float m_Speed = 0.2f;

private string route = Application.dataPath + @"\Resource\Text\main.txt";
private string[] co_txt;
*/
