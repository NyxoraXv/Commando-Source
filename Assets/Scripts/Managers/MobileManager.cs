using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MobileManager : MonoBehaviour
{
    public Button grenadeButton;
    public Button jumpButton;
    public Button shootButton;
    public FloatingJoystick joystick;

    private static MobileManager current;

    private bool isPressingGrenade;
    private bool isPressingShoot;
    private bool isPressingJump;

#if UNITY_STANDALONE
    private bool forceOnStandalone = false;
#endif

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
#if UNITY_STANDALONE
        // Remove this condition or set forceOnStandalone to true
        forceOnStandalone = true; 
        Debug.Log("Standalone mode: Disabling buttons and joystick.");
        DisableMobileControls();
#endif
    }

    private void DisableMobileControls()
    {
        grenadeButton.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        shootButton.gameObject.SetActive(false);
        joystick.gameObject.SetActive(false);
    }

    private void UpdateButtonState(ref bool buttonState, bool value, string debugMessage)
    {
        buttonState = value;
        Debug.Log(debugMessage);
    }

    public void PressGrenadeButton()
    {
        UpdateButtonState(ref isPressingGrenade, true, "Grenade button pressed.");
    }

    public void ReleaseGrenadeButton()
    {
        UpdateButtonState(ref isPressingGrenade, false, "Grenade button released.");
    }

    public void PressShootButton()
    {
        UpdateButtonState(ref isPressingShoot, true, "Shoot button pressed.");
    }

    public void ReleaseShootButton()
    {
        UpdateButtonState(ref isPressingShoot, false, "Shoot button released.");
    }

    public void PressJumpButton()
    {
        UpdateButtonState(ref isPressingJump, true, "Jump button pressed.");
    }

    public void ReleaseJumpButton()
    {
        UpdateButtonState(ref isPressingJump, false, "Jump button released.");
    }

    private bool GetButtonState(bool buttonState, string inputName)
    {
#if UNITY_STANDALONE
        if (forceOnStandalone)
        {
            Debug.Log($"{inputName} - Standalone mode: {buttonState}");
            return buttonState;
        }
        Debug.Log($"{inputName} - Standalone mode: {Input.GetButton(inputName)}");
        return Input.GetButton(inputName);
#else
        Debug.Log($"{inputName} - {buttonState}");
        return buttonState;
#endif
    }

    public static bool GetButtonGrenade()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return false;
        }

        return current.GetButtonState(current.isPressingGrenade, "Grenade");
    }

    public static bool GetButtonFire1()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return false;
        }

        return current.GetButtonState(current.isPressingShoot, "Fire1");
    }

    public static bool GetButtonJump()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return false;
        }

        return current.GetButtonState(current.isPressingJump, "Jump");
    }

    public static bool GetButtonSprint()
    {
        return true;
    }

    private float GetAxisValue(float joystickValue, string axisName)
    {
        float axisValue = Mathf.Abs(joystickValue) < 0.5f ? 0 : (joystickValue > Mathf.Epsilon ? 1 : (joystickValue < -Mathf.Epsilon ? -1 : 0));
        Debug.Log($"{axisName} - {axisValue}");
        return axisValue;
    }

    public static float GetAxisHorizontal()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return 0;
        }

        return current.GetAxisValue(current.joystick.Horizontal, "Horizontal");
    }

    public static float GetAxisVertical()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return 0;
        }

        return current.GetAxisValue(current.joystick.Vertical, "Vertical");
    }
}
