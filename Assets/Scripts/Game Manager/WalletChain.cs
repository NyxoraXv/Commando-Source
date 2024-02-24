using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;



public class WalletChain : MonoBehaviour
{
    [HideInInspector] public static bool isConnectedWallet { get; private set; } = false;
    private string Url = "https://dev-gtp.garudaverse.io/v2";

    public void GetWalletP()
    {

        StartCoroutine(GetWallet());

    }


    public IEnumerator LoopGetWallet()
    {
        Invoke("TimeOut", 300);
        //UI ketika loading muncul--------------------------------------------------------

        while (true)
        {
            string url = Url + "/account/wallet";
            string getToken = PlayerPrefs.GetString("AccessToken", "");
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Token", getToken);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string walletData = request.downloadHandler.text;
                Debug.Log("Wallet data: " + walletData);
                WalletAdress myDatawallet = JsonUtility.FromJson<WalletAdress>(walletData);


                // Format the URL with the token and open it
                // string formattedUrl = "https://dev-fe.garudaverse.io?token=" + getToken;
                PlayerPrefs.SetString("AddressWallet", myDatawallet.data.address_wallet);
                Debug.Log("Adress = " + myDatawallet.data.address_wallet);
                // Debug.Log(formattedUrl);


                if (myDatawallet.data.address_wallet != "" && myDatawallet.data.request_disconnect == false || myDatawallet.data.is_connected == true && myDatawallet.data.request_disconnect == false)
                {
                    Debug.Log("Sukses login");
                    isConnectedWallet = true;

                    // ---------------------------jika sudah sukses untuk connect wallet---------------------------
                    //------------------------------------------------------------------------------------------
                    break;
                }
                else
                {
                    isConnectedWallet = false;
                    Debug.Log("IsConnected wallet gagal");
                }
            }
            else
            {
                Debug.LogError("Get wallet data failed: " + request.error);

            }
            yield return new WaitForSeconds(3f);
        }
    }

    private void TimeOut()
    {
        StopAllCoroutines();

        // code untuk mengubah ke connect wallet----------------------
        // loginForm.OnWalletFailed();   
        // menghilangkan UI loading------------------------------------

    }

    public void TimeOutP()
    {
        Debug.Log("Time Out");
        StopAllCoroutines();

        /////===================ketika gagal connect wallet==================  
    }



    public IEnumerator GetWallet(bool openbrowser = true)
    {
        string url = Url + "/account/wallet";
        string getToken = PlayerPrefs.GetString("AccessToken", "");
        Debug.Log(getToken);
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Token", getToken);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string walletData = request.downloadHandler.text;
            Debug.Log("Wallet data: " + walletData);
            WalletAdress myDatawallet = JsonUtility.FromJson<WalletAdress>(walletData);

            // Format the URL with the token and open it
            string formattedUrl = "https://dev-fe.garudaverse.io?token=" + getToken + "&serverId=2";
            PlayerPrefs.SetString("AddressWallet", myDatawallet.data.address_wallet);
            Debug.Log("Adress = " + myDatawallet.data.address_wallet);
            Debug.Log(formattedUrl);
            if (openbrowser)
            {
                Application.OpenURL(formattedUrl);
            }


            if (myDatawallet.data.address_wallet != "" && myDatawallet.data.request_disconnect == false || myDatawallet.data.is_connected == true && myDatawallet.data.request_disconnect == false)
            {
                Debug.Log("Sukses login");
                isConnectedWallet = true;

                //=====================================jika connect wallet berhasil na=======================
                //================================================================================================
            }
            else
            {
                isConnectedWallet = false;
                //Manggil dev-api
                if (openbrowser)
                {
                    StartCoroutine(LoopGetWallet());
                    Invoke("TimeOut", 300);
                }
            }



        }
        else
        {
            // urus na error handling mu di sini
            Debug.LogError("Get wallet data failed: " + request.error);

        }
    }

    public void DisconnectP()
    {
        StartCoroutine(Disconnect());
    }

    IEnumerator Disconnect()
    {
        string url = Url + "/account/wallet/disconnect";
        string getToken = PlayerPrefs.GetString("AccessToken", "");
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Token", getToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string walletData = request.downloadHandler.text;
            Debug.Log("disconnect....");
            Debug.Log("Wallet data: " + walletData);
            isConnectedWallet = false;
            //======================jika ingin disconnect lalu isi perintah ketika setelah disconnect
            // sceneChange.LoadScene(0);
        }
        else
        {
            // urus sendiri na error handling mu
            Debug.LogError("disconnect data failed: " + request.error);

        }
    }

}