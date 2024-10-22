using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionList : MonoBehaviour
{
    private Image[] btn;
    [SerializeField] private PlayerAction playerAction;
    [SerializeField] private TextMeshProUGUI text;
    
    private void Awake()
    {
        btn = GetComponentsInChildren<Image>();
        text.text = "describe something";
    }

    public void UpdateSet()
    {
        for (int i = 0; i < btn.Length; i++)
        {
            if (playerAction.actions[i].img != null)
                btn[i].sprite = Resources.Load<Sprite>("Picture/" + playerAction.actions[i].img);
            else 
                btn[i].sprite = null;
        }

    }

    //액션 취소. 근데 이렇게 빙 돌아가야하나..
    public void RemoveAction(int index)
    {
        playerAction.RemoveAction(index);
        UpdateSet();
    }
}
