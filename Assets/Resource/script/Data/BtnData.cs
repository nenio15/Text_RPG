using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnData : MonoBehaviour
{
    public Text displayText;
    public TextMeshProUGUI levelDescriptionText;
    public Image image;

    [Header("INFO")]
    public string displayDescription;
    public string actionType;
    public int difficulty;
    public string diceType = "";

    public int index;
    public bool chosen = false;


    public void Active(string display, string dicetype = "", int diff = 0)
    {
        displayText.text = display;
        levelDescriptionText.text = dicetype;
        difficulty = diff;
        if (dicetype != null)
        {
            //여기서 난이도 조절 함수 추가.
            //difficulty -= LevelDown();
            levelDescriptionText.text += "\n" + difficulty;
        }

    }

    public void Press()
    {
        if (!chosen)
        {
            chosen = true;
            //이미지 변경...
            //image.color = 0x010100;
        }
        else
        {
            chosen = false;
            //image.color = 0xAABEF5;
        }

    }

}
