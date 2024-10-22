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
    //json ��Ʈ
    private string cha_route;
    public JObject player;

    //player�� ���� ��Ʈ ������
    public Character player_info;
    private CharacterData characterData = new CharacterData();

    public AudioClip deathClip;
    public AudioClip hitClip;

    private AudioSource playerAudioPlayer;
    private Animator playerAnimator;

    private PlayerMovement playerMovement;
    

    private void Awake()
    {
        //player info json ��������
        cha_route = Application.persistentDataPath + "/Info/Player.json";
        player = characterData.SetJson(cha_route);
        JToken info = player["Info"];
        if(info != null) player_info = JsonUtility.FromJson<Character>(info.ToString());

        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
    }

    //���� �ʱ�ȭ
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
        base.OnDamage(damage, hitPos, hitSurface);
    }

    public override void Die()
    {
        base.Die();



        //playerAudioPlayer.PlayOneShot(deathClip);
        playerMovement.enabled = false;
        //�߰� ���� ��Ȱ��ȭ
    }

    public override void Revive(float newHealth)
    {
        base.Revive(newHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (!dead)
        {
            // ��� ������Ʈ ����ͼ� �̸�����
        }
        
    }

    //json�� �ݿ�. ��������, ������ �������� ����. 
    public void UpdateData(int type, string content)
    {
        player_info.Skill = content; //�ӽ���ġ. �� ������ �ȵǳ�..
        //Debug.Log("player skill : " + player_info.Skill);
        characterData.Upload(type, content, player, cha_route);
    }
}
