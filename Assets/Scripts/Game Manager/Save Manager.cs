using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections;

public class SaveManager : MonoBehaviour
{

    public class PlayerData
    {
        public class PlayerInfo
        {
            public string PlayerName { get; set; }
            public int PlayerLevel { get; set; }    
            public int PlayerXP { get; set; }
            public int PlayerLastLevel { get; set; }
            public int PlayerScore { get; set; }
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

    public string username { get; set; }
    public int score { get; set; }

    public bool isLogin;

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

    public void InitializeNewPlayer(string playerName)
    {
        playerData.playerInformation.PlayerName = playerName;

        playerData.playerInformation.PlayerLevel = 1;
        playerData.playerInformation.PlayerXP = 0;
        playerData.currencyInfo.PlayerLUNC = 0f;
        playerData.currencyInfo.PlayerFRG = 0f;
        playerData.playerInformation.PlayerScore = 0;
        playerData.playerInformation.PlayerScore = 0;
        playerData.playerInformation.PlayerLastLevel = 0;

        playerData.characterInfo.SelectedCharacter = Character.Sucipto;

        // Initialize the OwnedCharacters dictionary with the starting character and level
        playerData.characterInfo.OwnedCharacters.Clear();
        playerData.characterInfo.OwnedCharacters.Add(Character.Sucipto, 1);

        SetUpDataLoad();

        Save();
    }

    private bool run;
    private void SetUpDataLoad()
    { 
        LeaderboardGameSystem.Instance.RefreshData();
        CurrencyManager.Instance.CurrentFRG = PlayerPrefs.GetInt("Coin");
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
            StartCoroutine(waitSignupScene(Username));
            InitializeNewPlayer(Username);
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

    IEnumerator waitSignupScene(string user)
    {
        while (!AccountForm.Instance.isLogin)
        {
            yield return null;
        }
        InitializeNewPlayer(user);
    }

    private bool Load()
    {
        string savePath = Path.Combine(Application.persistentDataPath, $"{username}player_data.json");

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                Debug.Log("Player Data JSON After Loading:\n" + json);
                playerData = JsonConvert.DeserializeObject<PlayerData>(json); // Deserialize using JsonConvert
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading save data: {e}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("No save data found");
            return false;
        }
    }

    public void Save()
    {
        Debug.Log("Save");
        LeaderboardGameSystem.Instance.SetCoin(int.Parse((CurrencyManager.Instance.CurrentFRG * 1000).ToString()));
        Debug.Log(int.Parse((CurrencyManager.Instance.CurrentFRG * 1000).ToString()));
        LeaderboardGameSystem.Instance.SetScore(playerData.playerInformation.PlayerScore);
        Debug.Log(playerData.playerInformation.PlayerScore);

    }


    private void OnApplicationQuit()
    {
        Save();
    }
}
