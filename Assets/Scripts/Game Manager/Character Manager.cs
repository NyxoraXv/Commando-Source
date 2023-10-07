using System;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    Sucipto,
    Habibi,
    Levia,
    Nurul
}

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    [SerializeField]
    public Character selectedCharacter = Character.Sucipto;

    [SerializeField] private Dictionary<Character, GameObject> characterObjects = new Dictionary<Character, GameObject>();

    // Dictionary to store owned characters and their levels
    public Dictionary<Character, int> ownedCharacters = new Dictionary<Character, int>();
    public int MaxLevel;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private GameObject sucipto, habibi, nurul, levia;

    private void Start()
    {
        InitializeCharacterDictionary();
        SwitchCharacter(selectedCharacter);
    }

    private void InitializeCharacterDictionary()
    {
        characterObjects[Character.Sucipto] = sucipto;
        characterObjects[Character.Habibi] = habibi;
        characterObjects[Character.Levia] = levia;
        characterObjects[Character.Nurul] = nurul;
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

            CharacterInformation charData = GetCharacterPrefab(newCharacter).GetComponent<CharacterInformation>();

            // Access the character data as needed, for example:
            var imageComponent = charData.Character.MaskedAvatar;
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
    public void AddOwnedCharacter(Character character, int level)
    {
        if (!ownedCharacters.ContainsKey(character))
        {
            ownedCharacters.Add(character, level);
        }
        else
        {
            ownedCharacters[character] = level;
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
