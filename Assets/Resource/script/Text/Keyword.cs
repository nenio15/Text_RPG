using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Keyword : MonoBehaviour, IPointerClickHandler
{   
    [SerializeField] public string keyword; //�ӽ� public
    [SerializeField] private string color;

    [SerializeField] private GameObject SelectPaper;
    [SerializeField] private GameObject InfoBook;
    [SerializeField] private GameObject Text;
    [SerializeField] private GameObject Button;

    private Vector3 destination = new Vector3(0.0f, -500.0f, -4.0f);
    private Vector2 speed = Vector2.zero;
    private float time = 0.1f;
    private Textchanger clicked = new Textchanger();

    void Start()
    {
        string root = Application.dataPath + @"\Resource\Text\main.txt";
        /*
        SelectPaper = GameObject.FindWithTag("select");     //good way?
        InfoBook = GameObject.FindWithTag("info");
        Text = GameObject.FindWithTag("Text");
        */

        //GetKeyword();
        //System.IO.File.r

    }

    public void GetKeyword()
    {
        
        string word = Text.GetComponent<TextManager>().keywords;
        //thinking.. if c -> more complex?
        string[] divided1 = word.Split('>');
        string[] divided2 = divided1[1].Split('<');
        string[] divided3 = divided1[0].Split('=');

        keyword = divided2[0];
        color = divided3[1];

        //transform.localScale = new Vector2(keyword.Length, 1);
        gameObject.SetActive(true); //give cooltime. for ���� ����

        /*
         1. Ŭ���� ��ȣ�ۿ�-> obj�� ��ũ��Ʈ (�Ķ���, ������, ��������� �ٸ� ��ȣ�ۿ�) (Ű���� ���) serialized�� � ������ �ִ��� logȮ��
         2. �ó����� json���� �ش� Ű���� ���� �б� ex) "����" : ["��1"], ["��2"], ....
         3. ���� ���� ��� / �����̵� �� ������ ��ġ �̵�(y��). ��������(����)
          3-1. �������δ� ũ�� ��. �������������� / Ư�� ���� ��½� ����.(�� ����...
          3-2. ������ �ϴ�, ���������� ����Ҷ� ����
         4. obj Ŭ�� -> type case, initiate paper/book/get. and interaction
         */
    }

    public void DelKeyword()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Ŭ���ϰ� �ٽ� Ŭ���ϴ� �˰����� ��� ©��..
        if (color == "blue") //blue
        {
            destination = new Vector3(0.0f, -500.0f, -4.0f);
            time = 0.2f;
            StartCoroutine(Moving(SelectPaper));
            Button.SetActive(true);
            //Invoke("ButtonSet", 2f);

            /*
            if(selection 
                �ó�����json�� �ִ� ������ ����Ʈ ���� �� �ݿ�
            else if(info 
                InfoBook.transform()   ����å ȣ��.. ���� �³�..? 
            else if(battle 
                ��Ʋ ��ȣ�ۿ뾲..
            */
        }
        else if (color == "red")    //red
        {
            //monster
        }
        else if(color == "yellow")    //yellow
        {
            destination = new Vector3(0.0f, 0.0f, -4.0f);
            time = 0.1f;
            StartCoroutine(Moving(InfoBook));
            //get keyword(�̰Ŵ� ������...?)
        }
        else if(color == "secret")    //none, secret
        {
            //Debug.Log("secret");

        }
        //Debug.Log(keyword);
    }
    void deleteKeyword()
    {
        gameObject.SetActive(false);

        //������ �����ϴ� Ű���� �������� �����Ѵ�.
        /* many case..
        1.new line ->
        2.select decision ->
        3. ...?
        */
    }

    IEnumerator Moving(GameObject obj)
    {
        //Debug.Log("moving");
        while (obj.transform.position != destination)
            yield return obj.transform.position = Vector2.SmoothDamp(obj.transform.position, destination, ref speed, time);
    }

}
