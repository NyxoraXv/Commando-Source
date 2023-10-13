using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    private ChangeCharacterManager changeCharacterManager;
    private int goldToSpend;
    public TMPro.TextMeshProUGUI price;

    public void onClick()
    {
        changeCharacterManager = GetComponentInParent<ChangeCharacterManager>();
        Debug.Log("changeCharacterManager initialized");

        var characterManager = CharacterManager.Instance;
        Debug.Log("characterManager initialized");

        var SaveManager = saveManager.Instance;
        Debug.Log("saveManager initialized");

        CharacterInformation characterInformation = characterManager.GetCharacterPrefab(SaveManager.playerData.characterInfo.SelectedCharacter).GetComponent<CharacterInformation>();
        Debug.Log("characterInformation initialized");

        

        goldToSpend = changeCharacterManager.CalculateUpgradeCost();
        Debug.Log("goldToSpend calculated : " + goldToSpend);

        if (CurrencyManager.Instance.spendGold(goldToSpend))
        {
            CharacterManager.Instance.UpgradeCharacter(changeCharacterManager.selectedCard.Character);
            Debug.Log("Character Upgraded");
        }
        else
        {
            Debug.LogWarning("Character Upgrade Fail");
        }

        price.text = ("Upgrade Cost: " + goldToSpend);
        changeCharacterManager.RefreshUIWithoutParameters();
    }
}
