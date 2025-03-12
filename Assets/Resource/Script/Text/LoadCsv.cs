using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
/*
[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}

[System.Serializable]
public class LocalizationItem
{
    public LocalizationItem(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
    public string key;
    public string value;
}

public class LoadCsv : MonoBehaviour
{
    LocalizationData localizationData;
    Dictionary<string, string> localizedText;
    bool isReady;

    private void LoadCSVFile()
    {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.dataPath, "csv");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath, Encoding.UTF8);
            string[] stringBigList = dataAsJson.Split('\n');
            localizationData = new LocalizationData();
            localizationData.items = new LocalizationItem[stringBigList.Length];
            for (var i = 1; i < stringBigList.Length; i++)
            {
                string[] stringList = stringBigList[i].Split(',');
                for (var j = 0; j < stringList.Length; j++)
                {
                    localizationData.items[i - 1] = new LocalizationItem(stringList[1], stringList[2]);
                }
            }
        }
    }
    private void LoadGameData()
    {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.dataPath, "txt");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        }
    }

    private void SaveGameData()
    {
        string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.dataPath, "", "txt");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData);
            File.WriteAllText(filePath, dataAsJson);
        }
    }

    public void LoadLocalizedText(string fileName)
    {
        localizedText = new Dictionary<string, string>();
        TextAsset mytxtData = Resources.Load<TextAsset>("Texts/" + fileName);
        //파일 정상적으로 읽어오는지 확인
        //        Debug.Log(Resources.Load<TextAsset>("Texts/"+fileName));
        //        Debug.Log(fileName);
        //        Debug.Log(mytxtData);
        string txt = mytxtData.text;
        if (txt != "" && txt != null)
        {
            string dataAsJson = txt;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                //불러오는 데이터 확인
                //Debug.Log(loadedData.items[i].key + ":" + loadedData.items[i].value);
                //공백데이터가 여러개 들어가면 오류발생
                if (!localizedText.ContainsKey(loadedData.items[i].key))
                    localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }

        isReady = true;
    }

}
*/