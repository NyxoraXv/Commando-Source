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

        try
        {
            saveManager.Instance.username = nameText.text;
            saveManager.Instance.Verify();
        }
        catch
        {
            Debug.LogError("Credential Error");
        }
    }
}
