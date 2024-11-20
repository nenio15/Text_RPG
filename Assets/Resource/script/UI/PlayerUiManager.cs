using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiManager : MonoBehaviour
{
    [SerializeField] private GameObject cur_class;  // ����� �ؽ�Ʈ, ���߿� �̹���
    [SerializeField] private PlayerHealth player_data; // �÷��̾��� �������� ����

    [Header("TEXT_LIST")]
    //[SerializeField] private Text level;
    //[SerializeField] private Text stats;
    [SerializeField] private TextMeshProUGUI hpBar;
    [SerializeField] private TextMeshProUGUI mpBar;
    //[SerializeField] private Text bar2;
    [SerializeField] private Text classname;

    public Slider healthSlider;
    public Slider manaSlider;
    
    private string  charoute;



    private void Start()
    {
        //�ʱ� ����.
        //healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = player_data.startingHealth;
        healthSlider.value = player_data.health;
        //�ݿ�
        UploadToGame();
        
        
        //UploadToData();
    }

   

    //�ΰ��ӿ� �ݿ�
    public void UploadToGame()
    {
        Character info = player_data.player_info;
        //����, ����, hp,mp, �̸� ����
        //level.text = info.Level.ToString();
        //stats.text = info.Stat[0] + "\t\t" + info.Stat[2] + "\t\t" + info.Stat[4] + "\n" + info.Stat[1] + "\t\t" + info.Stat[3] + "\t\t" + info.Stat[5];
        hpBar.text = player_data.health + "/" + player_data.startingHealth; //info.Hp[0]�� ������ ���� �ʿ�����? (dataUpload)
        mpBar.text = player_data.mana + "/" + player_data.maxMana;
        classname.text = info.Name;

        //��
        healthSlider.value = player_data.health;
    }

}
