using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class RankSystem : MonoBehaviour
{
    private int score;
    private new string name;


    [Header("Personal")]
    public TextMeshProUGUI playerNameTextPersonal;
    public TextMeshProUGUI scoreTextPersonal;
    private void OnEnable()
    {
        score = SaveManager.Instance.playerData.statistic.Score;
        name = SaveManager.Instance.playerData.userData.Username;
        UpdateRankUI();
    }

    private void UpdateRankUI()
    {
        playerNameTextPersonal.text = name;
        scoreTextPersonal.text = score.ToString();
    }
}
