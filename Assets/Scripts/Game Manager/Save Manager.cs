using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using UnityEditor;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

public class SaveManager : MonoBehaviour
{
    public class PlayerData
    {
        public StatisticData statistic { get; set; } = new StatisticData();

        public AchievementData achievementData { get; set; } = new AchievementData();

        public AccessTokenResponse accessTokenResponse { get; set; } = new AccessTokenResponse();
        
        public OwnedCharacterInformation characterInformation { get; set; } = new OwnedCharacterInformation();
        
        public UserResponseData userData { get; set; } = new UserResponseData();

        public WalletAdressData WalletData = new WalletAdressData();

        public CharacterInfo characterInfo { get; set; } = new CharacterInfo();
    }


    public static SaveManager Instance;

    public PlayerData playerData;

    public string serverUrl;

    public string username { get; set; }

    public bool isLogin;
    public bool isWalletConnected;


    public bool isGetAchievement = false;
    public bool isSetAchievement = false;
    public string Device;
    private bool isGetStatisticCalled = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerData = new PlayerData();

    }

    public void InitializeNewPlayer()
    {
        playerData.achievementData.data.player_level = 1;
        playerData.achievementData.data.player_exp = 0;
        playerData.achievementData.data.last_level = 0;

        playerData.statistic.data.lunc= 0f;
        playerData.statistic.data.frg = 0f;
        playerData.statistic.data.score = 0;

        playerData.characterInfo.SelectedCharacter = Character.Sucipto;
    }

    private void SetUpDataLoad()
    {
        if (!isGetStatisticCalled)
        {
            GetStatistic();
            isGetStatisticCalled = true;
        }
    }



    public bool Verify(string Username, string Password, string Email, bool isLogin)
    {
        if (isLogin) {

            string jsonData = "{\"Username\": \"" + Username + "\",\"Password\": \"" + Password + "\", \"device\": \""+ Device +"\"}";
            print(jsonData);
            AccountManager.Instance.SignInP(jsonData);
            StartCoroutine(waitLoginScene());
            return true;
        }
        else
        {
            string jsonData = "{\"Password\": \"" + Password + "\",\"Username\": \"" + Username + "\",\"Email\": \"" + Email + "\"}";
            print(jsonData);
            AccountManager.Instance.SignUpP(jsonData);
            StartCoroutine(waitSignupScene());

            return true;
        }
    }

    IEnumerator waitLoginScene()
    {
        while (!AccountManager.Instance.isLogin)
        {
            yield return null;
        }
        SetUpDataLoad();
    }

    IEnumerator waitSignupScene()
    {
        while (!AccountManager.Instance.isLogin)
        {
            yield return null;
        }
        InitializeNewPlayer();
    }

    public void Save()
    {
        SetStatistic(playerData.statistic.data);
        SetAchievement(playerData.achievementData.data);
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    IEnumerator WaitForAccessToken(Action callback)
    {
        while (string.IsNullOrEmpty(playerData.accessTokenResponse.data.access_token))
        {
            yield return null;
        }

        callback?.Invoke();
    }
    public void SetStatistic(Statistic newStatistic)
    {
        StartCoroutine(SetStatisticRequest(newStatistic));
    }

    IEnumerator SetStatisticRequest(Statistic newStatistic)
    {
        string url = serverUrl + "/statistics";

        // Convert Statistic object to JSON
        string jsonData = JsonConvert.SerializeObject(newStatistic);

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.certificateHandler = new CertificateWhore();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Authorization", playerData.accessTokenResponse.data.access_token);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Statistic data updated successfully!");
        }
        else
        {
            Debug.LogError("Failed to update statistic data: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            Debug.Log(jsonData);
            // Optionally, handle error response here
        }
    }

    public void GetStatistic()
    {
        StartCoroutine(GetStatisticRequest());
    }

    IEnumerator GetStatisticRequest()
    {
        string access = playerData.accessTokenResponse.data.access_token;
        string url = serverUrl + "/statistics";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new CertificateWhore();
        request.SetRequestHeader("Authorization", access);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonData = request.downloadHandler.text;
            StatisticData statistic = JsonConvert.DeserializeObject<StatisticData>(jsonData);
            playerData.statistic = statistic;
        }
        else
        {
            Debug.LogError("Failed to fetch statistic data: " + request.error);
            PopUpInformationhandler.Instance.pop("Failed To Load Statistic Data");
        }
    }
    public void SetAchievement(Achievement newAchievement)
    {
        StartCoroutine(SetAchievementRequest(newAchievement));
    }

    IEnumerator SetAchievementRequest(Achievement newAchievement)
    {
        string url = serverUrl + "/achievements";

        // Convert Achievement object to JSON
        string jsonData = JsonConvert.SerializeObject(newAchievement);

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.certificateHandler = new CertificateWhore();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Authorization", playerData.accessTokenResponse.data.access_token);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Achievement data updated successfully!");
        }
        else
        {
            Debug.LogError("Failed to update achievement data: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            // Optionally, handle error response here
        }
    }

    public void GetAchievement(Action<AchievementData> callback)
    {
        StartCoroutine(GetAchievementRequest(callback));
    }

    IEnumerator GetAchievementRequest(Action<AchievementData> callback)
    {
        string access = playerData.accessTokenResponse.data.access_token;
        string url = serverUrl + "/achievements";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new CertificateWhore();
        request.SetRequestHeader("Authorization", access);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonData = request.downloadHandler.text;
            Debug.Log(jsonData);
            AchievementData achievement = JsonUtility.FromJson<AchievementData>(jsonData);
            playerData.achievementData = achievement;
            Debug.Log("Last Level = " + achievement.data.last_level);
            callback?.Invoke(achievement);
        }
        else
        {
            Debug.LogError("Failed to fetch achievement data: " + request.error);
            PopUpInformationhandler.Instance.pop("Failed To Load Achievement Data");
        }
    }

    private string RandWord(int lengthWord, bool areNumber = false)
    {
        System.Random random = new System.Random();
        StringBuilder kalimat = new StringBuilder();

        string karakter;
        if (areNumber)
            karakter = "0123456789";
        else
            karakter = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?/";

        for (int i = 0; i < lengthWord; i++)
        {
            int index = random.Next(karakter.Length);
            char karakterAcak = karakter[index];
            kalimat.Append(karakterAcak);
        }

        return kalimat.ToString();
    }

    private string Encrypt(int data)
    {
        try
        {
            char[] coinData, startData;
            List<int> valueUnix = new List<int>();
            List<int> indexing = new List<int>();
            string randomKey = RandWord(95);
            int coin = data;
            TimeSpan t = DateTime.Now - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            secondsSinceEpoch += secondsSinceEpoch;

            coinData = coin.ToString().ToCharArray();
            startData = (randomKey + secondsSinceEpoch.ToString()).ToCharArray();
            for (int i = coinData.Length; i > 0; i--)
            {
                valueUnix.Add(int.Parse((startData[startData.Length - (i + 2)]).ToString()));
            }

            for (int j = 0; j < valueUnix.Count; j++)
            {
                int index;
                if (j != 0)
                    index = (valueUnix[j] + indexing[j - 1]) + 1;
                else
                    index = valueUnix[j];

                indexing.Add(index);
                startData[index] = coinData[j];
            }

            string result = new string(startData);
            result = coinData.Length.ToString() + result;
            Console.WriteLine(result);
            return result;
        }
        catch (System.Exception ex)
        {
            string randomKey = RandWord(95);
            return randomKey + RandWord(20);
        }
    }
}
