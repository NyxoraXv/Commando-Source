using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    private ChangeCharacterManager changeCharacterManager;
    private float FRGToSpend;
    public TMPro.TextMeshProUGUI price;

    public void onClick()
    {
        changeCharacterManager = GetComponentInParent<ChangeCharacterManager>();
        Debug.Log("changeCharacterManager initialized");

        var characterManager = CharacterManager.Instance;
        Debug.Log("characterManager initialized");

        var SaveManager = global::SaveManager.Instance;
        Debug.Log("saveManager initialized");

        CharacterInformation characterInformation = characterManager.GetCharacterPrefab(SaveManager.playerData.characterInfo.SelectedCharacter).GetComponent<CharacterInformation>();
        Debug.Log("characterInformation initialized");

        

        FRGToSpend = changeCharacterManager.CalculateUpgradeCost();
        Debug.Log("goldToSpend calculated : " + FRGToSpend);

        if (CurrencyManager.Instance.SpendFRG(FRGToSpend))
        {
            CharacterManager.Instance.UpgradeCharacter(changeCharacterManager.selectedCard.Character);
            Debug.Log("Character Upgraded");
        }
        else
        {
            CurrencyManager.Instance.insufficientFund(SaveManager.Instance.playerData.statistic.data.frg - FRGToSpend,
                                                    GameObject.FindWithTag("Main Menu Parent").transform,
                                                    PopUpInstantiate.CurrencyType.LUNC);

            Debug.LogWarning("Character Upgrade Fail");
        }

        price.text = ("Boost Cost: " + FRGToSpend);
        changeCharacterManager.RefreshUIWithoutParameters();
    }
}
