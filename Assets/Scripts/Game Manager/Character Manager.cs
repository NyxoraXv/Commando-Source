using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Character
{
    Sucipto,
    Habibi,
    Levia,
    Nurul,
    Guan,
    Nana,
    Dylan,
    Unknown
}

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    [SerializeField]
    public Character selectedCharacter;

    [SerializeField] public Dictionary<Character, GameObject> characterObjects = new Dictionary<Character, GameObject>();

    // Dictionary to store owned characters and their levels
    public Dictionary<Character, int> ownedCharacters = new Dictionary<Character, int>();
    public int MaxLevel;

    private void Awake()
    {
        SetupSingleton();
    }

    private void SetupSingleton()
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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method will be called whenever a new scene is loaded
        InitializeCharacterDictionary();
        SwitchCharacter(selectedCharacter);
    }

    private void InitializeCharacterDictionary()
    {
        characterObjects[Character.Sucipto] = sucipto;
        characterObjects[Character.Habibi] = habibi;
        characterObjects[Character.Levia] = levia;
        characterObjects[Character.Nurul] = nurul;
        characterObjects[Character.Guan] = frg;
        characterObjects[Character.Nana] = levi;
        characterObjects[Character.Dylan] = bigboy;
        characterObjects[Character.Unknown] = modelx;
    }

    [SerializeField] private GameObject sucipto, habibi, nurul, levia, frg, levi, bigboy, modelx;

    private void Start()
    {
        // You can perform any scene-independent initialization here if needed
    }

    public GameObject GetCharacterPrefab(Character character)
    {
        return characterObjects[character];
    }

    public void SwitchCharacter(Character newCharacter)
    {
        if (characterObjects.TryGetValue(newCharacter, out var character))
        {
            characterObjects[selectedCharacter].SetActive(false);
            selectedCharacter = newCharacter;
            character.SetActive(true);

            CharacterInformation charData = GetCharacterPrefab(newCharacter)?.GetComponent<CharacterInformation>();

            // Access the character data as needed, for example:
            if (charData != null)
            {
                var imageComponent = charData.Character.MaskedAvatar;
            }
        }
        else
        {
            Debug.LogError("Character not found in dictionary: " + newCharacter.ToString());
        }
    }

    public bool UpgradeCharacter(Character upgradedCharacter)
    {
        CharacterInformation charData = GetCharacterPrefab(upgradedCharacter)?.GetComponent<CharacterInformation>();

        if (charData == null)
        {
            Debug.LogError("Character data not found for upgrading character: " + upgradedCharacter.ToString());
            return false;
        }

        if (ownedCharacters.TryGetValue(upgradedCharacter, out int characterLevel) && characterLevel < (charData.Character.MaxLevel - 1))
        {
            characterLevel++; // Increment the character's level
            ownedCharacters[upgradedCharacter] = characterLevel; // Update the level in the dictionary
            Debug.Log("Upgraded " + upgradedCharacter.ToString() + " to level " + characterLevel);
            return true;
        }
        else if (characterLevel >= (charData.Character.MaxLevel - 1))
        {
            Debug.LogWarning("Character " + upgradedCharacter.ToString() + " is already at the maximum level.");
            return false;
        }

        return false; // Default case if character not found in ownedCharacters
    }

    // Method to add an owned character with a specified level
    public void AddOwnedCharacter(Character character)
    {
        if (!ownedCharacters.ContainsKey(character))
        {
            ownedCharacters.Add(character, 1);
            SaveManager.Instance.Save();
            Debug.Log("Character Added");
        }
    }

    public void AddOwnedCharacterWithLevel(Character character, int level)
    {
        if (!ownedCharacters.ContainsKey(character))
        {
            ownedCharacters.Add(character, level);
            Debug.Log("Character Added");
        }
        else
        {
            ownedCharacters[character] = level;
            Debug.Log("idk");
        }
    }

    // Method to get the level of an owned character
    public int GetOwnedCharacterLevel(Character character)
    {
        if (ownedCharacters.TryGetValue(character, out int level))
        {
            return level;
        }
        return 0; // Return 0 if the character is not found in ownedCharacters
    }
}
