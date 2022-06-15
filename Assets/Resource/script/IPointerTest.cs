using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IPointerTest : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, 
    IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public GameObject thisobj;

    // Start is called before the first frame update
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(thisobj.name + " click!");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(thisobj.name + " down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(thisobj.name + " up");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(thisobj.name + " enter!!");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(thisobj.name + " exit!!");
    }

}
