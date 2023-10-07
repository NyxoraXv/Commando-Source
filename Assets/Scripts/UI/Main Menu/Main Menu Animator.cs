using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MyAnimator : MonoBehaviour
{
    public GameObject Avatar, navbar, profile;

    void OnEnable()
    {
        if (Avatar != null)
        {
            Vector3 AvatarinitialPosition = Avatar.transform.localPosition;
            AvatarinitialPosition.x = -200f;
            Avatar.transform.localPosition = AvatarinitialPosition;
            Avatar.transform.DOLocalMoveX(-118f, 2f).SetEase(Ease.OutCubic);
        }
        
        if(navbar != null)
        {
            Vector3 NavbarinitialPosition = navbar.transform.localPosition;
            NavbarinitialPosition.x = -1072f;
            navbar.transform.localPosition = NavbarinitialPosition;
            navbar.transform.DOLocalMoveX(-838f, 2f).SetEase(Ease.OutCubic);
        }

        if(profile != null)
        {
            Vector3 ProfileinitialPosition = profile.transform.localPosition;
            ProfileinitialPosition.x = 360f;
            profile.transform.localPosition = ProfileinitialPosition;
            profile.transform.DOLocalMoveX(161f, 2f).SetEase(Ease.OutCubic);
        }

        if(gameObject.GetComponent<CanvasGroup>() != null)
        {
            gameObject.GetComponent<CanvasGroup>().DOFade(1f, 2f).SetEase(Ease.OutCubic).From(0f);
        }
    }


}
