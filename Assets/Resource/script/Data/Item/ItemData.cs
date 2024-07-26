using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Itme", menuName = "New Item")]
public class itemData : ScriptableObject
{
    [Header("INFO")]
    public string displayName;
    public string description;
    
    //... 이런식으로 만들어 놓아야하는게 맞는감..

}
