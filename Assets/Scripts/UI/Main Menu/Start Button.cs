using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartButton : MonoBehaviour
{
    public GameObject HologramMap; // Assign the HologramMap prefab in the Inspector
    public GameObject MainCanvas; // Assign the MainCanvas GameObject in the Inspector
    private CanvasGroup mainCanvasGroup;

    private void Start()
    {
        // Get the CanvasGroup component from the MainCanvas GameObject
        if (MainCanvas != null)
        {
            mainCanvasGroup = MainCanvas.GetComponent<CanvasGroup>();
        }
        else
        {
            Debug.LogError("MainCanvas is not set. Assign the 'MainCanvas' GameObject in the Inspector.");
        }
    }

    public void onClick()
    {

        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.DOFade(0f, 0.5f).From(1f);
            SaveManager.Instance.GetStatistic((data, isSuccess, message) =>
            {
                // Handle the data retrieval
                Destroy(MainCanvas);
                Instantiate(HologramMap);
            });
        }
        else
        {
            Debug.LogError("CanvasGroup is not set. Make sure you assign the 'MainCanvas' GameObject in the Inspector.");
        }
    }
}
