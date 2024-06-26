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

    private void SetUpDataLoad()
    {
        GetStatistic();
        CurrencyManager.Instance.Refresh();
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
    }

    public void fetchData()
    {
        GetStatistic();
        CurrencyManager.Instance.Refresh();
    }

    IEnumerator WaitForAccessToken(Action callback)
    {
        while (string.IsNullOrEmpty(playerData.accessTokenResponse.data.access_token))
        {
            yield return null;
        }

        callback?.Invoke();
    }
    public bool SetStatistic(Statistic newStatistic)
    {
        // Use a local variable to store the result
        bool requestSuccess = false;

        StartCoroutine(SetStatisticRequest(newStatistic, result =>
        {
            requestSuccess = result;
        }));

        return requestSuccess;
    }

    public IEnumerator SetStatisticRequest(Statistic newStatistic, Action<bool> resultCallback)
    {
        string url = serverUrl + "/statistics";
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
            CurrencyManager.Instance.Refresh();
            Debug.Log("Statistic data updated successfully!");
            resultCallback(true); // Indicate success
        }
        else
        {
            Debug.LogError("Failed to update statistic data: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            Debug.Log(jsonData);
            resultCallback(false); // Indicate failure
        }
    }

    public void Withdraw(float amount)
    {
        StartCoroutine(WithdrawRequest(amount));
    }

    IEnumerator WithdrawRequest(float amount)
    {
        string url = serverUrl + "/transactions";

        // Convert Statistic object to JSON
        string jsonData = "{\"type\": \"" + "WITHDRAW" + "\",\"amount\":"+amount+ "}";
        LoadingAnimation.Instance.toggleLoading();
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.certificateHandler = new CertificateWhore();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Authorization", playerData.accessTokenResponse.data.access_token);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Withdraw Success!");
            PopUpInformationhandler.Instance.pop("Withdraw Success");
            LoadingAnimation.Instance.stopLoading();
            fetchData();
        }
        else
        {
            Debug.LogError("Failed to Withdraw: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response: " + request.downloadHandler.text);
                LoadingAnimation.Instance.stopLoading();
            }
            Debug.Log(jsonData);
            LoadingAnimation.Instance.stopLoading();
            // Optionally, handle error response here
        }
    }

    public void Burn(float amount)
    {
        StartCoroutine(BurnRequest(amount));
    }

    IEnumerator BurnRequest(float amount)
    {
        string url = serverUrl + "/transactions";

        // Convert Statistic object to JSON
        string jsonData = "{\"type\": \"" + "BURN" + "\",\"amount\":" + amount + "}";
        LoadingAnimation.Instance.toggleLoading();

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.certificateHandler = new CertificateWhore();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Authorization", playerData.accessTokenResponse.data.access_token);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Burn Success!");
            fetchData();
            LoadingAnimation.Instance.stopLoading();
            PopUpInformationhandler.Instance.pop("Burn Success");
        }
        else
        {
            Debug.LogError("Failed to Burn: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            Debug.Log(jsonData);
            LoadingAnimation.Instance.stopLoading();
            // Optionally, handle error response here
        }
    }

    public void GetStatistic(Action<StatisticData, bool, string> onComplete = null)
    {
        StartCoroutine(GetStatisticRequest(onComplete));
    }

    IEnumerator GetStatisticRequest(Action<StatisticData, bool, string> onComplete)
    {
        string access = playerData.accessTokenResponse.data.access_token;
        try{LoadingAnimation.Instance.toggleLoading();}catch (Exception e) { }
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
            CurrencyManager.Instance.Refresh();
            onComplete?.Invoke(statistic, true, null);
            try { LoadingAnimation.Instance.stopLoading(); } catch (Exception e) { }
        }
        else
        {
            string errorMessage = "Failed to fetch statistic data: " + request.error;
            Debug.LogError(errorMessage);
            PopUpInformationhandler.Instance.pop("Failed To Load Statistic Data");
            onComplete?.Invoke(null, false, errorMessage);
            try { LoadingAnimation.Instance.stopLoading(); } catch (Exception e) { }
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
