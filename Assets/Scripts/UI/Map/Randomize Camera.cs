using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingCamera : MonoBehaviour
{
    private Camera cam;
    public float verticalAmplitude = 0.1f;
    public float verticalSpeed = 1.0f;
    public float horizontalAmplitude = 0.1f;
    public float horizontalSpeed = 1.0f;

    private Vector3 initialPosition;
    private Vector3 randomOffset;

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        initialPosition = transform.position;
        randomOffset = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0f);
    }

    private void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * verticalSpeed) * verticalAmplitude;

        float newX = initialPosition.x + Mathf.Sin(Time.time * horizontalSpeed + randomOffset.x) * horizontalAmplitude;
        float newZ = initialPosition.z + Mathf.Sin(Time.time * horizontalSpeed + randomOffset.y) * horizontalAmplitude;

        Vector3 newPosition = new Vector3(newX, newY, newZ);
        transform.position = newPosition;
    }
}
