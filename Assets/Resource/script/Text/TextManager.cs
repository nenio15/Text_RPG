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
    public string keywords;   //�ʱ�ȭ
    [SerializeField] private int robj_i = 0;
    public GameObject[] robj;   //color Ŭ�� ������. �����ҵ�?

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

        //start scene���� �ó����� �޾ƿ���
        cur_scenario = PlayerPrefs.GetString("Cur_scenario");

        //�б� ����
        textchanger.ReadScenarioParts(idx++, cur_scenario);//, cur_subscenario);    //json
        contents = System.IO.File.ReadAllLines(real_main);
    }

    // text click
    public void OnPointerClick(PointerEventData eventData)
    {
        // �д� ���� �ߴ�.
        if (stop_read) return;

        //null ���� ���� �ӽ� �ڵ�
        if (contents.Length <= current) current = contents.Length - 1;

        // �ؽ�Ʈ�� ��µǴ� ��Ȳ�� �ƴ϶��.
        if (!reading)
        {
            //������ �� ����                                      (#����)
            //if (contents.Length == current) EndStoryPart(++page, "", "");

            

            //#(�ߴ���) ����
            if (contents[current][0] == '#')
                if (MeetSign()) return; 

        }

        //������ �б�
        ReadStory(false);
    }

    //�ߴ��� ó��
    private bool MeetSign()
    {
        stop_read = true;
        //switch�� �ٲ� �ʿ䰡 �ִٸ� �ٲܰ�.(#key���� ��뵵�� �ִٸ�.)
        //# �ִ� ����
        StopCoroutine("Showing");

        // ���� �д� �������� ������
        switch (contents[current])
        {
            case "#jmp":
            case "#rpl":
                textchanger.ReadScenarioParts(textchanger.pre_move, textchanger.pre_main);
                ClearText();
                return true;
            case "#key":
                //�������� ������ //Debug.Log("READING[key] : stop and call selection");
                Selection.GetComponent<SelectionManager>().ShowSelection("key", keyi++, 0);
                current++;
                /*
                if (contents[current].Contains("sc"))
                {
                    Debug.Log("READING[sc_key] : ...");
                    sc_keyi++;
                }
                */
                //���빰 ���� ����
                contents = System.IO.File.ReadAllLines(real_main);
                //stop_read = true;
                return true;
            case "#btl":
                //���������� ���ϰų�, ��������� ���ϰų�.
                current++; //���� �� ���� �� �б�
                GameObject.Find("Battle").GetComponent<BattleManager>().BattleEntry();
                return true;
            default:
                current++;
                stop_read = false;
                break;

        }


        
        //Ű���� ���� �ʱ�ȭ -> �̰͵� �̻� ��ų��.
        if (robj_i > 0)
            for (; robj_i > 0; robj_i--)
                robj[robj_i - 1].GetComponent<Keyword>().DelKeyword();
        //stop_read = false;

        return false;

    }

    //��ũ��Ʈ + �� ����
    public void ClearText()
    {
        keyi = 0;
        sc_keyi = 0;
        current = 0;
        reading = false;
        stop_read = false;
        m_TypingText.text = "";

        //���� ä���
        contents = System.IO.File.ReadAllLines(real_main);
    }

    //main �о��
    public void ReadStory(bool changed)
    {
        //contents�� �迭�� Ȯ���Ǿ������? �׷� �ȵǴµ�..��.
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

            //Ÿ���� ȿ��
            typing_speed = m_Speed;
            StartCoroutine(Showing(m_TypingText, cur_text));

            //�ߺ� Ŭ���� ���� ������� ��ȯ
            reading = true;
        }
        else            // fast reading
            typing_speed = 0.0f;

    }

    //�ܺο��� �б� �����
    public void Reread()
    {
        reading = false;
        stop_read = false;
        contents = System.IO.File.ReadAllLines(real_main);
    }

    
    //Ÿ���� ȿ��
    IEnumerator Showing(Text typingText, string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            //Ű���� ���� ��� -> ���� ����
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
        //ù ���
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

    //sc_key�� ���� view ������. �Ƹ� �����ҵ�? ���Ӽ� + ������ ����
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