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

public class SaveManager : MonoBehaviour
{

    [Serializable]
    public class Statistic
    {
        public Userdata data;
        public object errors;
        public string message;
        public int code;

    }

    [Serializable]
    public class Userdata
    {
        public string game_id;
        public string id;
        public string user_id;
        public int score;
        public int coin;
        public int star;
        public string username;
        public int last_level;
        public int health;
        public short revive;
        public GameDetail Game;
    }


    public class PlayerData
    {
        public class PlayerInfo
        {
            public string PlayerName { get; set; }
            public int PlayerLevel { get; set; }    
            public int PlayerXP { get; set; }
            public int PlayerLastLevel { get; set; }
            public int PlayerScore { get; set; }
            public string walletAdress { get; set; }
            public string accessToken { get; set; }
            public string refreshToken { get; set; }
            public bool isWalletConnected {  get; set; }
        }

        public class CurrencyInfo
        {
            public float PlayerLUNC { get; set; }
            public float PlayerFRG { get; set; }
        }

        public class CharacterInfo
        {
            public Character SelectedCharacter { get; set; }
            public Dictionary<Character, int> OwnedCharacters { get; set; } = new Dictionary<Character, int>();
        }

        public class OwnedCharacterInformation
        {
            public Character Character { get; set; }
            public int Level { get; set; }

            public OwnedCharacterInformation(Character character, int level)
            {
                Character = character;
                Level = level;
            }
        }

        //public class OwnedWeaponInformation
        //{
        //    public Weapon selectedWeapon;
        //    public List<Weapon> ownedWeapons = new List<Weapon>();
        //}

        public PlayerInfo playerInformation { get; set; } = new PlayerInfo();
        public CurrencyInfo currencyInfo { get; set; } = new CurrencyInfo();
        public CharacterInfo characterInfo { get; set; } = new CharacterInfo();
        //public OwnedWeaponInformation weaponInfo { get; set; } = new OwnedWeaponInformation();
    }


    public static SaveManager Instance;

    public PlayerData playerData;

    private string serverUrl = "https://lunc-zombie.garudaverse.io";

    public string username { get; set; }

    public bool isLogin;
    public bool isGetachievement;

    private Statistic userDataClass;
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
        playerData.playerInformation.PlayerLevel = 1;
        playerData.playerInformation.PlayerXP = 0;
        playerData.currencyInfo.PlayerLUNC = 0f;
        playerData.currencyInfo.PlayerFRG = 0f;
        playerData.playerInformation.PlayerScore = 0;
        playerData.playerInformation.PlayerScore = 0;
        playerData.playerInformation.PlayerLastLevel = 0;

