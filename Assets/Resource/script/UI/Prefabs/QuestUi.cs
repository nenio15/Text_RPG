using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUi : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI goal;
    public TextMeshProUGUI content;
    public TextMeshProUGUI reward;
    


    public void Set(TextMeshProUGUI nickname)//���� �� �ޱ�
    {
        title.text = nickname.text;
        content.text = "����";
    }

}
