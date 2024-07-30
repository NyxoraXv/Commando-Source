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

        if (CharacterManager.Instance.UpgradeCharacter(changeCharacterManager.selectedCard.Character))
        {
            if (CurrencyManager.Instance.SpendFRG(FRGToSpend))
            {
                Debug.Log("Character Upgraded");
            }
            else
            {
                CurrencyManager.Instance.insufficientFund(SaveManager.Instance.playerData.statistic.data.frg - FRGToSpend,
                                                        GameObject.FindWithTag("Main Menu Parent").transform,
                                                        PopUpInstantiate.CurrencyType.LUNC);
            }

        }
        else
        {
            SaveManager.Instance.fetchData();
            PopUpInformationhandler.Instance.pop("Max Level Reached");

            Debug.LogWarning("Character Upgrade Fail");
        }

        price.text = ("Boost Cost: " + FRGToSpend);
        changeCharacterManager.RefreshUIWithoutParameters();
        SaveManager.Instance.fetchData();
        CurrencyManager.Instance.Refresh();
    }
}
