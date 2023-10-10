using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MapAnimator : MonoBehaviour
{
    public Camera cam;
    public Image DecoratorLeft, DecoratorRight;
    public Image Pattern;

    private void OnEnable()
    {
        StartCoroutine(PlayAnimations());
    }

    private IEnumerator PlayAnimations()
    {
        // Play the camera animation
        CameraAnimation();

        // Wait for 1 second
        yield return new WaitForSeconds(0.5f);

        // Now execute the ContainerDecoratorAnimation and PatternAnimation
        ContainerDecoratorAnimation();
        PatternAnimation();
    }

    private void ContainerDecoratorAnimation()
    {
        DecoratorLeft.DOFillAmount(1f, 1f).From(0f);
        DecoratorRight.DOFillAmount(1f, 1f).From(0f);
    }

    private void PatternAnimation()
    {
        Pattern.DOFillAmount(1f, 2f).From(0f);
    }

    private void CameraAnimation()
    {
        Transform Cam = cam.transform;
        Cam.DOLocalMoveZ(Cam.position.z, 2f).From(Cam.position.z + 40).OnComplete(() =>
        {
            cam.GetComponent<RecordingCamera>().enabled = true;
        });
    }
}
