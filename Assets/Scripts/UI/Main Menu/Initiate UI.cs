using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitiateUI : MonoBehaviour
{
    public GameObject Login, MainMenu;

    void Start()
    {
        // Find the GameObject with the tag "Main Camera"
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        // Get the Canvas component attached to this GameObject
        Canvas canvas = GetComponent<Canvas>();

        // Set the rendering camera of the Canvas to the "Main Camera"
        if (mainCamera != null && canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCamera.GetComponent<Camera>();
        }
        else
        {
            Debug.LogError("Main Camera or Canvas component not found. Make sure the 'Main Camera' tag is assigned to the appropriate GameObject.");
        }

        if(SaveManager.Instance.isLogin == true)
        {
            Login.SetActive(false);
            MainMenu.SetActive(true);
            
        }
    }
}
