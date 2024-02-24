using UnityEngine;
using UnityEngine.UI;

public class PopUpInstantiate : MonoBehaviour
{
    public static PopUpInstantiate Instance;

    public enum CurrencyType
    {
        FRG,
        LUNC
    }

    public Sprite frgPrefab;
    public Sprite luncPrefab;
    private Sprite selectedSprite;

    public Image Icon;
    public TMPro.TextMeshProUGUI amountDisplay;

    public CurrencyType Type { get; set; }


    public void pop(float Amount)
    {
        switch (Type)
        {
            case CurrencyType.FRG:
                selectedSprite = frgPrefab;
                InstantiateCurrencyPrefab(Type, Amount);
                break;
            case CurrencyType.LUNC:
                selectedSprite = luncPrefab;
                InstantiateCurrencyPrefab(Type, Amount);
                break;
        }
    }

    private void InstantiateCurrencyPrefab(CurrencyType type, float Amount)
    {
        Icon.sprite = selectedSprite;
        amountDisplay.text = Amount.ToString();
    }
}
