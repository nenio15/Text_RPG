using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Keyword : MonoBehaviour    //, IPointerClickHandler
{   
    [SerializeField] public string keyword; //임시 public
    [SerializeField] public string color;
    [SerializeField] public int idx;

    [SerializeField] private GameObject SelectPaper;
    [SerializeField] private GameObject InfoBook;
    [SerializeField] private GameObject Text;
    [SerializeField] private GameObject Button;

    private Vector3 destination = new Vector3(0.0f, -500.0f, -4.0f);
    private Vector2 speed = Vector2.zero;
    private float time = 0.1f;
    private Textchanger clicked;      // = new Textchanger();

    void Start()
    {
        string root = Application.dataPath + @"\Resource\Text\main.txt";
        time = 0.1f;
        /*
        SelectPaper = GameObject.FindWithTag("select");     //good way?
        InfoBook = GameObject.FindWithTag("info");
        Text = GameObject.FindWithTag("Text");
        */

    }

    public void GetKeyword(string word, int ki)
    {
        //string word = Text.GetComponent<TextManager>().keywords;
        string[] divided1 = word.Split('>');
        string[] divided2 = divided1[1].Split('<');
        string[] divided3 = divided1[0].Split('=');

        keyword = divided2[0];
        color = divided3[1];
        idx = ki;

        //transform.localScale = new Vector2(keyword.Length, 1);
        gameObject.SetActive(true);

        /*
         1. 클릭시 상호작용-> obj의 스크립트 (파란색, 빨간색, 노란색마다 다른 상호작용) (키워드 얻기) serialized로 어떤 정보가 있는지 log확인
         2. 시나리오 json에서 해당 키워드 관련 읽기 ex) "협박" : ["선1"], ["선2"], ....
         3. 다음 문장 출력 / 슬라이드 시 오브제 위치 이동(y축). 삭제여부(종류)
          3-1. 삭제여부는 크게 둘. 전투끝날때까지 / 특정 문장 출력시 삭제.(이 경우는...
          3-2. 삭제는 일단, 다음문장을 출력할때 삭제
         4. obj 클릭 -> type case, initiate paper/book/get. and interaction
         */
    }

    public void DelKeyword()
    {
        gameObject.SetActive(false);
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        //클릭하고 다시 클릭하는 알고리즘은 어떻게 짤까..
        if (color == "blue") //blue
        {
            destination = new Vector3(0.0f, -500.0f, -4.0f);
            time = 0.2f;
            //StartCoroutine(Moving(SelectPaper));
            Button.SetActive(true);
            //Invoke("ButtonSet", 2f);
        }
    }
    */
    /*
    IEnumerator Moving(GameObject obj)
    {
        //Debug.Log("moving");
        while (obj.transform.position != destination)
            yield return obj.transform.position = Vector2.SmoothDamp(obj.transform.position, destination, ref speed, time);
    }
    */

}
