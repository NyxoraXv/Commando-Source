using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

[Serializable]
public class WalletData
{
    public WalletDetails data;
    public object errors;
    public string message;
    public int code;
}
[Serializable]
public class WalletAdress
{
    public WalletAdressDetails data;
    public object errors;
    public string message;
    public int code;
}

[Serializable]
public class WalletDetails
{
    public string access_token;
    public string refresh_token;
}
[Serializable]
public class WalletAdressDetails
{
    public string id;
    public string user_id;
    public string address_wallet;
    public string balance;
    public string user_agent;
    public bool is_connected;
    public bool request_disconnect;
    public string UpdatedAt;
    public string CreatedAt;
    public string DeletedAt;
    public int score;
    public int star;
    public int coin;
    public int last_level;
}



public class AccountForm : MonoBehaviour
{
    // [SerializeField] private LoginForm loginForm;
    // [SerializeField] private WalletChain walletChain;
    [HideInInspector] public const string BASEURL = "https://dev-gtp.garudaverse.io/v2";

    private WalletDetails walletDetails;
    public static string accesWallet;
    public static AccountForm Instance;
    public bool isLogin = false;

    private void Awake()
    {
        accesWallet = GetData(1);
    }
    private void Start()
    {
        //walletChain.DisconnectP();
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

    public void SignUpP(string json)
    {
        StartCoroutine(SignUp(json));
        Debug.Log("SignUp");
    }

    IEnumerator SignUp(string jsonData)
    {
        Debug.Log("SignUp");
        string url = BASEURL + "/account/signup";


        // Menyiapkan permintaan POST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        // request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        // Memeriksa hasil permintaan dan melakukan tindakan berdasarkan hasilnya
        if (request.result == UnityWebRequest.Result.Success)
        {
            string walletData = request.downloadHandler.text;
            Debug.Log("Wallet data: " + walletData);
            Debug.Log("Sign up successful.");
            //CurrencyManager.Instance.CurrentLUNC = walletData
        }
        else
        {

            Debug.LogError("Sign up failed: " + request.error);
            if (request.error == "Cannot resolve destination host")
            {
                // error ketika internet mati
            }
            else if (request.error == "HTTP/1.1 409 Conflict")
            {
                // error jika email telah dibuat
            }
            else
            {
                // error selain semua di atas
            }
        }
    }
    public void SignInP(string json)
    {
        StartCoroutine(SignIn(json));
    }
    IEnumerator SignIn(string jsonData)
    {
        string url = BASEURL + "/account/signin";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string walletData = request.downloadHandler.text;
            Debug.Log("Wallet data: " + walletData);
            Debug.Log("Sign in successful.");
            isLogin = true;
            SaveAccessData(walletData);
            accesWallet = GetData(1);

            // jika signin berhasil masukkan perintah disini
        }
        else
        {
            Debug.LogError("Sign in failed: " + request.error);
            if (request.error == "Cannot resolve destination host")
            {
                // ketika error internet
            }
            else if (request.error == "HTTP/1.1 401 Unauthorized" || request.error == "HTTP/1.1 404 Not Found")
            {
                // ketika salah input password atau salah input email
            }
            else
            {
                // error selain yang di atas
            }
        }


    }

    //////////////    token harus di refresh karena bisa kadaluarsa, fungsi token untuk akses api

    public void RefreshTokenP()
    {
        StartCoroutine(RefreshToken());
    }


    IEnumerator RefreshToken()
    {
        string url = BASEURL + "/account/refresh";
        string refreshToken = GetData(2);
        refreshToken = refreshToken.Split(" ")[1];
        Debug.Log(refreshToken);
        string jsonData = "{\"refreshToken\": \"" + refreshToken + "\"}";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string walletData = request.downloadHandler.text;
            Debug.Log("Token refreshed.");
            Debug.Log("wallet data" + walletData);
            Debug.Log(request.result);
            SaveAccessData(walletData);
        }
        else
        {
            Debug.LogError("Token refresh failed: " + request.error);

        }

    }



    private void SaveAccessData(string jsonData)
    {
        WalletData myData = JsonUtility.FromJson<WalletData>(jsonData);
        PlayerPrefs.SetString("AccessToken", myData.data.access_token);
        PlayerPrefs.SetString("RefreshToken", myData.data.refresh_token);
    }


    public string GetData(short type)
    {
        // type 1 = return data.accestoken
        // type 2 = return data.refreshtoken

        try
        {

            switch (type)
            {
                case 1:
                    return PlayerPrefs.GetString("AccessToken", "");
                case 2:
                    return PlayerPrefs.GetString("RefreshToken", "");
                default:
                    return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Gagal membaca data: " + e.Message);
        }
        return null;

    }


}