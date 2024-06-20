using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "MyList", menuName = "ScriptableObjects/MyList", order = 1)]
public class MyList : ScriptableObject
{
    public string[] items;
}

public class MyComponent : MonoBehaviour
{
    public MyList myList;
    public int selectedIndex;
}



public class ScenarioManager : Editor
{
    public override void OnInspectorGUI()
    {
        MyComponent myComponent = (MyComponent)target;

        // 기본 인스펙터를 그립니다
        DrawDefaultInspector();

        if (myComponent.myList != null && myComponent.myList.items != null)
        {
            // 선택할 수 있는 목록을 표시합니다
            myComponent.selectedIndex = EditorGUILayout.Popup("Select Item", myComponent.selectedIndex, myComponent.myList.items);
        }
    }

}
