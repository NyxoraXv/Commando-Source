using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackButtonHandler : MonoBehaviour
{
    public CanvasGroup MainParent;
    public GameObject HologramMap;
    public void onClick()
    {
        MainParent = Instantiate(MainParent);
        MainParent.DOFade(1f, 0.5f).SetEase(Ease.InOutCubic).From(0f).OnPlay(() => {
            Destroy(HologramMap);
        });

    }
}
