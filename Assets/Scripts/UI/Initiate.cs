using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initiate : MonoBehaviour
{
    public GameObject MainCanvas;
    private void Awake()
    {
        Instantiate(MainCanvas);
    }
}
