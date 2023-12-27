using UnityEngine;
using UnityEngine.UI; // Import the UI namespace

public class ButtonHandler : MonoBehaviour
{
    public void OnButtonClick()
    {
        CurrencyManager.Instance.spendGold(-200);
        LevelManager.Instance.addXP(200);
        //Debug.Log(CharacterManager.Instance.selectedCharacter);
        SaveManager.Instance.Save();

        CharacterManager.Instance.AddOwnedCharacter(Character.Habibi);
    }
}
