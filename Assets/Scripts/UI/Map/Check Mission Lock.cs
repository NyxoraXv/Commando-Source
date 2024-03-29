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
        StartCoroutine(WaitNextMission());
        Debug.Log("Instantiated Hologram Map");
    }

    IEnumerator WaitNextMission()
    {
        //SaveManager.Instance.getAchievement();
        if(SaveManager.Instance.isGetAchievement)
        {
            yield return null;
        }
        SaveManager.Instance.isGetAchievement = false;   
        if (SaveManager.Instance.playerData.achievement.LastLevel < level) { gameObject.SetActive(false); }

    }
}
