using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class CharlistSlot : MonoBehaviour
{
     
    //public Button button;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI idText;
    //public ItemSlot itemslot;
    //private Outline outline;


    public int index = -1;

    public void Set(string name, string id, int i)
    {
        nameText.text = name;
        idText.text = id;
        index = i;
        //LoadSprite("Picture/Item/" + itemslot.itemData.img);
        //icon.sprite = Resources.Load<Sprite>("Picture/Item/" + itemslot.itemData.img);
        //Canvas.ForceUpdateCanvases();
    }

    private void LoadSprite(string key)
    {
        Addressables.LoadAssetAsync<Sprite>(key).Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                icon.sprite = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to load sprite: " + key);
            }
        };
    }

    public void Clicked()
    {
        CharlistManager.Instance.Selected(idText.text, index);
    }
}
