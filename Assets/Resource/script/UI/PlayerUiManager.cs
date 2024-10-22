using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiManager : MonoBehaviour
{
    [SerializeField] private GameObject cur_class;  // ����� �ؽ�Ʈ, ���߿� �̹���
    [SerializeField] private PlayerHealth player_data; // �÷��̾��� �������� ����

    [Header("TEXT_LIST")]
    [SerializeField] private Text level;
    [SerializeField] private Text stats;
    [SerializeField] private Text bar;
    [SerializeField] private Text classname;

    
    private string  charoute;

    //TextAsset jsonData = Resources.Load("Text/Battle/Player") as TextAsset;
    //var _data = JsonUtility.FromJson<Character>(jsonData.ToString());
    //string jsonstring = JsonUtility.ToJson(object obj);
    //T obj = JsonUtility.FromJson<T>(jsonData);
    private void Start()
    {
        //�ݿ�
        UploadToGame();

        
        //UploadToData();
    }

   

    //�ΰ��ӿ� �ݿ�
    public void UploadToGame()
    {
        //����, ����, hp,mp, �̸� ����
        level.text = player_data.player_info.Level.ToString();
        stats.text = player_data.player_info.Stat[0] + "\t\t" + player_data.player_info.Stat[2] + "\t\t" + player_data.player_info.Stat[4] + "\n" + player_data.player_info.Stat[1] + "\t\t" + player_data.player_info.Stat[3] + "\t\t" + player_data.player_info.Stat[5];
        bar.text = "Hp : " + player_data.player_info.Hp[0] + "/" + player_data.player_info.Hp[1] + " Mp : " + player_data.player_info.Mp[0] + "/" + player_data.player_info.Mp[1];
        classname.text = player_data.player_info.Name;

    }

}
