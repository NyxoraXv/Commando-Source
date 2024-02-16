using UnityEngine;
using UnityEngine.UI;

public class PopUpInstantiate : MonoBehaviour
{
    public static PopUpInstantiate Instance;

    public enum CurrencyType
    {
        Gold,
        Diamond
    }

    public Sprite goldPrefab;
    public Sprite diamondPrefab;
    private Sprite selectedSprite;

    public Image Icon;
    public TMPro.TextMeshProUGUI amountDisplay;

    public CurrencyType Type { get; set; }


    public void pop(float Amount)
    {
        switch (Type)
        {
            case CurrencyType.Gold:
                selectedSprite = goldPrefab;
                InstantiateCurrencyPrefab(Type, Amount);
                break;
            case CurrencyType.Diamond:
                selectedSprite = diamondPrefab;
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
