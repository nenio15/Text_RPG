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

        // �⺻ �ν����͸� �׸��ϴ�
        DrawDefaultInspector();

        if (myComponent.myList != null && myComponent.myList.items != null)
        {
            // ������ �� �ִ� ����� ǥ���մϴ�
            myComponent.selectedIndex = EditorGUILayout.Popup("Select Item", myComponent.selectedIndex, myComponent.myList.items);
        }
    }

}
