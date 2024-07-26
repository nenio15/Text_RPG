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


    public void Active(string display, string dice)
    {
        displayText.text = display;
        levelDescriptionText.text = dice;
        if (dice != "") levelDescriptionText.text += "\n" + difficulty;

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
