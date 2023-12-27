using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int CurrentGold, CurrentDiamond;
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
                TextMeshProUGUI GoldDisplay = goldObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI DiamondDisplay = diamondObject.GetComponent<TextMeshProUGUI>();

                // Check if the components are null before updating text values
                if (GoldDisplay != null && DiamondDisplay != null)
                {
                    // Update the text values
                    GoldDisplay.text = CurrentGold.ToString();
                    DiamondDisplay.text = CurrentDiamond.ToString();
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component is missing on either Gold or Diamond objects.");
                }
            }
            else
            {
                Debug.LogError("Gold or Diamond object not found.");
            }
        }
        catch (Exception e)
        {
            // Handle any other exceptions that might occur
            Debug.LogError($"An error occurred: {e.Message}");
        }


    }

    private void OnWillRenderObject()
    {
        Refresh();
    }

    public bool spendGold(int Amount)
    {
        if (CurrentGold >= Amount)
        {
            CurrentGold -= Amount;
            Refresh();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool spendDiamond(int Amount)
    {
        if (CurrentDiamond >= Amount)
        {
            CurrentDiamond -= Amount;
            Refresh();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void addGold(int Amount)
    {
        CurrentGold += Amount;
        Refresh();
    }

    public void addDiamond(int Amount)
    {
        CurrentDiamond += Amount;
        Refresh();
    }

    public void insufficientFund(int Amount, Transform transform, PopUpInstantiate.CurrencyType type)
    {
        GameObject popup = Instantiate(insufficientFundObject, transform);
        PopUpInstantiate popupInstantiateScript = popup.GetComponent<PopUpInstantiate>();

        if (popupInstantiateScript != null)
        {
            // Call a method on the PopUpInstantiate script to set parameters
            popupInstantiateScript.Instantiate(Amount);
        }
        else
        {
            Debug.LogError("PopUpInstantiate script not found on the instantiated object.");
        }
    }
}
