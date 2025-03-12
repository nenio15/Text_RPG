using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiManager : MonoBehaviour
{
    [SerializeField] private GameObject cur_class;  // 현재는 텍스트, 나중엔 이미지
    [SerializeField] private PlayerHealth player_data; // 플레이어의 직접정보 참조

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
        //초기 세팅.
        //healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = player_data.startingHealth;
        healthSlider.value = player_data.health;
        //반영
        UploadToGame();
        
        
        //UploadToData();
    }

   

    //인게임에 반영
    public void UploadToGame()
    {
        Character info = player_data.player_info;
        //레벨, 스탯, hp,mp, 이름 전부
        //level.text = info.Level.ToString();
        //stats.text = info.Stat[0] + "\t\t" + info.Stat[2] + "\t\t" + info.Stat[4] + "\n" + info.Stat[1] + "\t\t" + info.Stat[3] + "\t\t" + info.Stat[5];
        hpBar.text = player_data.health + "/" + player_data.startingHealth; //info.Hp[0]의 수정은 따로 필요한지? (dataUpload)
        mpBar.text = player_data.mana + "/" + player_data.maxMana;
        classname.text = info.Name;

        //바
        healthSlider.value = player_data.health;
    }

}
