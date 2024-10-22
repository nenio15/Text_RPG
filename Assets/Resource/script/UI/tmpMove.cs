using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmpMove : MonoBehaviour
{
    [SerializeField] private RectTransform Field;

    public Vector3 FieldFixPos;
    public Vector3 BasePos;
    //private RectTransform RectPos;

    
    public void Set(Vector3 pos)
    {
        FieldFixPos = pos;
        BasePos = Field.position;
    }
    

    public void Press()
    {
        //¼öÁ¤
        FieldFixPos += Field.position - BasePos;
        //RectPos.position = FieldFixPos;

        PlayerMovement battlePlayer = FindObjectOfType<PlayerMovement>();
        //if (battlePlayer != null) battlePlayer.StartCoroutine("UpdateRun", FieldFixPos);
        //battlePlayer.UpdateRun(FieldFixPos);
        battlePlayer.pos = FieldFixPos;
        battlePlayer.run = true;
        
        //Debug.Log(FieldFixPos + " : " + battlePlayer.transform.position);
        //move(pos - movedpos)
    }

}
