using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Json의 치환을 위한 클래스
public class ConvertJson
{ 
    //json형식 파일을 string형식으로 치환
    public string MakeJson(string jpath)
    {
        string str = null;
        using (StreamReader sr = File.OpenText(jpath))
        using (JsonTextReader reader = new JsonTextReader(sr))
        {
            str = sr.ReadToEnd();
            sr.Close();
        }
        return str;
    }
}
