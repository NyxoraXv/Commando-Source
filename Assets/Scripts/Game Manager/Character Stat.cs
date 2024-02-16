using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class CharacterInformation : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        [Header("Name")]
        public Character CharacterName;

        [System.Serializable]
        public class LevelData
        {
            public int Damage;
            public int HP;
            public float Agility;
            public float UpgradeCost;
        }

        public List<LevelData> Levels = new List<LevelData>();

        [Header("Level")]
        public int MaxLevel;

        [Header("Image")]
        public Sprite FullAvatar;
        public Sprite MaskedAvatar;
        public Sprite CroppedAvatar;

        [Header("Price")]
        public int Price;

        [Header("Character Player")]
        public RuntimeAnimatorController PlayerController;
        public RuntimeAnimatorController PlayerPreviewController;
        public RuntimeAnimatorController Weapon;
        public Vector3 HandPivotIdle;
        public Vector3 HandPivotRun;

    }

    public new CharacterData Character;

    private void Start()
    {
        Character.MaxLevel = Character.Levels.Count;

        if (SaveManager.Instance != null)
        {
            // Iterate through the owned characters to find the one with the matching name
            foreach (var kvp in SaveManager.Instance.playerData.characterInfo.OwnedCharacters)
            {
                Character ownedCharacter = kvp.Key; // Access the character enum directly
                int characterLevel = kvp.Value; // Access the level

                if (ownedCharacter.ToString() == Character.CharacterName.ToString())
                {
                    // Use 'ownedCharacter' and 'characterLevel' as needed.
                }
            }
        }
    }
}
