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
        //���� ������ ����
        diceType = dicetype;
        difficulty = diff;
        displayDescription = display;

        //�� ����
        displayText.text = display;
        levelDescriptionText.text = dicetype;

        //���̽� ������..
        if (dicetype != "")
        {
            //���⼭ ���̵� ���� �Լ� �߰�.
            //difficulty -= LevelDown();
            levelDescriptionText.text += "\n" + difficulty;
        }

    }

    public void Press()
    {
        if (!chosen)
        {
            chosen = true;
            //�̹��� ����...
            //image.color = 0x010100;
        }
        else
        {
            chosen = false;
            //image.color = 0xAABEF5;
        }

    }

}
