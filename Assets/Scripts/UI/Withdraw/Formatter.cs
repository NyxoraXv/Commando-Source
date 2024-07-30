using UnityEngine;
using TMPro;

public class Formatter : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        inputField.onValueChanged.AddListener(FormatInput);
    }

    void FormatInput(string input)
    {
        if (float.TryParse(input, out float value))
        {
            inputField.text = value.ToString("0.00");
        }
        else
        {
            inputField.text = "0.00";
        }
    }
}
