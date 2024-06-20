using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveStartScene : MonoBehaviour
{
    public void MoveToMain(TextMeshProUGUI label)
    {
        PlayerPrefs.SetString("Cur_scenario", label.text.ToString());
        SceneManager.LoadScene("MainScene");
    }

}
