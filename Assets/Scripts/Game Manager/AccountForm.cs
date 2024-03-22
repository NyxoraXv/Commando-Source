using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;




public class AccountForm : MonoBehaviour
{
    // [SerializeField] private LoginForm loginForm;
    // [SerializeField] private WalletChain walletChain;
    [HideInInspector] public const string BASEURL = "https://lunc-zombie.garudaverse.io/v2";

    private WalletDetails walletDetails;
    public static string accesWallet;
    public static AccountForm Instance;
    public bool isLogin = false;
    public bool isSignup = false;

    //private void Awake()
    //{
    //    accesWallet = GetData(1);
    //}

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
        LoadingAnimation.Instance.toggleLoading();

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
            isSignup = true;
            PopUpInformationhandler.Instance.pop("Signed Up");
            //CurrencyManager.Instance.CurrentLUNC = walletData
            LoadingAnimation.Instance.stopLoading();
        }
        else
        {
            isSignup = false;
            Debug.LogError("Sign up failed: " + request.error);
            if (request.error == "Cannot resolve destination host")
            {
                PopUpInformationhandler.Instance.pop("Network Error");
            }
            else if (request.error == "HTTP/1.1 409 Conflict")
            {
                PopUpInformationhandler.Instance.pop("Username/Password/Email already used");
            }
            else
            {
                PopUpInformationhandler.Instance.pop("Unknown Error");
            }
            LoadingAnimation.Instance.stopLoading();
        }
    }
    public void SignInP(string json)
    {
        StartCoroutine(SignIn(json));
    }
    IEnumerator SignIn(string jsonData)
    {
        string url = BASEURL + "/account/signin";
        LoadingAnimation.Instance.toggleLoading();
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string walletData = request.downloadHandler.text;
            PopUpInformationhandler.Instance.pop("Logged In");
            Debug.Log("Wallet data: " + walletData);
            Debug.Log("Sign in successful.");
            isLogin = true;
            SaveAccessData(walletData);
            accesWallet = GetData(1);
            LoadingAnimation.Instance.stopLoading();
        }
        else
        {
            Debug.LogError("Sign in failed: " + request.error);
            if (request.error == "Cannot resolve destination host")
            {
                PopUpInformationhandler.Instance.pop("Network Error");
            }
            else if (request.error == "HTTP/1.1 401 Unauthorized" || request.error == "HTTP/1.1 404 Not Found")
            {
                PopUpInformationhandler.Instance.pop("Username/Password/Email wrong");
            }
            else
            {
                PopUpInformationhandler.Instance.pop("Unknown Error");
            }
            LoadingAnimation.Instance.stopLoading();
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
        LoadingAnimation.Instance.toggleLoading();
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
            LoadingAnimation.Instance.stopLoading();
        }
        else
        {
            Debug.LogError("Token refresh failed: " + request.error);
            LoadingAnimation.Instance.stopLoading();
        }

    }



    private void SaveAccessData(string jsonData)
    {
        WalletData myData = JsonUtility.FromJson<WalletData>(jsonData);
        SaveManager.Instance.playerData.playerInformation.accessToken =  myData.data.access_token;
        SaveManager.Instance.playerData.playerInformation.refreshToken =  myData.data.refresh_token;
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
                    return SaveManager.Instance.playerData.playerInformation.accessToken;
                case 2:
                    return SaveManager.Instance.playerData.playerInformation.refreshToken;
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