using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    private CharacterManager characterManager;
    private Character current;

    [Header("UI Elements")]
    public Image previewImage;
    public TextMeshProUGUI nameText, priceText;

    [Header("Button Prefab")]
    public Button buttonPrefab;

    [Header("Buttons")]
    public List<Character> characters = new List<Character>();

    [Header("Button Parent")]
    public GameObject buttonParent;

    private void ChangePreview(Character selected)
    {
        var characterData = characterManager.GetCharacterPrefab(selected).GetComponent<CharacterInformation>().Character;
        previewImage.sprite = characterData.FullAvatar;
        nameText.text = characterData.CharacterName.ToString();
        priceText.text = characterData.Price.ToString();
        current = selected;
    }

    private void CreateButtons()
    {
        foreach (var character in characters)
        {
            Button buttonInstance = Instantiate(buttonPrefab, buttonParent.transform);
            buttonInstance.onClick.AddListener(() => SelectCharacter(character));

            // Check if the character is owned
            if (IsCharacterOwned(character))
            {
                buttonInstance.interactable = false; // Disable the button
                Debug.Log($"owned.");
            }

            // Set button text or image as needed
            // You might want to customize this part based on your needs

            // Center the button with the parent width
            RectTransform buttonRectTransform = buttonInstance.GetComponent<RectTransform>();
            buttonRectTransform.sizeDelta = new Vector2(buttonParent.GetComponent<RectTransform>().rect.width, buttonRectTransform.sizeDelta.y);
        }
    }

    private bool IsCharacterOwned(Character character)
    {
        // Check if the character is owned based on your SaveManager or any other logic
        return SaveManager.Instance.playerData.characterInfo.OwnedCharacters.ContainsKey(character);
    }

    public void SelectCharacter(Character selected)
    {
        ChangePreview(selected);
    }

    private void Start()
    {
        characterManager = CharacterManager.Instance;

        if (buttonPrefab == null)
        {
            Debug.LogError("Button Prefab is null. Make sure to assign a Button Prefab in the Inspector.");
            return;
        }

        if (buttonParent == null)
        {
            Debug.LogError("Button Parent is null. Make sure to assign a GameObject as the Button Parent in the Inspector.");
            return;
        }

        CreateButtons();
    }

    public void buy()
    {
        if (CurrencyManager.Instance.spendGold(CharacterManager.Instance.GetCharacterPrefab(current).GetComponent<CharacterInformation>().Character.Price))
        {
            CharacterManager.Instance.AddOwnedCharacter(current);
        }
        else
        {
            Debug.Log("idk, doesnt work");
        }
    }
}
