using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleFieldMove : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    [SerializeField] private ScrollRect battleField;
    [SerializeField] private GameObject marker;
    [SerializeField] private GameObject line;
    [SerializeField] private GameObject player;

    //�̰� ĵ������ �ٲ� ���� �ٸ���.. �� �׳� �����ұ�.
    [SerializeField] private Transform CanvasPointer;


    //[SerializeField] Vector3 fieldPos;
    private float distance = 10;


    //���߿� ����� ��ġ �̺�Ʈ..
    void Update()
    {
        //Debug.Log(Input.touchCount);
        //Debug.Log(Input.GetTouch(0).position);
        //Debug.Log(Input.GetTouch(0).deltaPosition);
        //Debug.Log(Input.GetTouch(0).tapCount);

        if (Input.touchCount > 0)
        {
            Debug.Log(Input.touchCount);
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Debug.Log("Began - �հ����� ȭ���� ��ġ�ϴ� �� ����: " + Input.GetTouch(0).position);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Debug.Log("Moved - �հ����� ȭ�� ������ ��ġ�� ���·� �̵��ϰ� �ִ� ����: " + Input.GetTouch(0).position);
            }
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("first drag battle field");
        //battleField.enabled = true;

        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {

        
        
        /*
        �̰Ŵ� ��ũ�Ѻ䰡 �ƴϰ�, mask�϶� ����.
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition;

        */
        //throw new System.NotImplementedException();

        

        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);

        ReserveMovement(mousePosition - CanvasPointer.position);
    }

    public void ReserveMovement(Vector3 clickPosition)
    {
        LineDraw draw = line.GetComponent<LineDraw>();
        Vector3 playerPosition = player.transform.position;

        marker.transform.position = clickPosition;
        if (draw != null) draw.DrawLine(playerPosition, clickPosition, true);

    }

}
