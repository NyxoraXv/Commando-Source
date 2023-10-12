using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI levelText;

    private void Awake()
    {
        onClick();
    }

    public void onClick()
    {
        levelText = GetComponent<TMPro.TextMeshProUGUI>();
        levelText.text = LevelManager.Instance.currentLevel.ToString();
        LevelManager.Instance.onXPAdded += LevelHandler;
    }

    public void LevelHandler(int xp)
    {
        levelText.text = LevelManager.Instance.currentLevel.ToString();
    }
}
