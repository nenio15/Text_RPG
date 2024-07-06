using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Json�� ġȯ�� ���� Ŭ����
public class ConvertJson
{ 
    //json���� ������ string�������� ġȯ
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
