using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpInformationhandler : MonoBehaviour
{
    public static PopUpInformationhandler Instance;
    public GameObject PopUp;

    private void Awake()
    {
        Instance = this;
    }

    public void pop(string text)
    {
        GameObject gameobject = Instantiate(PopUp);
        gameobject.GetComponent<PopUpInformation>().triggerPopup(text);

    }
}
