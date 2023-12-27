// This script is a Manager that controls the UI HUD (deaths, health, and score) for the 
// project. All HUD UI commands are issued through the static methods of this class

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    //This class holds a static reference to itself to ensure that there will only be
    //one in existence. This is often referred to as a "singleton" design pattern. Other
    //scripts access this one through its public static methods
    static UIManager current;
    public TextMeshProUGUI gameOverText;    //Text element showing the Game Over message
    public Image healthBar;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bombs;
    public TextMeshProUGUI ammoText;
    public Image winUI;
    public TextMeshProUGUI winPointsText;
    public GameObject MobileCanvas;

    public Image characterAvatar;

    void Awake()
    {
        if (current)
            Destroy(current);
        current = this;

        characterAvatar.sprite = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.FullAvatar;

        // disable game over text
        current.gameOverText.gameObject.SetActive(false);

        current.winUI.gameObject.SetActive(false);
        current.winPointsText.gameObject.SetActive(false);

        // set score text to 0
        MobileCanvas.SetActive(true);
        UpdateScoreUI();
        UpdateBombsUI();
    }

    public static void UpdateScoreUI()
    {
        //If there is no current UIManager, exit
        if (current == null)
            return;

        //Refresh the score
        current.scoreText.SetText(GameManager.GetScore().ToString());
    }

   public static void UpdateBombsUI()
    {
        //If there is no current UIManager, exit
        if (current == null)
            return;

        //Refresh the score
        current.bombs.SetText(GameManager.GetBombs().ToString());
    }

    public static void UpdateAmmoUI()
    {
        //If there is no current UIManager, exit
        if (current == null)
            return;

        //Refresh the score
        if (GameManager.GetHeavyMachineAmmo() == 0)
        {
            current.ammoText.SetText("oo");
        }
        else
        {
            current.ammoText.SetText(GameManager.GetHeavyMachineAmmo().ToString());
        }
    }

    public static void DisplayGameOverText()
    {
        //If there is no current UIManager, exit
        if (current == null)
            return;

        //Show the game over text
        current.gameOverText.gameObject.SetActive(true);
    }

    public static void UpdateHealthUI(float health, float maxHealth)
    {
        // If there is no current UIManager, exit
        if (current == null)
            return;

        // Calculate the fill amount percentage
        float fillAmount = health / maxHealth;

        // Animate the health bar fill amount using DOTween
        current.healthBar.DOFillAmount(fillAmount, 0.5f); // Adjust the duration as needed
    }


    public static void DisplayWinUI()
    {
        //If there is no current UIManager, exit
        if (current == null)
            return;

        //Show the win text and points
        current.winUI.gameObject.SetActive(true);

        current.winPointsText.SetText(GameManager.GetScore().ToString());
        current.winPointsText.gameObject.SetActive(true);
    }
}
