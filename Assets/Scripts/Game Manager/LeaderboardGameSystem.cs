using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System;
using UnityEngine.UI;

[Serializable]
public class ScoreData
{
    public RankData[] data;
    public object errors;
    public string message;
    public int code;
}

[Serializable]
public class RankData
{
    public string username;
    public int coin;
    public int star;
    public int score;
    public string address_wallet;
    public int rank;
}

[Serializable]
public class GameDetail
{
    public string id;
    public string user_id;
    public int token;
    public string game_type_id;
}

public class LeaderboardGameSystem : MonoBehaviour
{
    private string serverUrl = "https://lunc-zombie.garudaverse.io";
    private ScoreData scoreDataCls;
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
        myScore.text = SaveManager.Instance.playerData.playerInformation.PlayerScore.ToString();
        myName.text = SaveManager.Instance.playerData.playerInformation.PlayerName;
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
        string url = serverUrl + "/v2/game/statistics?order=desc&query=combined&limit=50&offset=0&short=null";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Token", SaveManager.Instance.playerData.playerInformation.accessToken);
        request.certificateHandler = new CertificateWhore();
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {


            string scoreData = request.downloadHandler.text;
            scoreDataCls = JsonUtility.FromJson<ScoreData>(scoreData);

            Array.Sort(scoreDataCls.data, (x, y) => y.score.CompareTo(x.score));

            foreach (Transform child in leaderboardParent)
            {
                Destroy(child.gameObject);
            }

            Vector3 currentPosition = new Vector3(startingPosX, startingPosY, 0f);

            for (int i = 0; i < scoreDataCls.data.Length && i < maxEntries; i++)
            {
                GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardParent);
                entry.transform.SetParent(leaderboardParent, false);
                entry.transform.position = currentPosition;

                TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = scoreDataCls.data[i].username;
                texts[1].text = scoreDataCls.data[i].score.ToString();
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
