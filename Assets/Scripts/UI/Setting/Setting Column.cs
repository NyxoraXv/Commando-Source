using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingInputType
{
    Slider,
    Option,
    CustomOption
}

public class SettingColumn : MonoBehaviour
{
    public string title;
    public SettingInputType Type;
    [Header("Dependencies")]
    public TMPro.TextMeshProUGUI Title;
    public GameObject SliderPrefab, OptionPrefab, CustomOptionPrefab;

    // Start method with branching based on SettingInputType
    public void Start()
    {
        Title.text = title;

        // Disable all prefabs at the beginning
        SliderPrefab.SetActive(false);
        OptionPrefab.SetActive(false);
        CustomOptionPrefab.SetActive(false);

        // Determine which prefab to enable based on the type
        GameObject prefabToEnable = null;
        switch (Type)
        {
            case SettingInputType.Slider:
                prefabToEnable = SliderPrefab;
                break;

            case SettingInputType.Option:
                prefabToEnable = OptionPrefab;
                break;

            case SettingInputType.CustomOption:
                prefabToEnable = CustomOptionPrefab;
                break;
        }

        if (prefabToEnable != null)
        {
            // Enable the selected prefab
            prefabToEnable.SetActive(true);
        }
    }
}
