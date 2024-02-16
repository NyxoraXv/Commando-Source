using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;

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
    private string serverUrl = "https://dev-gtp.garudaverse.io";
    private ScoreData scoreDataCls;
    private Statistic coinDataCls;
    [SerializeField] private TextMeshProUGUI myScore, myStars;
    public static LeaderboardGameSystem Instance;

    // public TextMeshProUGUI scoreText;

    private void Start()
    {
        // StartCoroutine(CheckInternetConnection());
        if (myScore != null)
        {
            for (int i = 0; i < 20; i++)
            {
                Username[i].text = "";
                ScorePoint[i].text = "";
                // StarName[i].text = "";
                StarPoint[i].text = "";
            }

            CheckStarScore();
            // GetStatistic(BlockChainActions.accesWallet);
            LeaderboardData();
            // GetAllStars();
            // GetAllScores();
            RefreshData();
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    public void CheckStarScore()
    {
        myScore.text = PlayerPrefs.GetInt("Score").ToString();

        myStars.text = PlayerPrefs.GetInt("Star").ToString();
    }

    public void RefreshData()
    {
        GetStatistic(PlayerPrefs.GetString("AccessToken"));
        LeaderboardData();
    }

    private string RandWord(int lengthWord, bool areNumber = false)
    {
        System.Random random = new System.Random();
        StringBuilder kalimat = new StringBuilder();

        // Set karakter yang diizinkan
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

            // Logs(valueUnix);

            for (int j = 0; j < valueUnix.Count; j++)
            {
                int index;
                if (j != 0)
                {
                    index = (valueUnix[j] + indexing[j - 1]) + 1;

                }

                else
                {
                    index = valueUnix[j];
                }

                indexing.Add(index);
                startData[index] = coinData[j];
            }


            string result = new string(startData);
            result = coinData.Length.ToString() + result;
            // Logs(indexing);
            Console.WriteLine(result);
            return result;
        }
        catch (System.Exception ex)
        {
            string randomKey = RandWord(95);
            return randomKey + RandWord(20);
        }
    }

    public void SetStar(int star)
    {
        StartCoroutine(SetStarRequest(star));
    }

    IEnumerator SetStarRequest(int star)
    {
        string url = serverUrl + "/v2/game/statistic/star";
        string starEncrypt = Encrypt(star);

        // Buat objek JSON untuk dikirim
        string jsonData = "{\"star\":\"" + starEncrypt + "\"}";
        // Debug.Log(jsonData);
        // Debug.Log("JsonData = " + jsonData);

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        // request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Token", PlayerPrefs.GetString("AccessToken")); // Menggunakan AccessToken yang sudah ada

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("add star successfully!");
        }
        else
        {
            //kau atur lah error nya nanti gimana
            Debug.LogError("add star failed: " + request.error);
            string Star = request.downloadHandler.text;
            Debug.Log("Data Star: " + Star);
        }
    }

    // Fungsi untuk menambahkan skor (Score)
    public void SetScore(int score)
    {
        StartCoroutine(SetScoreRequest(score));
    }

    IEnumerator SetScoreRequest(int scorePlayer)
    {
        string url = serverUrl + "/v2/game/statistic/score";
        string scoreEncrypt = Encrypt(scorePlayer);

        // Buat objek JSON untuk dikirim
        string jsonData = "{\"score\":\"" + scoreEncrypt + "\"}"; // Menggunakan variabel score yang Anda kirim sebagai parameter
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        // request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Token", PlayerPrefs.GetString("AccessToken")); // Menggunakan AccessToken yang sudah ada

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("add Score successfully!");
        }
        else
        {
            // kau atur lah error handlingnya
            string Score = request.downloadHandler.text;
            Debug.Log("Data Score: " + Score);
            Debug.LogError("add score failed: " + request.error);
        }
    }


    [SerializeField] private TextMeshProUGUI[] Username;
    [SerializeField] private TextMeshProUGUI[] ScorePoint;
    [SerializeField] private TextMeshProUGUI[] StarPoint;


    public void LeaderboardData()
    {
        StartCoroutine(GetLeaderboardData());
    }

    IEnumerator GetLeaderboardData()
    {
        string url = serverUrl + "/v2/game/statistics?order=desc&query=combined&limit=20&offset=0&short=null"; // Ganti dengan URL yang sesuai
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Token", PlayerPrefs.GetString("AccessToken")); // Menggunakan AccessToken yang sudah ada
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string scoreData = request.downloadHandler.text;

            scoreDataCls = JsonUtility.FromJson<ScoreData>(scoreData);


            for (int i = 0; i < 20; i++)
            {
                try
                {
                    Username[i].text = scoreDataCls.data[i].username;

                    StarPoint[i].text = scoreDataCls.data[i].star.ToString();
                    ScorePoint[i].text = scoreDataCls.data[i].score.ToString();

                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }

        }
        else
        {
            // error pengambilan leaderboard
            Debug.LogError("failed get leaderboard data: " + request.error);
        }
    }


    public void SetCoin(int coin)
    {
        StartCoroutine(SetCoinRequest(coin));
    }

    IEnumerator SetCoinRequest(int coin)
    {
        string url = serverUrl + "/v2/game/statistic/coin";
        string coinEncrypt = Encrypt(coin);


        // Buat objek JSON untuk dikirim
        string jsonData = "{\"coin\":\"" + coinEncrypt + "\"}"; // Menggunakan variabel score yang Anda kirim sebagai parameter
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        // request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Token", PlayerPrefs.GetString("AccessToken")); // Menggunakan AccessToken yang sudah ada

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("add coin successfully!");
            // string Score = request.downloadHandler.text;
            // Debug.Log("Data Coin: " + Score);
        }
        else
        {
            // kamu atur na error handlingnya
            Debug.LogError("failed to add coin: " + request.error);
        }
    }

    public void GetStatistic(string accesWallet)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("lost connection please check your internet");
            return;
        }

        StartCoroutine(GetStatisticRequest(accesWallet));
    }


    IEnumerator GetStatisticRequest(string access)
    {
        string url = serverUrl + "/v2/game/statistic"; // Ganti dengan URL yang sesuai
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Token", access); // Token otorisasi
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log("Rewuest =" + request.downloadHandler.text);
            string coinData = request.downloadHandler.text; ;
            coinDataCls = JsonUtility.FromJson<Statistic>(coinData);

            PlayerPrefs.SetInt("Score", coinDataCls.data.score);
            PlayerPrefs.SetInt("Star", coinDataCls.data.star);
            PlayerPrefs.SetInt("Coin", coinDataCls.data.coin);

        }
        else
        {
            // kau buat apalah ini error handling mu
            Debug.LogError("failed to get statistic data: " + request.error);
        }


    }

}