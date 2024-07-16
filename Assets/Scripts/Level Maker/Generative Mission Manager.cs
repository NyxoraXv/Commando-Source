using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;


public class GenerativeMissionManager : MonoBehaviour
{
    public static GenerativeMissionManager instance;
    public Mission_Data missionData;

    private void Start()
    {
        instance = this;
    }

    public void fetch(Action OnComplete) { 
        StartCoroutine(fetchMissionData(OnComplete));
    }

    IEnumerator fetchMissionData(Action OnComplete)
    {
        string access = SaveManager.Instance.playerData.accessTokenResponse.data.access_token;
        string url = SaveManager.Instance.serverUrl + "/manager-levels/users";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new CertificateWhore();
        request.SetRequestHeader("Authorization", access);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("JSON Response: " + json);

            if (!string.IsNullOrEmpty(json))
            {
                MissionDataWrapper missionWrapper = JsonConvert.DeserializeObject<MissionDataWrapper>(json);
                if (missionWrapper != null && missionWrapper.data != null)
                {
                    missionData = missionWrapper.data;
                    Debug.Log("Mission Name: " + missionWrapper.data.name);
                }
                else
                {
                    Debug.LogError("Deserialization failed. Mission data is null.");
                }
            }
            else
            {
                Debug.LogError("Empty JSON response.");
            }
        }
        else
        {
            Debug.LogError("Request error: " + request.error);
        }

        OnComplete?.Invoke();
    }

}
