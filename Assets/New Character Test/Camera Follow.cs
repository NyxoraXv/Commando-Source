using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public Transform target;
    public float maxOffsetDistance = 5f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            HandleOffsetMovement();
        }
    }

    void HandleOffsetMovement()
    {
        Vector3 offset = CalculateOffsetFromMouse();
        Vector3 newPosition = target.position + offset;
        newPosition.z = transform.position.z; // Retain the original Z position
        transform.position = newPosition;
    }

    Vector3 CalculateOffsetFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.transform.position.z;

        Vector3 targetScreenPosition = mainCamera.WorldToScreenPoint(target.position);
        Vector3 offsetDirection = (mousePosition - targetScreenPosition).normalized;
        float distance = Mathf.Clamp(Vector3.Distance(mousePosition, targetScreenPosition) / Screen.width, 0f, 1f);
        float offsetMagnitude = distance * maxOffsetDistance;

        return offsetDirection * offsetMagnitude;
    }
}
