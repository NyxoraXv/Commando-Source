using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MobileManager : MonoBehaviour
{
    public Button grenadeButton;
    public Button jumpButton;
    public Button sprintButton;
    public FloatingJoystick joystick;
    public FloatingJoystick ArmJoystick;

    public bool grenadeButtonClicked;
    public bool jumpButtonClicked;
    public bool sprintButtonClicked;

    private static MobileManager current;

#if UNITY_STANDALONE
    private bool forceOnStandalone = false;
#endif

    private void Awake()
    {
        current = this;
        sprintButton.onClick.AddListener(OnSprintButtonClick);
        jumpButton.onClick.AddListener(OnJumpButtonClick);
        grenadeButton.onClick.AddListener(OnGrenadeButtonClick);

    }

    private void Start()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        // If running on PC or in the editor, disable the MobileManager GameObject
        Debug.Log("Running on PC. Disabling MobileManager GameObject.");
        gameObject.SetActive(false);
#endif
    }

    private void LateUpdate()
    {
        grenadeButtonClicked = false;
        jumpButtonClicked = false;
        sprintButtonClicked = false;
    }

    private void DisableMobileControls()
    {
        grenadeButton.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        sprintButton.gameObject.SetActive(false);
        joystick.gameObject.SetActive(false);
    }

    private void OnSprintButtonClick()
    {
        sprintButtonClicked = true;
    }

    private void OnJumpButtonClick()
    {
        jumpButtonClicked = true;
    }

    private void OnGrenadeButtonClick()
    {
        grenadeButtonClicked = true;
    }

    public static bool GetButtonSprint()
    {
        return current.sprintButtonClicked;
    }

    public static bool GetButtonJump()
    {
        return current.jumpButtonClicked;
    }

    public static bool GetButtonGrenade()
    {
        return current.grenadeButtonClicked;
    }

    private float GetAxisValue(float joystickValue, string axisName)
    {

        return joystickValue;
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

    public static float GetArmAxisVertical()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return 0;
        }

        return current.GetAxisValue(current.ArmJoystick.Vertical, "Vertical");
    }

    public static float GetArmAxisHorizontal()
    {
        if (!current)
        {
            Debug.LogError("MobileManager not initialized!");
            return 0;
        }

        return current.GetAxisValue(current.ArmJoystick.Horizontal, "Horizontal");
    }

}
