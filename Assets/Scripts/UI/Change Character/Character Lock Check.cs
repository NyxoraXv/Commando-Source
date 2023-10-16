using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLockCheck : MonoBehaviour
{
    private void OnEnable()
    {
        Character card = this.GetComponentInParent<EnumCard>().Character;

        if (SaveManager.Instance.playerData.characterInfo.OwnedCharacters.TryGetValue(card, out int index))
        {
            gameObject.SetActive(false);
        }


    }
}
