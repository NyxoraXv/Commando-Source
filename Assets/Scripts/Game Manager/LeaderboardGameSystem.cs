using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System;
using UnityEngine.UI;

public class LeaderboardGameSystem : MonoBehaviour
{
    private LeaderboardData leaderboardData;
    [SerializeField] private TextMeshProUGUI myScore;
    [SerializeField] private TextMeshProUGUI myName;
    [SerializeField] private Transform leaderboardParent;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private int maxEntries;
    [SerializeField] private float gapBetweenEntries = 10f;
    [SerializeField] private float startingPosY = 0f;
    [SerializeField] private float startingPosX = 0f;
    [SerializeField] private Color first, second, third;

    private void OnEnable()
    {
        LeaderboardData();
        myScore.text = SaveManager.Instance.playerData.statistic.data.score.ToString();
        myName.text = SaveManager.Instance.playerData.userData.data.username;
    }

    public void RefreshData()
    {
        LeaderboardData();
    }

    public void LeaderboardData()
    {
        StartCoroutine(GetLeaderboardData());
    }

    IEnumerator GetLeaderboardData()
    {
        Debug.Log("Starting GetLeaderboardData coroutine...");
        LoadingAnimation.Instance.toggleLoading();
        Debug.Log("access-token = " + SaveManager.Instance.playerData.accessTokenResponse.data.access_token);
        string url = SaveManager.Instance.serverUrl + "/statistics/leaderboards";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", SaveManager.Instance.playerData.accessTokenResponse.data.access_token);
        request.certificateHandler = new CertificateWhore();
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string scoreData = request.downloadHandler.text;
            LeaderboardData leaderboardData = JsonUtility.FromJson<LeaderboardData>(scoreData);

            // Sort leaderboard data by score
            Array.Sort(leaderboardData.data, (x, y) => y.score.CompareTo(x.score));

            foreach (Transform child in leaderboardParent)
            {
                Destroy(child.gameObject);
            }

            Vector3 currentPosition = new Vector3(startingPosX, startingPosY, 0f);

            for (int i = 0; i < leaderboardData.data.Length && i < maxEntries; i++)
            {
                GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardParent);
                entry.transform.SetParent(leaderboardParent, false);
                entry.transform.position = currentPosition;

                TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = leaderboardData.data[i].username;
                texts[1].text = leaderboardData.data[i].score.ToString();
                texts[2].text = (i + 1).ToString(); // Assuming the third text is for the rank number
                if (i == 0) // Gold for top rank
                {
                    entry.GetComponent<Image>().color = first;
                }
                else if (i == 1) // Silver for second rank
                {
                    entry.GetComponent<Image>().color = second;
                }
                else if (i == 2) // Bronze for third rank
                {
                    entry.GetComponent<Image>().color = third;
                }
                else // For other ranks, you can set a default color
                {
                    entry.GetComponent<Image>().color = Color.white;
                }
                currentPosition.y -= gapBetweenEntries;
            }
            LoadingAnimation.Instance.stopLoading();
        }
        else
        {
            Debug.LogError("Failed to get leaderboard data: " + request.error);
            LoadingAnimation.Instance.stopLoading();
        }
        Debug.Log("GetLeaderboardData coroutine finished.");
        LoadingAnimation.Instance.stopLoading();
    }

}
