using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public enum Difficulty
{
    Easy,
    Intermediate,
    Hunter,
    Professional,
    Chainkiller
}

public enum RewardType
{
    XP,
    Gold,
    Diamond,
    Item
}

[System.Serializable]
public class mission_data
{
    public string id;
    public string name;
    public int user_level;
    public int width;
    public int height;
    public int seed;
    public int total_enemy;
    public int created_at;
    public int updated_at;
}

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    private SaveManager saveManager;

    public mission_data mission;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        saveManager = SaveManager.Instance;
    }

    public void fetch()
    {
        StartCoroutine(fetch_i());
    }

    IEnumerator fetch_i()
    {
        string url = saveManager.serverUrl + "/manager-levels/users";
        string access_token = SaveManager.Instance.playerData.accessTokenResponse.data.access_token;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new CertificateWhore();
        request.SetRequestHeader("Authorization", access_token);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            mission_data mission_cache = JsonConvert.DeserializeObject<mission_data>(json);
            mission = mission_cache;
        }
    }




}
