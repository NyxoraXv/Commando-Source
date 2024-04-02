using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using System;

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
            public int RateOfFire;
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

        [Header("NFT ID")]
        public string NFT_ID;

        [Header("Damager Object")]
        public GameObject BulletPrefab;
        public GameObject KnifeEffectPrefab;

    }

    public CharacterData Character;

    private void Start()
    {
        Character.MaxLevel = Character.Levels.Count;

        if (SaveManager.Instance != null)
        {
            foreach (var kvp in SaveManager.Instance.playerData.characterInfo.OwnedCharacters)
            {
                Character ownedCharacter = kvp.Key;
                int characterLevel = kvp.Value;

                if (ownedCharacter.ToString() == Character.CharacterName.ToString())
                {
                }
            }
        }
    }
}
