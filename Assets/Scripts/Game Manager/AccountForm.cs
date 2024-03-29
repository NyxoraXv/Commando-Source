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
        string url = SaveManager.Instance.serverUrl + "/api/users";
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
            string accountData = request.downloadHandler.text;
            Debug.Log("Account Data: " + accountData);
            Debug.Log("Sign up successful.");
            isSignup = true;
            PopUpInformationhandler.Instance.pop("Signed Up");
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
        string url = SaveManager.Instance.serverUrl + "/api/users/_login";
        LoadingAnimation.Instance.toggleLoading();
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string accountData = request.downloadHandler.text;
            PopUpInformationhandler.Instance.pop("Logged In");
            Debug.Log("Account data: " + accountData);
            Debug.Log("Sign in successful.");
            isLogin = true;
            SaveAccessData(accountData);
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

    /*public void RefreshTokenP()
    {
        StartCoroutine(RefreshToken());
    }

    IEnumerator RefreshToken()
    {
        string url = SaveManager.Instance.serverUrl + "/account/refresh";
        LoadingAnimation.Instance.toggleLoading();
        string refreshToken = SaveManager.Instance.playerData.authResponse.RefreshToken;
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

    }*/



    private void SaveAccessData(string jsonData)
    {
        SaveManager.Instance.playerData.authResponse = JsonUtility.FromJson<UserAuthResponse>(jsonData);
    }

}