using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class NFTData
{
    public string owner;
    public string token_id;
    public string ask_price;
    public string ask_denom;
    public string name;
    public string image;
    public string video;
    public string rarity;
    public string boost;
    public string level;
    public string collection;
    public string mystery_pack;
}


[System.Serializable]
public class NFTResponse
{
    public bool status;
    public int total;
    public List<NFTData> data;
}

public class WalletChain : MonoBehaviour
{
    [HideInInspector] public static bool isConnectedWallet { get; private set; } = false;
    private string Url = "https://dev-gtp.garudaverse.io/v2";
    public static WalletChain Instance;
    public Image walletIcon;

    private void Awake()
    {
        Instance = this;
    }

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
                    PopUpInformationhandler.Instance.pop("Wallet Connected");
                    isConnectedWallet = true;

                    // ---------------------------jika sudah sukses untuk connect wallet---------------------------
                    //------------------------------------------------------------------------------------------
                    break;
                }
                else
                {
                    isConnectedWallet = false;
                    PopUpInformationhandler.Instance.pop("An Error Occured");
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
                Debug.Log("Sukses Connect");
                isConnectedWallet = true;
                walletIcon.color = Color.white;
                StartCoroutine(getNFT());
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

    IEnumerator getNFT()
    {
        string addressWallet = PlayerPrefs.GetString("AddressWallet");

        string requestBody = "{\"contractAddress\":\"terra1j7h8v7sdppru5gl67y05h2jvh5xa0g9rmylfs8vf7xaa8l8anwxqmh0aew\",\"walletAddress\":\"" + addressWallet + "\"}";

        UnityWebRequest request = new UnityWebRequest("https://api.garudaverse.io/check-list-nft", "POST");

        request.SetRequestHeader("Content-Type", "application/json");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            HandleResponse(jsonResponse);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public int[] convertCharacterID(string numbersString)
    {
        string[] numberStrings = numbersString.Split(',');
        int[] numbersArray = new int[numberStrings.Length];

        for (int i = 0; i < numberStrings.Length; i++)
        {
            if (int.TryParse(numberStrings[i], out int parsedNumber))
            {
                numbersArray[i] = parsedNumber;
            }
            else
            {
                Debug.LogWarning($"Unable to parse '{numberStrings[i]}' as an integer.");
                numbersArray[i] = 0; // For example, set to 0
            }
        }

        return numbersArray;
    }

    public void HandleResponse(string jsonResponse)
    {
        NFTResponse response = JsonUtility.FromJson<NFTResponse>(jsonResponse);

        if (response.status)
        {
            foreach (KeyValuePair<Character, GameObject> kvp in CharacterManager.Instance.characterObjects)
            {
                int[] arrayID = convertCharacterID(kvp.Value.GetComponent<CharacterInformation>().Character.NFT_ID);
                List<int> arrayIDList = arrayID.ToList(); // Convert array to List<int>

                foreach (NFTData nftData in response.data)
                {
                    int tokenId;
                    if (int.TryParse(nftData.token_id, out tokenId)) // Attempt to parse token_id to int
                    {
                        Debug.Log(tokenId);
                        if (arrayIDList.Contains(tokenId) || arrayIDList.SequenceEqual(new List<int> { 0 }))
                        {
                            Debug.Log(kvp.Key);
                            CharacterManager.Instance.AddOwnedCharacter(kvp.Key);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Unable to parse token_id: {nftData.token_id}");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Error: Response status is false");
        }
    }

    private void Start()
    {
        if (isConnectedWallet && walletIcon != null)
        {
            walletIcon.color = Color.white;
        }
    }


}