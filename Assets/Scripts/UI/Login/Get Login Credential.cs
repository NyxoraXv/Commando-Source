using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLoginCredential : MonoBehaviour
{
    public TMPro.TMP_InputField nameText;
    public TMPro.TMP_InputField emailText;
    public TMPro.TMP_InputField passwordText;
    public GameObject Login, LoginContainer, signUp;
    public GameObject mainMenu;


    // Edit this whatever you like!



    public void setCredential(bool isLogin)
    {
        if(isLogin)
        {
            SaveManager.Instance.isLogin = true;
            SaveManager.Instance.Verify(nameText.text, passwordText.text, emailText.text, isLogin);
            StartCoroutine(waitSceneLogin());
        }
        else
        {
            SaveManager.Instance.Verify(nameText.text, passwordText.text, emailText.text, isLogin);
            StartCoroutine(waitSceneSignup());
        }
        

    }

    IEnumerator waitSceneSignup()
    {
        while (!AccountManager.Instance.isSignup)
        {
            yield return null;
        }
        LoginContainer.SetActive(true);
        signUp.SetActive(false);
    }

    IEnumerator waitSceneLogin()
    {
        while (!AccountManager.Instance.isLogin)
        {
            yield return null;
        }
        mainMenu.SetActive(true);
        Login.SetActive(false);
    }


}
