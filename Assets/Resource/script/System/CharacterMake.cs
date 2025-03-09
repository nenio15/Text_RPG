using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class CharacterMake : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private GameObject Setting;

    [SerializeField] private GameObject Name;
    [SerializeField] private Toggle Sex;

    [SerializeField] private GameObject Stat_info;
    [SerializeField] private GameObject[] Stat_leftbutton;
    [SerializeField] private GameObject[] Stat_rightbutton;
    [SerializeField] private GameObject Reset;

    [SerializeField] private GameObject Subwindow;
    [SerializeField] private GameObject classlist;
    [SerializeField] private GameObject pastlist;

    [SerializeField] private GameObject scenario;
    [SerializeField] private GameObject scenariolist;
    [SerializeField] private GameObject Describe;

    private int[] stat = { 1, 1, 1, 1, 1, 1 };

    // Start is called before the first frame update
    void Start()
    {
        //초기화
        CloseSub();
    }

    // 창 닫기
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(Subwindow.activeSelf);
        if(Subwindow.activeSelf == true) CloseSub();
    }

    private void CloseSub()
    {
        Subwindow.SetActive(false);
        classlist.SetActive(false);
        pastlist.SetActive(false);
    }

    public void ResetStat()
    {
        for (int i = 0; i < 6; i++) stat[i] = 1;
        Stat_info.GetComponent<TextMeshProUGUI>().text = "1\n1\n1\n1\n1\n1";
    }

    public void ButtonUp(int idx)
    {
        //idx == -6 ~ 5.
        if (idx < 0) stat[(idx) * -1 - 1]--;
        else stat[idx]++;


        string text = "";
        for(int i = 0; i < 6; i++)
        {
            text += stat[i];
            text += "\n";
        }

        
        Stat_info.GetComponent<TextMeshProUGUI>().text = text;
    }


    public void ClassSelect(string name)
    {


    }
}
