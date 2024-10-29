using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System;
//using static UnityEditor.LightingExplorerTableColumn;

public class PlayerHealth : LivingEntity
{
    //json 루트
    private string cha_route;
    public JObject player;

    //player의 메인 루트 데이터
    public Character player_info;
    private CharacterData characterData = new CharacterData();

    public AudioClip deathClip;
    public AudioClip hitClip;

    private AudioSource playerAudioPlayer;
    private Animator playerAnimator;

    private PlayerMovement playerMovement;
    

    private void Awake()
    {
        //player info json 가져오기
        cha_route = Application.persistentDataPath + "/Info/Player.json";
        player = characterData.SetJson(cha_route);
        JToken info = player["Info"];
        if(info != null) player_info = JsonUtility.FromJson<Character>(info.ToString());

        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
    }

    //상태 초기화
    protected override void OnEnable()
    {
        startingHealth = player_info.Hp[1];
        startingMana = player_info.Mp[1];
        
        base.OnEnable();
        //Debug.Log(health.ToString());

        playerMovement.enabled = true;
    }

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
    }

    public override void OnDamage(float damage, Vector3 hitPos, Vector3 hitSurface)
    {
        player_info.Hp[0] -= 1; // 중복. 임시 조치. ui바꾸면서 바꿀것.
        base.OnDamage(damage, hitPos, hitSurface);
        transform.position = transform.position - new Vector3(-200, 200);
    }

    public override void Die()
    {
        base.Die();



        //playerAudioPlayer.PlayOneShot(deathClip);
        playerMovement.enabled = false;
        //추가 조작 비활성화
    }

    public override void Revive(float newHealth)
    {
        base.Revive(newHealth);
    }

    //json에 반영. 전투말고, 레벨업 따위에서 갱신. 
    public void UpdateData(string type, int count)
    {
        //player_info.Skill = content; //임시조치. 왜 변경이 안되냐..
        characterData.Upload(type, count, player, cha_route);
    }
}