        playerData.characterInfo.SelectedCharacter = Character.Sucipto;
    }

    private bool isGetStatisticCalled = false;
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
            string jsonData = "{\"email\": \"" + Email + "\",\"password\": \"" + Password + "\", \"device\": \"laptop\"}";
            print(jsonData);
            AccountForm.Instance.SignInP(jsonData);
            StartCoroutine(waitLoginScene());
            SetUpDataLoad();
            return true;
        }
        else
        {
            string jsonData = "{\"email\": \"" + Email + "\",\"password\": \"" + Password + "\",\"username\": \"" + Username + "\", \"type\": \"54104f36-04d0-4b54-8d40-f8fb17fdb5cb\"}";
            print(jsonData);
            AccountForm.Instance.SignUpP(jsonData);
            StartCoroutine(waitSignupScene());

            return true;
        }

        /*if (Load())
        {
            SetUpDataLoad();
            Debug.Log("Save Data Loaded");
            isLogin = true;
            return true;
        }
        else
        {
            // Initialize playerData if not loaded
            InitializeNewPlayer(username);
            Save();
            isLogin = true;
            return false;
        }*/
    }

    IEnumerator waitLoginScene()
    {
        while (!AccountForm.Instance.isLogin)
        {
            yield return null;
        }
        SetUpDataLoad();
    }

    IEnumerator waitSignupScene()
    {
        while (!AccountForm.Instance.isLogin)
        {
            yield return null;
        }
        InitializeNewPlayer();
    }

    public void Save()
    {
        Debug.Log("Save");
        SetCoin((int)(playerData.currencyInfo.PlayerFRG * 100f));
        Debug.Log((int)(SaveManager.Instance.playerData.currencyInfo.PlayerFRG * 1f));
        SetScore();
        Debug.Log("score = " + playerData.playerInformation.PlayerScore);
        setLastLevel();
    }

    private void OnApplicationQuit()
    {
        Save();
    }


    public void GetStatistic()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("lost connection please check your internet");
            PopUpInformationhandler.Instance.pop("lost connection please check your internet");
            return;
        }

        if (string.IsNullOrEmpty(playerData.playerInformation.accessToken))
        {
            StartCoroutine(WaitForAccessToken(() => StartCoroutine(GetStatisticRequest())));
        }
        else
        {
            StartCoroutine(GetStatisticRequest());
            StartCoroutine(GetAchievementRequest());
        }
    }

    IEnumerator WaitForAccessToken(Action callback)
    {
        while (string.IsNullOrEmpty(playerData.playerInformation.accessToken))
        {
            yield return null;
        }

        callback?.Invoke();
    }



    IEnumerator GetStatisticRequest()
    {
        string access = playerData.playerInformation.accessToken;
        Debug.Log("Getting Statistic Request");
        string url = serverUrl + "/v2/game/statistic";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new CertificateWhore();
        request.SetRequestHeader("Token", access);
        request.downloadHandler = new DownloadHandlerBuffer();
        Debug.Log(access + "access");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string coinData = request.downloadHandler.text;
            userDataClass = JsonUtility.FromJson<Statistic>(coinData);

            playerData.playerInformation.PlayerScore = userDataClass.data.score;
            playerData.currencyInfo.PlayerFRG = userDataClass.data.coin/100f;
            playerData.currencyInfo.PlayerLUNC= userDataClass.data.coin / 10f;
            //PopUpInformationhandler.Instance.pop("Save Data Loaded");
            playerData.playerInformation.PlayerName = userDataClass.data.username;

            Debug.Log("Score:"+userDataClass.data.score +"Coin:"+ userDataClass.data.coin+"Name"+ userDataClass.data.username);
        }
        else
        {
            Debug.Log(request.error);
            PopUpInformationhandler.Instance.pop("Failed To Load Save Data");
        }
    }

    public void setLastLevel() 
    {
        StartCoroutine(setLastLevelRequest());
    }

    IEnumerator setLastLevelRequest()
    {
        Debug.Log("Setting level: " + playerData.playerInformation.PlayerLastLevel);
        string url = serverUrl + "/v2/game/achievement/";

        // Update the JSON data to include the last_level as an integer value
        string jsonData = "{\"last_level\":" + playerData.playerInformation.PlayerLastLevel + ",\"health\":2,\"revive\":3}";

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.certificateHandler = new CertificateWhore();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Token", playerData.playerInformation.accessToken);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Progress Saved!");
            //PopUpInformationhandler.Instance.pop("Progress Saved!");
        }
        else
        {
            Debug.LogError("Failed to save progress: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            //PopUpInformationhandler.Instance.pop("Failed to save progress");
        }
    }

    public void getAchievement()
    {
        StartCoroutine(GetAchievementRequest());
    }

    IEnumerator GetAchievementRequest()
    {
        string access = playerData.playerInformation.accessToken;
        Debug.Log("Getting Achievement Request");
        string url = serverUrl + "/v2/game/achievement";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new CertificateWhore();
        request.SetRequestHeader("Token", access);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            isGetachievement = true;

            string achievementData = request.downloadHandler.text;
            userDataClass = JsonUtility.FromJson<Statistic>(achievementData);

            playerData.playerInformation.PlayerLastLevel = userDataClass.data.last_level;
            Debug.Log("Last Level = " + userDataClass.data.last_level);

        }
        else
        {
            isGetachievement = false;

            PopUpInformationhandler.Instance.pop("Failed To Load Achievement Data");
        }
    }

    public void SetScore()
    {
        StartCoroutine(SetScoreRequest());
    }

    IEnumerator SetScoreRequest()
    {
        string url = serverUrl + "/v2/game/statistic/score";
        string scoreEncrypt = Encrypt(playerData.playerInformation.PlayerScore);

        // Create JSON data for the request body
        string jsonData = "{\"score\":\"" + scoreEncrypt + "\"}";

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.certificateHandler = new CertificateWhore();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Token", playerData.playerInformation.accessToken);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score added successfully!");
            //SaveManager.Instance.playerData.playerInformation.PlayerScore += score;
            // Optionally, handle successful response here
        }
        else
        {
            Debug.LogError("Failed to add score: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            // Optionally, handle error response here
        }
    }

    public void SetCoin(int coin)
    {
        StartCoroutine(SetCoinRequest(coin));
    }

    IEnumerator SetCoinRequest(int coin)
    {
        Debug.Log("Setting coin: " + coin);

        string url = serverUrl + "/v2/game/statistic/coin";
        string coinEncrypt = Encrypt(coin);

        string jsonData = "{\"coin\":\"" + coinEncrypt + "\"}";

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.certificateHandler = new CertificateWhore();
        request.SetRequestHeader("Token", playerData.playerInformation.accessToken);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Coin added successfully!");
        }
        else
        {
            Debug.LogError("Failed to add coin: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            // Optionally, handle error response here
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
