using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class HistoryDataTester : MonoBehaviour
{
    public Button fetchHistoryButton;

    void Start()
    {
        if (fetchHistoryButton != null)
        {
            fetchHistoryButton.onClick.AddListener(() => GetHistoryData(OnHistoryDataReceived));
        }
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
        }
        else
        {
            Debug.LogError("Failed to fetch history data: " + error);
            // Handle error condition, e.g., display an error message
        }
    }
}
