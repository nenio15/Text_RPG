using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject[] UIButton;
    public Button[] buttons;
    //delegate void ButtonEvent(GameObject off, GameObject on);

    void Swap(GameObject off, GameObject on)
    {
        off.SetActive(false);
        on.SetActive(true);
    }

    void Start()
    {
        //UIButton[1].SetActive(false);
        //ButtonEvent buttonswitch = new ButtonEvent(Swap);
        buttons[1].onClick.AddListener(()=> Swap(UIButton[1], UIButton[0]));
        buttons[0].onClick.AddListener(()=> Swap(UIButton[0], UIButton[1]));
        
    }

    void Update()
    {

    }
}
