using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NavigationButtonManager : MonoBehaviour
{
    public BackgroundParallax parallax;

    [Header("Change Character Settings")]
    public float changeCharacterScaleEnd = 1.0f;
    public float changeCharacterScaleDuration = 1.0f;
    public Vector3 changeCharacterMoveEnd = Vector3.zero;
    public float changeCharacterMoveDuration = 1.0f;
    public Vector3 changeCharacterRotateEnd = Vector3.zero;
    public float changeCharacterRotateDuration = 1.0f;
    public Ease changeCharacterParallaxEase = Ease.InOutCubic;

    [Header("Main Menu Settings")]
    public float mainMenuScaleEnd = 1.0f;
    public float mainMenuScaleDuration = 1.0f;
    public Vector3 mainMenuMoveEnd = Vector3.zero;
    public float mainMenuMoveDuration = 1.0f;
    public Vector3 mainMenuRotateEnd = Vector3.zero;
    public float mainMenuRotateDuration = 1.0f;
    public Ease mainMenuParallaxEase = Ease.InOutCubic;

    [Header("Objectives Settings")]
    public float objectivesScaleEnd = 1.0f;
    public float objectivesScaleDuration = 1.0f;
    public Vector3 objectivesMoveEnd = Vector3.zero;
    public float objectivesMoveDuration = 1.0f;
    public Vector3 objectivesRotateEnd = Vector3.zero;
    public float objectivesRotateDuration = 1.0f;
    public Ease objectivesParallaxEase = Ease.InOutCubic;

    [Header("Change Weapon Settings")]
    public float changeWeaponScaleEnd = 1.0f;
    public float changeWeaponScaleDuration = 1.0f;
    public Vector3 changeWeaponMoveEnd = Vector3.zero;
    public float changeWeaponMoveDuration = 1.0f;
    public Vector3 changeWeaponRotateEnd = Vector3.zero;
    public float changeWeaponRotateDuration = 1.0f;
    public Ease changeWeaponParallaxEase = Ease.InOutCubic;

    [Header("Setting Settings")]
    public float settingScaleEnd = 1.0f;
    public float settingScaleDuration = 1.0f;
    public Vector3 settingMoveEnd = Vector3.zero;
    public float settingMoveDuration = 1.0f;
    public Vector3 settingRotateEnd = Vector3.zero;
    public float settingRotateDuration = 1.0f;
    public Ease settingParallaxEase = Ease.InOutCubic;

    [Header("Event Settings")]
    public float eventScaleEnd = 1.0f;
    public float eventScaleDuration = 1.0f;
    public Vector3 eventMoveEnd = Vector3.zero;
    public float eventMoveDuration = 1.0f;
    public Vector3 eventRotateEnd = Vector3.zero;
    public float eventRotateDuration = 1.0f;
    public Ease eventParallaxEase = Ease.InOutCubic;

    [Header("Market Settings")]
    public float marketScaleEnd = 1.0f;
    public float marketScaleDuration = 1.0f;
    public Vector3 marketMoveEnd = Vector3.zero;
    public float marketMoveDuration = 1.0f;
    public Vector3 marketRotateEnd = Vector3.zero;
    public float marketRotateDuration = 1.0f;
    public Ease marketParallaxEase = Ease.InOutCubic;

    void Start()
    {
        // Create a new sequence
        Sequence rotationSequence = DOTween.Sequence();

        // Create the first rotation animation and append it to the sequence
        rotationSequence.Append(parallax.transform.DORotate(new Vector3(0, 0, 360), 1200f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear));

        // Create the second rotation animation and append it to the sequence
        rotationSequence.Append(parallax.transform.DORotate(new Vector3(0, 0, 180), 1200f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear));

        // You can continue to append more rotations as needed
        rotationSequence.SetLoops(-1, LoopType.Incremental); // Infinite looping
    }

    public void ToChangeCharacter()
    {
        parallax.doParallax(changeCharacterScaleEnd,
                            changeCharacterScaleDuration,
                            changeCharacterMoveEnd,
                            changeCharacterMoveDuration,
                            changeCharacterRotateEnd,
                            changeCharacterRotateDuration,
                            changeCharacterParallaxEase);
    }

    public void ToMainMenu()
    {
        parallax.doParallax(mainMenuScaleEnd,
                            mainMenuScaleDuration,
                            mainMenuMoveEnd,
                            mainMenuMoveDuration,
                            mainMenuRotateEnd,
                            mainMenuRotateDuration,
                            mainMenuParallaxEase);
    }

    public void ToObjectives()
    {
        parallax.doParallax(objectivesScaleEnd,
                            objectivesScaleDuration,
                            objectivesMoveEnd,
                            objectivesMoveDuration,
                            objectivesRotateEnd,
                            objectivesRotateDuration,
                            objectivesParallaxEase);
    }

    public void ToChangeWeapon()
    {
        parallax.doParallax(changeWeaponScaleEnd,
                            changeWeaponScaleDuration,
                            changeWeaponMoveEnd,
                            changeWeaponMoveDuration,
                            changeWeaponRotateEnd,
                            changeWeaponRotateDuration,
                            changeWeaponParallaxEase);
    }

    public void ToSetting()
    {
        parallax.doParallax(settingScaleEnd,
                            settingScaleDuration,
                            settingMoveEnd,
                            settingMoveDuration,
                            settingRotateEnd,
                            settingRotateDuration,
                            settingParallaxEase);
    }

    public void ToEvent()
    {
        parallax.doParallax(eventScaleEnd,
                            eventScaleDuration,
                            eventMoveEnd,
                            eventMoveDuration,
                            eventRotateEnd,
                            eventRotateDuration,
                            eventParallaxEase);
    }

    public void ToMarket()
    {
        parallax.doParallax(marketScaleEnd,
                            marketScaleDuration,
                            marketMoveEnd,
                            marketMoveDuration,
                            marketRotateEnd,
                            marketRotateDuration,
                            marketParallaxEase);
    }
}
