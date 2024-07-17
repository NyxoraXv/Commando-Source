using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour
{
    private Transform background;
    public float parallaxScaleX = -20;
    public float parallaxScaleY = 0;
    public float smoothing = 1f;
    public bool isActive = true;

    private Transform cam;
    private Vector3 previousCamPos;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // Attempt to get Camera.main in case it's already assigned
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
            previousCamPos = cam.position;
        }
        else
        {
            // If Camera.main is not yet assigned, wait for it to become available
            StartCoroutine(WaitForCam());
        }

        background = transform; // Assuming the script is attached to the background itself
    }

    IEnumerator WaitForCam()
    {
        yield return new WaitUntil(() => Camera.main != null);
        cam = Camera.main.transform;
        previousCamPos = cam.position;
    }

    void FixedUpdate()
    {
        if (!isActive || cam == null)
            return;

        float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScaleX;
        float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScaleY;

        float backgroundTargetPosX = background.position.x + parallaxX;
        float backgroundTargetPosY = background.position.y + parallaxY;

        Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgroundTargetPosY, background.position.z);

        background.position = Vector3.Lerp(background.position, backgroundTargetPos, smoothing * Time.deltaTime);

        previousCamPos = cam.position;
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }
}
