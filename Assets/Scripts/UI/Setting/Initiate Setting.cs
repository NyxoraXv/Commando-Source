using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitiateSetting : MonoBehaviour
{
    public GameObject Graphics, Control, Sound;
    public NavigationButton NavButtonManager;
    public RectTransform graphicsButton;

    private void OnEnable()
    {
        Graphics.SetActive(false);
        Graphics.SetActive(false);
        Graphics.SetActive(false);

        NavButtonManager.OnGraphicsClicked(graphicsButton);
    }
}
