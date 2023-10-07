using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipButton : MonoBehaviour
{
    private ChangeCharacterManager ChangeCharacterManager;

    public void OnClick()
    {
        ChangeCharacterManager = GetComponentInParent<ChangeCharacterManager>();

        if(CharacterManager.Instance.selectedCharacter != ChangeCharacterManager.selectedCard.Character)
        {
            CharacterManager.Instance.SwitchCharacter(ChangeCharacterManager.selectedCard.Character);
        }

        if (ChangeCharacterManager.isEquipped())
        {
            Debug.Log("Character Equipped");
        }
        
    }

}
