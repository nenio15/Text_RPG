using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using System.IO;

public class StartScene_Event : MonoBehaviour
{
    private string route;
    private void Start()
    {
        /*
        //캐릭터 리스트 생성.
        route = UnityEngine.Application.persistentDataPath;
        string str = Resources.Load<TextAsset>("Text/Info/Charlist.json").ToString();
        if(!File.Exists(route + "/Charlist.json")) File.WriteAllText(route + "/Charlist.json", str);
        */
    }

}
