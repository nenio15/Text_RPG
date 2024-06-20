using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextManager : MonoBehaviour, IPointerClickHandler
{
    
    public Text m_TypingText;
    public float m_Speed = 0.2f;
    private int idx = 0;
    private string m_Message;
    private int current = 0;
    
    private bool stop_read = false;

    [SerializeField] private string cur_scenario = "main_scenario";
    [SerializeField] private string cur_subscenario = "Main_1";

    [SerializeField] private bool reading = false;
    [SerializeField] private int page = 0;
    //[SerializeField] public bool eventcall = false;

    private int keyi = 0;
    private int sc_keyi = 0;

    [SerializeField]
    private GameObject clickobj;

    [Header("RAYCASYER")]
    public Canvas m_canvas;
    public GraphicRaycaster m_gr;
    public PointerEventData m_ped;

    [Header("C_KEYWORD")]
    public string keywords;   //�ʱ�ȭ
    [SerializeField] private int robj_i = 0;
    public GameObject[] robj;   //color Ŭ�� ������

    private string real_main;
    //public Textchanger textchanger = new Textchanger();
    //Keyword key = new Keyword();
    [SerializeField] private GameObject Selection;
    [SerializeField] private Textchanger textchanger;
    private EventInformer eventInformer = new EventInformer();

    float typing_speed = 0.2f;
    string[] contents;

    private void Start()
    {
        real_main = Application.dataPath + @"\Resource\Text\main.txt";
        System.IO.File.WriteAllText(real_main, ""); //reset
        m_TypingText.text = "";
        m_ped = new PointerEventData(null);
        typing_speed = m_Speed;

        //�ӽ� scenario/medium_0   //main_scenario/Main_1
        //cur_scenario = "town";  //"scenario" "town" "region"
        //cur_subscenario = "plain_town"; //"medium_0" "plain_town" "Forest"

        textchanger.ReadScenarioParts(idx++, cur_scenario);//, cur_subscenario);    //json
        contents = System.IO.File.ReadAllLines(real_main);
    }

    // text click
    public void OnPointerClick(PointerEventData eventData)
    {
        // �д� �� �ߴ�.   ������ �켱.
        if (stop_read) return;

        // ������ �б� ����
        if (!reading)
        {
            //������ �б� �� ����(#����)       //Debug.Log(current + " and " + contents.Length);
            if (contents.Length == current) EndStoryPart(++page, "", "");

            //�̺�Ʈ ���� ���� // �÷��׸� ���� �������ϳ�?
            //if (eventcall) EventCalling();

            //������ ���� ���� 
            if (contents[current][0] == '#')
            {
                //�̺�Ʈ ���� ����
                if (contents[current].Contains("#key"))
                    if (eventInformer.CheckInsertEvent(cur_scenario))
                    {
                        EventCalling(0, "main_scenario", "Main_1");
                        ReadStory(false);
                        return;
                    }


                //������ show
                Debug.Log("reuturn to original space");
                MeetSign();
            }

        }
        ReadStory(false);
    }

    private void EventCalling(int move, string main, string sub)
    {
        //eventcall = false;

        //event choose (say something..) ���ϰ� �ҷ�


        //before save and event insert
        Debug.Log("[CALLING_EVENT] : Main_1 1...");
        eventInformer.ScenarioSaveTmp();
        EndStoryPart(move, main, sub);
        //and back my save.... how?

    }

    // ������ ����
    private void MeetSign()
    {
        //# �ִ� ����

        // ���� �д� �������� ������
        if (contents[current].Contains("#key"))
        {           
            //�������� ������ //Debug.Log("READING[key] : stop and call selection");
            Selection.GetComponent<SelectionManager>().ShowSelection("key", keyi++);

            if (contents[current].Contains("sc"))
            {
                Debug.Log("READING[sc_key] : ...");
                sc_keyi++;
            }
            stop_read = true;
            current++;
            return;

        }
        else if (contents[current].Contains("#near"))
        {
            Debug.Log("READING[nearby] : ...");


        }
        else if (contents[current].Contains("sc"))
        {
            Debug.Log("READING[sc_key] : only");
            sc_keyi++;
        }
        current++;
        

        //Ű���� ���� �ʱ�ȭ
        if (robj_i > 0)
            for (; robj_i > 0; robj_i--)
                robj[robj_i - 1].GetComponent<Keyword>().DelKeyword();
        stop_read = false;

    }

    //ȭ�� ����
    public void ClearText()
    {
        Debug.Log("clearing...");
        keyi = 0;
        sc_keyi = 0;
        current = 0;
        reading = false;
        //text reset
        m_TypingText.text = "";
    }

    public void EndStoryPart(int move, string next_main, string next_sub)
    {
        //���� �ó����� ������ ���� �ó�����
        Debug.Log("READING[end] : ���� �ó����� " + idx);
        ClearText();  

        if (next_main == "") // �̰Ÿ� Ȱ���ϸ� ������ ������
        {
            //move�� page, num���� �̵�                  // if (move == 0) textchanger.readScenarioParts(idx++, cur_scenario, cur_subscenario); // ���ʿ� 0�̸� +1�̶� ����. ������.
            textchanger.ReadScenarioParts(move, cur_scenario);//, cur_subscenario); 
            idx += move; // ... why?
            page = move;
        }
        else  //cur scenario end or escape
        {
            cur_scenario = next_main;
            cur_subscenario = next_sub;

            textchanger.ReadScenarioParts(0, next_main);//, next_sub);
            page = 0;
        }

        contents = System.IO.File.ReadAllLines(real_main);
        return;
    }

    public void ReadStory(bool changed)
    {
        // �����Ѵ��� �������� changed�� ������ ������ (�ٽ� Ȯ���Ұ�)
        if (changed)
        {
            contents = System.IO.File.ReadAllLines(real_main);
            for (; robj_i > 0; robj_i--)
                robj[robj_i - 1].GetComponent<Keyword>().DelKeyword();
        }
        
        if (!reading)   //normal reading
        {
            string cur_text = "";
            Debug.Log("Before line : " + contents[current-1 > 0 ? current-1 : 0]);
            Debug.Log("cur line pos : " + current);
            //Debug.Log("page pos : " + contents[current]);
            while (contents[current][0] != '#')
                cur_text += STyping(contents[current++] + '\n');
            cur_text += '\n';

            


            reading = true;
            typing_speed = m_Speed;
            StartCoroutine(Showing(m_TypingText, cur_text));
            //if (contents.Length >= current) current++;
        }
        else            // fast reading
            typing_speed = 0.0f;

        stop_read = false;
    }

    

    IEnumerator Showing(Text typingText, string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            //Ű���� ���� ���
            if (message[i] == '|')
            {
                string after_message = message.Substring(i + 1);
                string[] divided_message = after_message.Split('|');
                typingText.text += divided_message[0];
                i += divided_message[0].Length + 1;
                //newKeyword(i, message, divided_message[0]);     //Ű���� ���� ������ ����
                //Debug.Log("Ű������ ���� : " + divided_message[0].Length);
            }
            else
            {
                typingText.text += message[i];
            }
            yield return new WaitForSeconds(typing_speed);
        }
        reading = false;
    }

    string STyping(string message)        //���� �� ���(�� ���� ��)
    {
        for (int i = 0; i < message.Length; i++)
        {
            //Ű���� ���� ���
            if (message[i] == '|')
            {
                string after_message = message.Substring(i + 1);
                string[] divided_message = after_message.Split('|');
                i += divided_message[0].Length + 1;
                NewKeyword(i, message, divided_message[0]);     //Ű���� ���� ������ ����
                //sc_keyi++;  // Ű���� ����ŭ �þ�µ�....?
            }
        }
        return message;
    }

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
        int position = -407 + (view_position) * 40 + center * 40;        //���� (����� ũ�Ⱑ �޶�), �� ���� ũ�� : 40, ���� : 20

        keywords = keyword_message;
        robj[robj_i].GetComponent<Keyword>().GetKeyword(keywords, sc_keyi);
        RectTransform rect = robj[robj_i].GetComponent<RectTransform>();
        //Debug.Log("KEYWORD[obj] : " + robj[robj_i].GetComponent<Keyword>().keyword);
        rect.anchoredPosition = new Vector2(position, -275);
        robj_i = (robj_i + 1) % 5;

    }

}