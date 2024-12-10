using UnityEngine;
using TMPro;
using System;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;
    public float duration = 3f;

    private float timer;

    public void Setup(float damage)
    {
        damageText.text = damage.ToString();
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (timer > duration)
        {
            Color color = damageText.color;
            color.a -= fadeSpeed * Time.deltaTime;
            damageText.color = color;

            if (color.a <= 0) Destroy(gameObject);
        }
    }
}
