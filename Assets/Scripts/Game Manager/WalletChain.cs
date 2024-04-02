using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class WalletChain : MonoBehaviour
{
    [HideInInspector] public static bool isConnectedWallet { get; private set; } = false;
    public static WalletChain Instance;
    public Image walletIcon;
    public GameObject disconnectButton;
    public GameObject refreshButton;
    public WalletAdressData myDatawallet;

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
        while (true)
        {
            string url = SaveManager.Instance.serverUrl + "/wallets/";
            string getToken = SaveManager.Instance.playerData.accessTokenResponse.data.access_token;
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", getToken);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string walletData = request.downloadHandler.text;
                Debug.Log("Wallet data: " + walletData);
                myDatawallet = JsonUtility.FromJson<WalletAdressData>(walletData);


                // Format the URL with the token and open it
                // string formattedUrl = "https://api.garudaverse.io?token=" + getToken;
                SaveManager.Instance.playerData.WalletData = myDatawallet;
                Debug.Log("Address = " + SaveManager.Instance.playerData.WalletData.data.address);
                // Debug.Log(formattedUrl);


                if (myDatawallet.data.address != "")
                {
                    isConnectedWallet = true;
                    SaveManager.Instance.isWalletConnected = true;
                    disconnectButton.gameObject.SetActive(true);
                    refreshButton.gameObject.SetActive(true );
                    walletIcon.color = Color.white;
                    setWalletInformation();
                    StartCoroutine(getNFT());

                    // ---------------------------jika sudah sukses untuk connect wallet---------------------------
                    //------------------------------------------------------------------------------------------
                    break;
                }
                else
                {
                    isConnectedWallet = true;
                    disconnectButton.gameObject.SetActive(true);
                    refreshButton.gameObject.SetActive(true);
                    walletIcon.color = Color.white;
                    StartCoroutine(getNFT());
                    setWalletInformation();
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

    public void getNFTp()
    {
        StartCoroutine(getNFT());
    }


    public IEnumerator GetWallet(bool openbrowser = true)
    {
        string url = SaveManager.Instance.serverUrl + "/wallets/";
        string getToken = SaveManager.Instance.playerData.accessTokenResponse.data.access_token;
        LoadingAnimation.Instance.toggleLoading();
        Debug.Log(getToken);
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", getToken);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string walletData = request.downloadHandler.text;
            Debug.Log("Wallet data: " + walletData);
            WalletAdressData myDatawallet = JsonUtility.FromJson<WalletAdressData>(walletData);

            string address = myDatawallet.data.address;
            Debug.Log(address);



            // Format the URL with the token and open it
            string formattedUrl = "https://dev-fe.garudaverse.io?token=" + getToken + "&serverId=3";
            SaveManager.Instance.playerData.WalletData = myDatawallet;
            Debug.Log("Adress = " + myDatawallet.data.address);
            Debug.Log(formattedUrl);

            if (openbrowser)
            {
                Application.OpenURL(formattedUrl);
            }
            if (myDatawallet.data.address != "" && myDatawallet.data.request_disconnected == false || myDatawallet.data.request_disconnected == true && myDatawallet.data.request_disconnected == false)
            {
                setWalletInformation();
                Debug.Log("Sukses Connect");
                isConnectedWallet = true;
                walletIcon.color = Color.white;
                PopUpInformationhandler.Instance.pop("Wallet Connected");
                StartCoroutine(getNFT());
                SaveManager.Instance.isWalletConnected = true;
                disconnectButton.gameObject.SetActive(true);
                refreshButton.gameObject.SetActive(true);
                LoadingAnimation.Instance.stopLoading();
                //=====================================jika connect wallet berhasil na=======================
                //================================================================================================
            }
            else
            {
                isConnectedWallet = true;
                disconnectButton.gameObject.SetActive(true);
                refreshButton.gameObject.SetActive(true);
                //Manggil dev-api
                if (openbrowser)
                {
                    StartCoroutine(LoopGetWallet());
                    Invoke("TimeOut", 300);
                }
                LoadingAnimation.Instance.stopLoading();
            }

        }
        else
        {
            // urus na error handling mu di sini
            PopUpInformationhandler.Instance.pop("Please Try Again");
            Debug.LogError("Get wallet data failed: " + request.error);

        }
    }

    public void DisconnectP()
    {
        StartCoroutine(Disconnect());
    }

    IEnumerator Disconnect()
    {
        disconnectButton.GetComponent<Button>().interactable = false;
        LoadingAnimation.Instance.toggleLoading();
        string url = SaveManager.Instance.serverUrl + "/wallets/disconnect";
        string getToken = SaveManager.Instance.playerData.accessTokenResponse.data.access_token;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Authorization", getToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string walletData = request.downloadHandler.text;
            Debug.Log("disconnect....");
            Debug.Log("Wallet data: " + walletData);
            isConnectedWallet = false;
            SaveManager.Instance.isWalletConnected = false;
            disconnectButton.gameObject.SetActive(false);
            refreshButton.gameObject.SetActive(false);
            walletIcon.color = Color.red;
            walletText.text = "Wallet Disconnected";
            disconnectButton.GetComponent<Button>().interactable = true;
            PopUpInformationhandler.Instance.pop("Wallet Disconnected");
            SaveManager.Instance.playerData.characterInfo.OwnedCharacters.Clear();
            CharacterManager.Instance.ClearOwnedCharacters();
            LoadingAnimation.Instance.stopLoading();
            //======================jika ingin disconnect lalu isi perintah ketika setelah disconnect
            // sceneChange.LoadScene(0);
        }
        else
        {
            disconnectButton.gameObject.SetActive(true);
            refreshButton.gameObject.SetActive(false);
            // urus sendiri na error handling mu
            LoadingAnimation.Instance.stopLoading();
            Debug.LogError("disconnect data failed: " + request.error);

        }
    }

    private void setRequestDisconnect()
    {
        string url = SaveManager.Instance.serverUrl + "/wallets/";
    }

    IEnumerator getNFT()
    {
        string addressWallet = SaveManager.Instance.playerData.WalletData.data.address;
        LoadingAnimation.Instance.toggleLoading();
        Debug.Log(addressWallet);

        string requestBody = "{\"contractAddress\":\"terra1j7h8v7sdppru5gl67y05h2jvh5xa0g9rmylfs8vf7xaa8l8anwxqmh0aew\",\"walletAddress\":\"" + addressWallet + "\"}";
        Debug.Log(requestBody);

        UnityWebRequest request = new UnityWebRequest("https://api.garudaverse.io/check-list-nft", "POST");

        request.SetRequestHeader("Content-Type", "application/json");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            LoadingAnimation.Instance.stopLoading();
            Debug.Log($"{jsonResponse}");
            HandleResponse(jsonResponse);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            LoadingAnimation.Instance.stopLoading();
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
    private void OnEnable()
    {
        if (SaveManager.Instance.isWalletConnected)
        {
            setWalletInformation();
        }
    }

    private void Start()
    {
        if (isConnectedWallet && walletIcon != null)
        {
            walletIcon.color = Color.white;
        }
    }

    public TMPro.TextMeshProUGUI walletText;

    private void setWalletInformation()
    {
        string walletString = SaveManager.Instance.playerData.WalletData.data.address;
        string wallet = walletString.Substring(0, 4) + "..." + walletString.Substring(walletString.Length - 4);

        walletText.text = wallet;
    }


    public void refreshAll()
    {
        //AccountForm.Instance.RefreshTokenP();
        getNFTp();
    }
}