using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField] private Transform Field;

    [SerializeField] private float width = 0.1f;

    public Vector3[] Markers;

    private Vector3 FieldFixPos;
    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 1;
        line.startWidth = line.endWidth = width;
    }

    public void DrawLine(Vector3 startPos, Vector3 endPos, bool reset)
    {
        //리셋.. 라인을 여러번 그리면 어떻게 리셋함? 서치 필요.
        if (reset)
        {
            line.positionCount = 1;
            Markers[line.positionCount - 1] = startPos;
        }

        line.SetPosition(line.positionCount - 1, startPos);
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, endPos);

        Markers[line.positionCount - 1] = endPos;

        FieldFixPos = Field.position;

    }

    //맵 스크롤시 좌표 추적.
    private void Update()
    {
        Vector3 movement = Field.position - FieldFixPos;

        for (int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, Markers[i] + movement);
        }



        /*
        if (Input.GetMouseButton(0))
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0f;
            if (Vector3.Distance(currentPosition, previousPosition) > minDistance)
            {
                //line.SetPosition(1, currentPosition);
                //first point
                if (previousPosition == transform.position)
                {
                    line.SetPosition(0, currentPosition);
                }
                else
                {
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, currentPosition);
                }

                previousPosition = currentPosition;
            }
        }
        */
    }


}
