using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initiate : MonoBehaviour
{
    public GameObject MainCanvas;
    private void Awake()
    {
        Instantiate(MainCanvas);
    }

    private IEnumerator Refresh()
    {
        yield return new WaitForSeconds(1.0f);

        CurrencyManager.Instance.Refresh();

        CharacterManager.Instance.SwitchCharacter(saveManager.Instance.playerData.characterInfo.SelectedCharacter);
    }
}
