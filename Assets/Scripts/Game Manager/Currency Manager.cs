using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public float CurrentLUNC, CurrentFRG;
    public GameObject insufficientFundObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        Refresh();
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
                TextMeshProUGUI GoldDisplay = goldObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI DiamondDisplay = diamondObject.GetComponent<TextMeshProUGUI>();

                // Check if the components are null before updating text values
                if (GoldDisplay != null && DiamondDisplay != null)
                {
                    // Update the text values
                    GoldDisplay.text = CurrentLUNC.ToString();
                    DiamondDisplay.text = CurrentFRG.ToString();
                }
            }
        }
        catch (Exception e)
        {
            // Handle any other exceptions that might occur
            Debug.LogError($"An error occurred: {e.Message}");
        }


    }

    public bool spendFRG(float Amount)
    {
        if (CurrentLUNC >= Amount)
        {
            CurrentLUNC -= Amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool spendLUNC(float Amount)
    {
        if (CurrentFRG >= Amount)
        {
            CurrentFRG -= Amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void addFRG(float Amount)
    {
        CurrentLUNC += Amount;
    }

    public void addLUNC(float Amount)
    {
        CurrentFRG += Amount;
    }

    public void insufficientFund(float Amount, Transform transform, PopUpInstantiate.CurrencyType type)
    {
        GameObject popup = Instantiate(insufficientFundObject, transform);
        PopUpInstantiate popupInstantiateScript = popup.GetComponent<PopUpInstantiate>();

        if (popupInstantiateScript != null)
        {
            // Call a method on the PopUpInstantiate script to set parameters
            popupInstantiateScript.pop(Amount);
        }
        else
        {
            Debug.LogError("PopUpInstantiate script not found on the instantiated object.");
        }
    }
}
