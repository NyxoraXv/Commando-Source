using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavManager : MonoBehaviour
{
    public float speed = 0.5f;
    public float damp = 0.15f;
    public Toggle Automove;
    public float maxDistanceX = 160;
    private float minDistanceX = 0;
    public bool canMoveY;

    public Transform Player;
    public float minDistanceY;
    public float maxDistanceY;

    void Start()
    {
        canMoveY = false;
        Automove.isOn = false;
    }


    void LateUpdate()
    {
        //Move Left (if you are using different OS you can change KeyCode acording to your system
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && Player.position.x >= minDistanceX)
        {
            transform.position += Vector3.left * speed * damp * Time.deltaTime;
            Automove.isOn = false;
        }
        //Move Right (if you are using different OS you can change KeyCode acording to your system
        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && Player.position.x <= maxDistanceX)
        {
            transform.position += Vector3.right * speed * damp * Time.deltaTime;
            Automove.isOn = false;
        }
        //Move Down (if you are using different OS you can change KeyCode acording to your system
        if (Automove.isOn && Player.position.x <= maxDistanceX)
        {
            transform.position += Vector3.right * speed * damp * Time.deltaTime;
        }
        if (Automove.isOn && canMoveY && Player.position.y >= minDistanceY)
        {
            transform.position += Vector3.down * speed * damp * Time.deltaTime;
        }

        //Move Down (if you are using different OS you can change KeyCode acording to your system
        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && Player.position.y >= minDistanceY && canMoveY)
        {
            transform.position += Vector3.down * speed * damp * Time.deltaTime;
            Automove.isOn = false;
        }
        //Move Up (if you are using different OS you can change KeyCode acording to your system
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && Player.position.y <= maxDistanceY && canMoveY)
        {
            transform.position += Vector3.up * speed * damp * Time.deltaTime;
            Automove.isOn = false;
        }
    }
    public void AutomoverCheck()
    {
        if (Automove.isOn == false)
        {
            Automove.isOn = true;
        }
        else
        {
            Automove.isOn = false;
        }
    }
}
