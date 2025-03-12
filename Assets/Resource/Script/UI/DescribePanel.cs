using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DescribePanel : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI type;
    public TextMeshProUGUI value;
    public TextMeshProUGUI limit;
    public TextMeshProUGUI detail;
    public TextMeshProUGUI sell;

    public TextMeshProUGUI usebtn;

    private void Awake()
    {
        
    }

    public void Set(ItemSlot item) // pos랑 data값.
    {
        //이동값이 필요합니다ㅏㅏㅏㅏㅏ
        if(item == null) return;


        nickname.text = item.itemData.name;
        type.text = item.itemData.type;
        detail.text = item.itemData.detail;
        sell.text = item.itemData.sell.ToString();

        //value.text = item.item.value.ToString(); // 흠좀무 class로 형식 바꿈. 나중에 추가시킬것.
        //limit.text = item.item.limit.ToString();

        usebtn.text = (item.isEquipment) ? "착용한다" : "사용한다"; // 한글... 다국어 씨발.
    }

    //tmp 기능
    public void LoadScene()
    {
        SceneManager.LoadScene("StartScene");
    }

}
