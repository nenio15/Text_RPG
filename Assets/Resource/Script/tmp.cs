using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;

public class tmp : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public TextMeshProUGUI text;

    public void Start()
    {
        text.text = "none";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        text.text = "clicked";
    }

    //나중에 모바일 터치 이벤트..
    void Update()
    {
        //Debug.Log(Input.touchCount);
        //Debug.Log(Input.GetTouch(0).position);
        //Debug.Log(Input.GetTouch(0).deltaPosition);
        //Debug.Log(Input.GetTouch(0).tapCount);
        /*
        if (Input.touchCount > 0)
        {
            Debug.Log(Input.touchCount);
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                text.text = "begintouch";
                Debug.Log("Began - 손가락이 화면을 터치하는 그 순간: " + Input.GetTouch(0).position);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                text.text = "movetouch";
                Debug.Log("Moved - 손가락이 화면 위에서 터치한 상태로 이동하고 있는 상태: " + Input.GetTouch(0).position);
            }
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                text.text = "endtouch";
                Debug.Log("lfkjlaj");
            }

        }
        */
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        text.text = "beginDrag";
        //battleField.enabled = true;

        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        text.text = "endDrag";
        //throw new System.NotImplementedException();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        text.text = "onDrag";


        /*
        이거는 스크롤뷰가 아니고, mask일때 쓴것.
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition;

        */
        //throw new System.NotImplementedException();




    }

}
