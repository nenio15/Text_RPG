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

    [SerializeField]    private string cur_scenario = "main_scenario";
    [SerializeField]    private string cur_subscenario = "Main_1";

    [SerializeField]
    private bool reading = false;

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
    public Textchanger textchanger = new Textchanger();
    //Keyword key = new Keyword();
    [SerializeField]
    private GameObject Selection;
    
    float typing_speed = 0.2f;
    string[] contents;

    private void Start()
    {
        real_main = Application.dataPath + @"\Resource\Text\main.txt";
        System.IO.File.WriteAllText(real_main, ""); //reset
        m_TypingText.text = "";
        m_ped = new PointerEventData(null);
        typing_speed = m_Speed;

        //�ӽ�
        cur_scenario = "scenario";
        cur_subscenario = "medium_0";

        textchanger.readScenarioParts(idx++, cur_scenario, cur_subscenario);    //json
        contents = System.IO.File.ReadAllLines(real_main);
        //0511�� var �Ŵ����� ����;; ��... �̰� �ڵ� ���̸� ��� ã�� ��ġ�ڳ�;;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (stop_read) return;

        // �ó����� �ϴܶ�
        if (contents.Length == current) //(contents[current] == null)
        {
            Debug.Log("READING[end] : ���� �ó����� " + idx);
            //���⼭ ���� �ó������� ����. �ٸ� num�� ����� �ٸ��ٸ�,
            //return -1�� ������ ���⼭ for������ Ž���� �ʿ䰡 �ִ�.
            keyi = 0;
            sc_keyi = 0;

            textchanger.readScenarioParts(idx++, cur_scenario, cur_subscenario);
            ReadStory(true);
            return;
        }

        // ������ ����
        if (!reading)
        {
            if (contents[current][0] == '#')
            {
                // outbound�� ���..? (....)
                if (contents[current].Contains("#key"))  // ������.. �ڵ�..
                {
                    Debug.Log("READING[key] : stop and call selection");
                    Selection.GetComponent<SelectionManager>().ShowSelection("key", keyi++);
                    
                    if (contents[current].Contains("sc"))    
                    {
                        Debug.Log("READING[sc_key] : ...");
                        sc_keyi++;
                    }
                    stop_read = true;
                    current++;
                    return;

                }else if (contents[current].Contains("sc"))
                {
                    Debug.Log("READING[sc_key] : only");
                    sc_keyi++;
                }
                current++;
            }

            if (robj_i > 0)
                for (; robj_i > 0; robj_i--)
                    robj[robj_i - 1].GetComponent<Keyword>().DelKeyword();
        }

        ReadStory(false);
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
            Debug.Log(contents[current]);
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
                newKeyword(i, message, divided_message[0]);     //Ű���� ���� ������ ����
                //sc_keyi++;  // Ű���� ����ŭ �þ�µ�....?
            }
        }
        return message;
    }

    void newKeyword(int real_position, string message, string keyword_message)  //get view pos
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