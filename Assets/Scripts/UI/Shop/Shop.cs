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
        // Clear existing buttons
        foreach (Transform child in buttonParent.transform)
        {
            Destroy(child.gameObject);
        }

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

            // Set button image as avatar preview
            var characterData = characterManager.GetCharacterPrefab(character).GetComponent<CharacterInformation>().Character;
            buttonInstance.GetComponent<Image>().sprite = characterData.FullAvatar;

            // Center the button with the parent width
            RectTransform buttonRectTransform = buttonInstance.GetComponent<RectTransform>();
            buttonRectTransform.sizeDelta = new Vector2(buttonParent.GetComponent<RectTransform>().rect.width, buttonRectTransform.sizeDelta.y);

            // Highlight the button by changing its opacity
            Color normalColor = buttonInstance.image.color;
            buttonInstance.image.color = new Color(normalColor.r, normalColor.g, normalColor.b, IsCharacterOwned(character) ? 0.5f : 1f);
        }
    }

    private void RefreshUI()
    {
        if (current != null)
        {
            var characterData = characterManager.GetCharacterPrefab(current).GetComponent<CharacterInformation>().Character;
            previewImage.sprite = characterData.FullAvatar;
            nameText.text = characterData.CharacterName.ToString();
            priceText.text = characterData.Price.ToString();
        }

        // Recreate buttons
        CreateButtons();
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
        if (CurrencyManager.Instance.SpendFRG(CharacterManager.Instance.GetCharacterPrefab(current).GetComponent<CharacterInformation>().Character.Price))
        {
            CharacterManager.Instance.AddOwnedCharacter(current);
            RefreshUI();
        }
        else
        {
            Debug.Log("idk, doesn't work");
        }
    }
}
