using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Networking;

public class HistoryDataButton : MonoBehaviour
{
    public GameObject historyEntryPrefab; // Reference to the HistoryEntry prefab
    public Transform contentTransform; // Reference to the Content transform in the Scroll View

    private void OnEnable()
    {
        GetHistoryData(OnHistoryDataReceived);
    }

    public void GetHistoryData(Action<HistoryData, bool, string> onComplete = null)
    {
        StartCoroutine(FetchHistoryDataCoroutine(onComplete));
    }

    private IEnumerator FetchHistoryDataCoroutine(Action<HistoryData, bool, string> onComplete)
    {
        string url = SaveManager.Instance.serverUrl + "/statistics/histories?order=desc&limit=100";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new CertificateWhore(); // Ensure this is properly handled
        request.SetRequestHeader("Authorization", SaveManager.Instance.playerData.accessTokenResponse.data.access_token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonData = request.downloadHandler.text;
            HistoryData historyData = JsonConvert.DeserializeObject<HistoryData>(jsonData);
            onComplete?.Invoke(historyData, true, null);
        }
        else
        {
            string errorMessage = "Failed to fetch history data: " + request.error;
            Debug.LogError(errorMessage);
            onComplete?.Invoke(null, false, errorMessage);
        }
    }

    private void OnHistoryDataReceived(HistoryData historyData, bool success, string error)
    {
        if (success)
        {
            Debug.Log(historyData.ToString());
            DisplayHistoryData(historyData); // Display the history data
        }
        else
        {
            Debug.LogError("Failed to fetch history data: " + error);
            // Handle error condition, e.g., display an error message
        }
    }

    private void DisplayHistoryData(HistoryData historyData)
    {
        if (historyEntryPrefab == null)
        {
            Debug.LogError("History entry prefab is not assigned!");
            return;
        }

        if (contentTransform == null)
        {
            Debug.LogError("Content transform is not assigned!");
            return;
        }

        // Clear existing history entries
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // Calculate initial position from top of viewport
        RectTransform contentRectTransform = contentTransform.GetComponent<RectTransform>();
        float startY = contentRectTransform.rect.height / 2.25f - 100f; // Start at half the height of the content transform
        float spacingY = 250f; // Adjust as needed for spacing between entries

        // Create a history entry for each item in historyData
        for (int i = 0; i < historyData.data.Count; i++)
        {
            var entry = historyData.data[i];

            // Instantiate history entry prefab
            GameObject historyItem = Instantiate(historyEntryPrefab, contentTransform);

            // Set position for the new entry
            float posY = startY - i * spacingY;
            historyItem.transform.localPosition = new Vector3(0, posY, 0);

            // Set data on the history entry
            Transform idTextTransform = historyItem.transform.Find("ID/OutputText");
            if (idTextTransform != null)
            {
                idTextTransform.GetComponent<TMP_Text>().text = entry.id;
            }

            Transform scoreTextTransform = historyItem.transform.Find("Score/OutputText");
            if (scoreTextTransform != null)
            {
                scoreTextTransform.GetComponent<TMP_Text>().text = entry.score.ToString();
            }

            Transform luncTextTransform = historyItem.transform.Find("LUNC/OutputText");
            if (luncTextTransform != null)
            {
                luncTextTransform.GetComponent<TMP_Text>().text = entry.lunc.ToString();
            }

            Transform frgTextTransform = historyItem.transform.Find("FRG/OutputText");
            if (frgTextTransform != null)
            {
                frgTextTransform.GetComponent<TMP_Text>().text = entry.frg.ToString();
            }

            Transform createdAtTextTransform = historyItem.transform.Find("Current Mission Level/OutputText");
            if (createdAtTextTransform != null)
            {
                createdAtTextTransform.GetComponent<TMP_Text>().text = entry.created_at.ToString();
            }

            Transform lastLevelTextTransform = historyItem.transform.Find("Account Level/OutputText");
            if (lastLevelTextTransform != null)
            {
                lastLevelTextTransform.GetComponent<TMP_Text>().text = entry.last_level.ToString();
            }

            Transform levelTextTransform = historyItem.transform.Find("Account XP/OutputText");
            if (levelTextTransform != null)
            {
                levelTextTransform.GetComponent<TMP_Text>().text = entry.level.ToString();
            }
        }
    }

}
