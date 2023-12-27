using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BackButtonHandler : MonoBehaviour
{
    public GameObject HologramMap, MainCanvas;

    public void onClick()
    {
        Instantiate(MainCanvas);
        CanvasGroup canvasGroup = MainCanvas.GetComponent<CanvasGroup>();


            // Check if the CanvasGroup component is found
            if (canvasGroup != null)
            {
                // Fade in the CanvasGroup using DOTween
                canvasGroup.DOFade(1f, 0.5f)
                    .SetEase(Ease.InOutCubic)
                    .From(0f)
                    .OnPlay(() => {
                        // Destroy the HologramMap GameObject
                        Destroy(HologramMap);
                    });
            }
    }
}
