using System.Globalization;
using UnityEngine;
using TMPro;

public class Withdraw : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField amount;

    public void GetAllCurrency()
    {
        float frgValue = SaveManager.Instance.playerData.statistic.data.frg;
        float roundedDownValue = ForceRoundDown(frgValue);
        amount.text = roundedDownValue.ToString(CultureInfo.InvariantCulture);
    }

    public void Send()
    {
        float parsedAmount;
        var cultureInfo = CultureInfo.InvariantCulture;

        // Try to parse using the invariant culture, which uses "." as the decimal separator
        if (float.TryParse(amount.text, NumberStyles.Float, cultureInfo, out parsedAmount))
        {
            if (parsedAmount >= 0)
            {
                float roundedAmount = ForceRoundDown(parsedAmount);
                SaveManager.Instance.Withdraw(roundedAmount);
            }
            else
            {
                PopUpInformationhandler.Instance.pop("The amount cannot be negative.");
            }
        }
        else
        {
            // Handle parse failure or incorrect format
            PopUpInformationhandler.Instance.pop("Invalid amount format. Please enter a valid number.");
        }
    }

    private float ForceRoundDown(float value)
    {
        // Convert to a string with three decimal places
        string valueString = value.ToString("F3", CultureInfo.InvariantCulture);

        // Parse the string back to a float
        float truncatedValue = float.Parse(valueString, CultureInfo.InvariantCulture);

        // Truncate to two decimal places by using string manipulation
        string truncatedString = truncatedValue.ToString("F2", CultureInfo.InvariantCulture);

        // Convert back to float to ensure it's truncated
        truncatedValue = float.Parse(truncatedString, CultureInfo.InvariantCulture);

        // Calculate the remainder to the nearest 0.05
        float remainder = truncatedValue % 0.05f;

        // Subtract the remainder to round down to the nearest 0.05
        float roundedDownValue = truncatedValue - remainder;

        return roundedDownValue;
    }
}
