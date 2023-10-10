using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartButton : MonoBehaviour
{
    public GameObject MainParent, Map; 

    public void onClick()
    {
        CanvasGroup Parent = MainParent.GetComponent<CanvasGroup>();

        Parent.DOFade(0f, 0.5f).From(1f).OnComplete(() =>{

            MainParent.SetActive(false);
            Map.SetActive(true);
        
        });

    }
}
