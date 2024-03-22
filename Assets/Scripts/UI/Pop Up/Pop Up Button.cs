using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpButton : MonoBehaviour
{
    public void Buy()
    {
        
    }

    public void Cancel(GameObject PopUp)
    {
        Destroy(PopUp);
    }
}
