using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class RankSystem : MonoBehaviour
{
    public GameObject rankColumnPrefab;
    public Transform scrollViewContent;

    [Header("Assign TextMeshPro objects for rank, player name, and score")]
    public TextMeshProUGUI rankNumberText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI scoreText;
    private int score;
    private new string name;
    private int rank = 1;


    [Header("Personal")]
    public TextMeshProUGUI playerNameTextPersonal;
    public TextMeshProUGUI scoreTextPersonal;
    private void OnEnable()
    {
        score = SaveManager.Instance.score;
        name = SaveManager.Instance.playerData.playerInformation.PlayerName;
        UpdateRankUI();
    }

    private void UpdateRankUI()
    {
        playerNameTextPersonal.text = name;
        scoreTextPersonal.text = score.ToString();


            rankNumberText.text = rank.ToString();
            playerNameText.text = name;
            scoreText.text = score.ToString();
    }
}
