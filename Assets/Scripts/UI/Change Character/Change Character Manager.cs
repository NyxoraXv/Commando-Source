using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeTai.TrueShadow;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeCharacterManager : MonoBehaviour
{
    [Header("Text")]
    public TMPro.TextMeshProUGUI txt_name, txt_level, txt_damage, txt_hp, txt_agility, price;

    [Header("Image")]
    public Image Avatar;

    [Header("Animator")]
    private Animator animator;

    [Header("Highlighter")]
    public GameObject Highlighter;

    [Header("Equip and Upgrade Button")]
    public Button EquipButton;
    public Button UpgradeButton;

    [Header("Stat Container and stat Canvas Group")]
    public RectTransform Container;
    public RectTransform ContainerShadow;
    public CanvasGroup StatCanvasGroup;

    private Outline PreviousOutline;
    private TrueShadow PreviousCardShadow;

    [HideInInspector]
    public EnumCard selectedCard;

    private void Awake()
    {
        animator = Avatar.gameObject.GetComponent<Animator>();
    }

    public void onClick(GameObject card)
    {
        selectedCard = card.GetComponent<EnumCard>();
        Character associatedCharacter = selectedCard.Character;

        if (associatedCharacter != null)
        {
            GameObject CharacterPrefab = CharacterManager.Instance.GetCharacterPrefab(associatedCharacter);
            CharacterInformation characterInformation = CharacterPrefab.GetComponent<CharacterInformation>();

            RefreshUI(characterInformation, card);
        }
    }

    public void RefreshUI(CharacterInformation characterInformation, GameObject Container)
    {
        UpdateHighlight(Container);
        PlayHighlightAnimation(Container);
        UpdateText(characterInformation);
        UpdateSprite(characterInformation);
        isEquipped();
        
    }

    public void RefreshUIWithoutParameters()
    {
        if (selectedCard != null && selectedCard.Character != null)
        {
            GameObject CharacterPrefab = CharacterManager.Instance.GetCharacterPrefab(selectedCard.Character);
            CharacterInformation characterInformation = CharacterPrefab.GetComponent<CharacterInformation>();

            RefreshUI(characterInformation, selectedCard.gameObject);
        }
    }


    private void UpdateHighlight(GameObject Container)
    {
        var cardOutline = Container.GetComponent<Outline>();
        var cardShadow = Container.GetComponent<TrueShadow>();

        Color NormalOutlineColor = new Color(1f, 1f, 1f, 1f);
        Color NormalCardShadowColor = new Color(1.0f, 1f, 1f, 0.15f);

        Color HighlightColor = new Color(0.639f, 0.996f, 0.937f, 1f);

        // Handle Button Highlight
        if (PreviousCardShadow != null)
        {
            PreviousOutline.effectColor = NormalOutlineColor;
            PreviousCardShadow.Color = NormalCardShadowColor;
        }

        cardOutline.effectColor = HighlightColor;
        cardShadow.Color = HighlightColor;

        if (PreviousCardShadow == null || PreviousCardShadow != cardShadow)
        {
            PreviousOutline = cardOutline;
            PreviousCardShadow = cardShadow;
        }
    }

    private void PlayHighlightAnimation(GameObject Container)
    {
        // Handle Highlighter
        if (!Highlighter.activeSelf)
        {
            Highlighter.SetActive(true);
        }

        // Set the parent of the Highlighter to the same parent as the selected card
        Transform parentTransform = selectedCard.gameObject.transform;
        Highlighter.transform.SetParent(parentTransform);

        // Adjust the local position of the highlighter GameObject
        Highlighter.transform.localPosition = new Vector3(0f, -110f, 0f); // Adjust the values as needed

        Tween HighlightWidthAnimation = Highlighter.GetComponent<RectTransform>()
            .DOSizeDelta(new Vector2(138f, Highlighter.GetComponent<RectTransform>().sizeDelta.y), 0.5f)
            .From(new Vector2(0f, 3f))
            .SetEase(Ease.InOutCubic);

        HighlightWidthAnimation.Play();
    }



    private void UpdateText(CharacterInformation characterInformation)
    {
        var CharacterInformation = characterInformation.Character;
        var Character = CharacterManager.Instance;

        // Handle Text Refresh
        txt_name.text = CharacterInformation.CharacterName.ToString();
        txt_level.text = "Level " + ((Character.GetOwnedCharacterLevel(CharacterInformation.CharacterName)).ToString());

        txt_damage.text = CharacterInformation.Levels[Character.GetOwnedCharacterLevel(CharacterInformation.CharacterName)].Damage.ToString();
        txt_hp.text = CharacterInformation.Levels[Character.GetOwnedCharacterLevel(CharacterInformation.CharacterName)].HP.ToString();
        txt_agility.text = CharacterInformation.Levels[Character.GetOwnedCharacterLevel(CharacterInformation.CharacterName)].Agility.ToString();

        price.text = ("Upgrade Cost: " + CalculateUpgradeCost().ToString());
    }

    private void UpdateSprite(CharacterInformation characterInformation)
    {
        var Character = characterInformation.Character;

        Color initialColor = new Color(1f, 1f, 1f, 0f);
        Avatar.color = initialColor;

        DOTween.Kill(Avatar);

        // Tween the color
        Avatar.DOColor(new Color(1f, 1f, 1f, 1f), 0.5f);

        animator.runtimeAnimatorController = characterInformation.Character.PlayerPreviewController;

        // Append the local position tween
        Avatar.GetComponent<RectTransform>().transform.DOLocalMoveX(-542f, 1f)
             .SetEase(Ease.InOutCubic)
             .From(-591f);
    }

    public bool isEquipped()
    {
        bool isEquipped = CharacterManager.Instance.selectedCharacter == selectedCard.Character;
        string buttonText = isEquipped ? "Equipped" : "Equip";

        EquipButton.interactable = !isEquipped;
        EquipButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buttonText;

        return isEquipped;
    }

    public int CalculateUpgradeCost()
    {
        GameObject CharacterPrefab = CharacterManager.Instance.GetCharacterPrefab(selectedCard.Character);
        CharacterInformation characterInformation = CharacterPrefab.GetComponent<CharacterInformation>();

        return characterInformation.Character.Levels[CharacterManager.Instance.GetOwnedCharacterLevel(characterInformation.Character.CharacterName)].UpgradeCost;
    }

    private void OnEnable()
    {
        // Get all GameObjects that have EnumCard components in children
        EnumCard[] enumCards = GetComponentsInChildren<EnumCard>();

        foreach (EnumCard enumCard in enumCards)
        {
            Character character = enumCard.Character;
            if (character == CharacterManager.Instance.selectedCharacter)
            {
                onClick(enumCard.gameObject);
            }
        }
    }






}
