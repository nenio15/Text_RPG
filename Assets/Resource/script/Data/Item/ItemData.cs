using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Itme", menuName = "New Item")]
public class itemData : ScriptableObject
{
    [Header("INFO")]
    public string displayName;
    public string description;
    
    //... �̷������� ����� ���ƾ��ϴ°� �´°�..

}
