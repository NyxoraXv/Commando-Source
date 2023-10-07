using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance; 
    public int CurrentGold, CurrentDiamond;
    public TMPro.TMP_Text GoldDisplay, DiamondDisplay;

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
        GoldDisplay.SetText(CurrentGold.ToString());
        DiamondDisplay.SetText(CurrentDiamond.ToString());
    }

    private void OnWillRenderObject()
    {
        Refresh();
    }

    public bool spendGold(int Amount)
    {
        if(CurrentGold >= Amount)
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
}
