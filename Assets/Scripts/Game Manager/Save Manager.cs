using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class saveManager : MonoBehaviour
{

    public class PlayerData
    {
        public class PlayerInfo
        {
            public string PlayerName { get; set; }
            public int PlayerLevel { get; set; }    
            public int PlayerXP { get; set; }
        }

        public class CurrencyInfo
        {
            public int PlayerGold { get; set; }
            public int PlayerDiamond { get; set; }
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

        public PlayerInfo playerInformation { get; set; } = new PlayerInfo();
        public CurrencyInfo currencyInfo { get; set; } = new CurrencyInfo();
        public CharacterInfo characterInfo { get; set; } = new CharacterInfo();
    }


    public static saveManager Instance;

    public PlayerData playerData;

    public string username { get; set; }

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
        playerData.currencyInfo.PlayerGold = 300;
        playerData.currencyInfo.PlayerDiamond = 10;

        playerData.characterInfo.SelectedCharacter = Character.Sucipto;

        // Initialize the OwnedCharacters dictionary with the starting character and level
        playerData.characterInfo.OwnedCharacters.Clear();
        playerData.characterInfo.OwnedCharacters.Add(Character.Sucipto, 1);

        SetUpDataLoad();

        Save();
    }

    private void SetUpDataSave()
    {
        playerData.playerInformation.PlayerName = username;

        // Save playerData values
        playerData.playerInformation.PlayerLevel = LevelManager.Instance.currentLevel;
        playerData.playerInformation.PlayerXP = LevelManager.Instance.CurrentXP;

        // Currency Manager
        playerData.currencyInfo.PlayerGold = CurrencyManager.Instance.CurrentGold;
        playerData.currencyInfo.PlayerDiamond = CurrencyManager.Instance.CurrentDiamond;

        // Character Manager
        playerData.characterInfo.SelectedCharacter = CharacterManager.Instance.selectedCharacter;

        // Save owned characters and their levels
        playerData.characterInfo.OwnedCharacters.Clear(); // Clear the existing data

        // Iterate through the owned characters in CharacterManager and add them to playerData
        foreach (var kvp in CharacterManager.Instance.ownedCharacters)
        {
            playerData.characterInfo.OwnedCharacters.Add(kvp.Key, kvp.Value);
        }

        Debug.Log("Set up data save" + username);
    }


    private void SetUpDataLoad()
    {
        // Load player data values
        LevelManager.Instance.currentLevel = playerData.playerInformation.PlayerLevel;
        LevelManager.Instance.CurrentXP = playerData.playerInformation.PlayerXP;

        // Load currency data
        CurrencyManager.Instance.CurrentGold = playerData.currencyInfo.PlayerGold;
        CurrencyManager.Instance.CurrentDiamond = playerData.currencyInfo.PlayerDiamond;

        // Load selected character data
        CharacterManager.Instance.selectedCharacter = playerData.characterInfo.SelectedCharacter;
        foreach (var kvp in playerData.characterInfo.OwnedCharacters)
        {
            Character character = kvp.Key; // Access the character enum directly
            int characterLevel = kvp.Value; // Access the level

            // Add the owned character to the CharacterManager
            CharacterManager.Instance.AddOwnedCharacter(character, characterLevel);
        }


        Debug.Log("Set up data load" + username);
    }

    public bool Verify()
    {
        if (Load())
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
        }
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
        Debug.Log("Save" + username);

        // Populate playerData with the data you want to save
        SetUpDataSave();

        string savePath = Path.Combine(Application.persistentDataPath, $"{username}player_data.json");
        string json = JsonConvert.SerializeObject(playerData); // Serialize using JsonConvert
        File.WriteAllText(savePath, json);
        Debug.Log("Saved player data to: " + savePath);
    }


    private void OnApplicationQuit()
    {
        Save();
    }
}
