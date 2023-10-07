using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarHandler : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<Image>().sprite = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.MaskedAvatar;
    }


}
