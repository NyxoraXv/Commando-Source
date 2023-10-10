using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerRotation : MonoBehaviour
{
    public Transform[] layers; // An array to hold your layers

    public float rotationSpeed1 = 30f; // Rotation speed for layer 1
    public float rotationSpeed2 = 45f; // Rotation speed for layer 2
    public float rotationSpeed3 = 60f; // Rotation speed for layer 3

    private void Start()
    {
        // Populate the layers array with your child transforms
        layers = GetComponentsInChildren<Transform>();

        // Remove the parent's transform from the array
        List<Transform> layerList = new List<Transform>(layers);
        layerList.Remove(transform);
        layers = layerList.ToArray();
    }

    private void Update()
    {
        // Rotate each layer in different Z-directions
        for (int i = 0; i < layers.Length; i++)
        {
            float rotationSpeed = i == 0 ? rotationSpeed1 : i == 1 ? rotationSpeed2 : rotationSpeed3;
            layers[i].Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}
