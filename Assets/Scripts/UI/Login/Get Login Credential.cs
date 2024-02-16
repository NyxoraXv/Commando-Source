using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLoginCredential : MonoBehaviour
{
    public TMPro.TMP_InputField nameText;
    public TMPro.TMP_InputField emailText;
    public TMPro.TMP_InputField passwordText;
    public GameObject Login;
    public GameObject mainMenu;


    // Edit this whatever you like!
    public void setCredential()
    {
        SaveManager.Instance.isLogin = true;
        SaveManager.Instance.Verify(nameText.text, passwordText.text, emailText.text, true);
        StartCoroutine(waitScene());
        

    }

    IEnumerator waitScene()
    {
        while (!AccountForm.Instance.isLogin)
        {
            yield return null;
        }
        mainMenu.SetActive(true);
        Login.SetActive(false);
    }
}
