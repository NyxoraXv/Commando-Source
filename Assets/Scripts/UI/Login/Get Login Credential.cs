using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLoginCredential : MonoBehaviour
{
    private TMPro.TextMeshProUGUI nameText;


    // Edit this whatever you like!
    public void setCredential()
    {
        nameText = GetComponent<TMPro.TextMeshProUGUI>();
        SaveManager.Instance.isLogin = true;
            SaveManager.Instance.username = nameText.text;
            SaveManager.Instance.Verify();

    }
}
