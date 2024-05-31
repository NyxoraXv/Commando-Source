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
        if (SaveManager.Instance.playerData.statistic.data.last_level < level) { gameObject.SetActive(false); }
        Debug.Log("Instantiated Hologram Map");
    }

}
