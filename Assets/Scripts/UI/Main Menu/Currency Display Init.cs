using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CurrencyDisplayInit : MonoBehaviour
{
    
    void Start()
    {
        GameObject FRG = GameObject.FindWithTag("Diamond");
        GameObject LUNC = GameObject.FindWithTag("Gold");

        FRG.GetComponent<TMPro.TextMeshProUGUI>().text = SaveManager.Instance.playerData.statistic.data.frg.ToString();
        LUNC.GetComponent<TMPro.TextMeshProUGUI>().text = SaveManager.Instance.playerData.statistic.data.lunc.ToString();

        gameObject.SetActive(true);
    }

}
