using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileManager : MonoBehaviour
{
    public Button grenadeButton;
    public Button jumpButton;
    public Button shootButton;
    public FloatingJoystick joystick;

    static MobileManager current;

    bool btnPressGrenade;
    bool btnPressShoot;
    bool btnPressJump;

#if UNITY_STANDALONE
    private bool forceOnStandalone = false;
#endif

    void Awake()
    {
        current = this;
    }

    void Start()
    {
#if UNITY_STANDALONE
        if (!forceOnStandalone)
        {
            Debug.Log("Standalone mode: Disabling buttons and joystick.");
            grenadeButton.gameObject.SetActive(false);
            jumpButton.gameObject.SetActive(false);
            shootButton.gameObject.SetActive(false);
            joystick.gameObject.SetActive(false);
        }
#endif
    }

    void LateUpdate()
    {
        btnPressShoot = false;
        btnPressJump = false;
        btnPressGrenade = false;
    }

    public void ClickButtonShoot()
    {
        btnPressShoot = true;
        Debug.Log("Shoot button clicked.");
    }

    public void ClickButtonJump()
    {
        btnPressJump = true;
        Debug.Log("Jump button clicked.");
    }

    public void ClickButtonGrenade()
    {
        btnPressGrenade = true;
        Debug.Log("Grenade button clicked.");
    }

    bool _GetButtonGrenade()
    {
        return btnPressGrenade;
    }

    static public bool GetButtonGrenade()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return false;
        }

#if UNITY_STANDALONE
        if (current.forceOnStandalone)
        {
            bool result = current._GetButtonGrenade();
            Debug.Log($"GetButtonGrenade - Standalone mode: {result}");
            return result;
        }
        Debug.Log($"GetButtonGrenade - Standalone mode: {Input.GetKeyDown(KeyCode.G)}");
        return Input.GetKeyDown(KeyCode.G);
#else
        bool result = current._GetButtonGrenade();
        Debug.Log($"GetButtonGrenade - {result}");
        return result;
#endif
    }

    bool _GetButtonFire1()
    {
        return btnPressShoot;
    }

    static public bool GetButtonFire1()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return false;
        }

#if UNITY_STANDALONE
        if (current.forceOnStandalone)
        {
            bool result = current._GetButtonFire1();
            Debug.Log($"GetButtonFire1 - Standalone mode: {result}");
            return result;
        }
        Debug.Log($"GetButtonFire1 - Standalone mode: {Input.GetButton("Fire1")}");
        return Input.GetButton("Fire1");
#else
        bool result = current._GetButtonFire1();
        Debug.Log($"GetButtonFire1 - {result}");
        return result;
#endif
    }

    bool _GetButtonJump()
    {
        return btnPressJump;
    }

    static public bool GetButtonJump()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return false;
        }

#if UNITY_STANDALONE
        if (current.forceOnStandalone)
        {
            bool result = current._GetButtonJump();
            Debug.Log($"GetButtonJump - Standalone mode: {result}");
            return result;
        }
        Debug.Log($"GetButtonJump - Standalone mode: {Input.GetButton("Jump")}");
        return Input.GetButton("Jump");
#else
        bool result = current._GetButtonJump();
        Debug.Log($"GetButtonJump - {result}");
        return result;
#endif
    }

    bool _GetButtonCrouch()
    {
        return joystick.Vertical < -0.5f;
    }

    static public bool GetButtonCrouch()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return false;
        }

#if UNITY_STANDALONE
        if (current.forceOnStandalone)
        {
            bool result = current._GetButtonCrouch();
            Debug.Log($"GetButtonCrouch - Standalone mode: {result}");
            return result;
        }
        Debug.Log($"GetButtonCrouch - Standalone mode: {Input.GetButton("Crouch")}");
        return Input.GetButton("Crouch");
#else
        bool result = current._GetButtonCrouch();
        Debug.Log($"GetButtonCrouch - {result}");
        return result;
#endif
    }

    float _GetAxisHorizontal()
    {
        float horizontalValue = Mathf.Abs(joystick.Horizontal) < 0.5f ? 0 : (joystick.Horizontal > Mathf.Epsilon ? 1 : (joystick.Horizontal < -Mathf.Epsilon ? -1 : 0));
        Debug.Log($"GetAxisHorizontal - {horizontalValue}");
        return horizontalValue;
    }

    static public float GetAxisHorizontal()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return 0;
        }

#if UNITY_STANDALONE
        if (current.forceOnStandalone)
        {
            float result = current._GetAxisHorizontal();
            Debug.Log($"GetAxisHorizontal - Standalone mode: {result}");
            return result;
        }
        Debug.Log($"GetAxisHorizontal - Standalone mode: {Input.GetAxis("Horizontal")}");
        return Input.GetAxis("Horizontal");
#else
        float result = current._GetAxisHorizontal();
        Debug.Log($"GetAxisHorizontal - {result}");
        return result;
#endif
    }

    float _GetAxisVertical()
    {
        float verticalValue = Mathf.Abs(joystick.Vertical) < 0.5f ? 0 : (joystick.Vertical > Mathf.Epsilon ? 1 : (joystick.Vertical < -Mathf.Epsilon ? -1 : 0));
        Debug.Log($"GetAxisVertical - {verticalValue}");
        return verticalValue;
    }

    static public float GetAxisVertical()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return 0;
        }

#if UNITY_STANDALONE
        if (current.forceOnStandalone)
        {
            float result = current._GetAxisVertical();
            Debug.Log($"GetAxisVertical - Standalone mode: {result}");
            return result;
        }
        Debug.Log($"GetAxisVertical - Standalone mode: {Input.GetAxis("Vertical")}");
        return Input.GetAxis("Vertical");
#else
        float result = current._GetAxisVertical();
        Debug.Log($"GetAxisVertical - {result}");
        return result;
#endif
    }
}
