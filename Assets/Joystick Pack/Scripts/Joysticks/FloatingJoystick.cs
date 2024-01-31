using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    private Vector3 anchorPosition;

    protected override void Start()
    {
        base.Start();
        //background.gameObject.SetActive(false);
        anchorPosition = background.gameObject.transform.position;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        //background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.transform.position = anchorPosition;
        base.OnPointerUp(eventData);
    }
}