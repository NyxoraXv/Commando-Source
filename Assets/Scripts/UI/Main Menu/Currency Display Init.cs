using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CurrencyDisplayInit : MonoBehaviour
{
    
    void Start()
    {
        GameObject Diamond = GameObject.FindWithTag("Diamond");
        GameObject Gold = GameObject.FindWithTag("Gold");

        Diamond.GetComponent<TMPro.TextMeshProUGUI>().text = SaveManager.Instance.playerData.currencyInfo.PlayerFRG.ToString();
        Gold.GetComponent<TMPro.TextMeshProUGUI>().text = SaveManager.Instance.playerData.currencyInfo.PlayerLUNC.ToString();

        gameObject.SetActive(true);
    }

}
