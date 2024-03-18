using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setnameprofile : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;

    private void OnEnable()
    {
        StartCoroutine(waitUsername());
    }

    IEnumerator waitUsername()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();

        while (string.IsNullOrEmpty(SaveManager.Instance.playerData.playerInformation.PlayerName))
        {
            yield return null;
        }

        text.text = SaveManager.Instance.playerData.playerInformation.PlayerName;
    }
}
