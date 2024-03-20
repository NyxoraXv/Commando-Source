using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckMissionLock : MonoBehaviour
{
    public int level;
    public Image layer;
    void Start()
    {
        SaveManager.Instance.getAchievement();
        if (SaveManager.Instance.playerData.playerInformation.PlayerLastLevel < level) { gameObject.SetActive(false); }
        for (int i = 0; i < level; i++)
        {
            layer.color = Color.white;
        }
    }
}
