using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setnameprofile : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;

    private void OnEnable()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        text.text = SaveManager.Instance.username;
    }
}
