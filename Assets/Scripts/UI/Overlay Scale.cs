using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayScale : MonoBehaviour
{
    private void Awake()
    {
        Display display;

        GetComponent<Image>().transform.localScale = new Vector3(
            display.systemWidth,
            display.systemHeight,
            0
            );
    }
}
