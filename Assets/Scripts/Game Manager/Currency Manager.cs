using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CurrencyManager : MonoBehaviour
{

    public static CurrencyManager Instance;
    public GameObject insufficientFundObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Refresh()
    {

        try
        {
            // Find the game objects with the "gold" and "diamond" tags
            GameObject goldObject = GameObject.FindGameObjectWithTag("Gold");
            GameObject diamondObject = GameObject.FindGameObjectWithTag("Diamond");

            // Check if the objects are null before accessing components
            if (goldObject != null && diamondObject != null)
            {
                // Get the TextMeshProUGUI components from the found objects
                TextMeshProUGUI LuncDisplay = goldObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI FRGDisplay = diamondObject.GetComponent<TextMeshProUGUI>();

                // Check if the components are null before updating text values
                if (LuncDisplay != null && FRGDisplay != null)
                {
                    // Update the text values
                    LuncDisplay.text = SaveManager.Instance.playerData.statistic.data.lunc.ToString();
                    FRGDisplay.text = SaveManager.Instance.playerData.statistic.data.frg.ToString();
                }
            }
        }
        catch (Exception e)
        {

        }
    }
    
    public bool spendFRG(float amount)
    {
        Statistic statistic = new Statistic
        {
            frg = amount*-1
        };

        return SaveManager.Instance.SetStatistic(statistic);
    }

    public bool addLUNC(int amount)
    {
        Statistic statistic = new Statistic
        {
            lunc = amount
        };

        return SaveManager.Instance.SetStatistic(statistic);
    }


    public void insufficientFund(float Amount, Transform transform, PopUpInstantiate.CurrencyType type)
    {
        GameObject popup = Instantiate(insufficientFundObject, transform);
        PopUpInstantiate popupInstantiateScript = popup.GetComponent<PopUpInstantiate>();

        if (popupInstantiateScript != null)
        {
            popupInstantiateScript.pop(Amount);
        }
        else
        {
            Debug.LogError("PopUpInstantiate script not found on the instantiated object.");
        }
    }
}
