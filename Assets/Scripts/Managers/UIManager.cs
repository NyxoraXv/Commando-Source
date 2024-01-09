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
    public Image gameOver;   //Text element showing the Game Over message
    public GameObject restartButton;
    public GameObject homeButton;
    public Image healthBar;
    public Image bossBar;
    public GameObject bossBarParent;
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
        current.gameOver.gameObject.SetActive(false);

        current.restartButton.gameObject.SetActive(false);
        current.homeButton.gameObject.SetActive(false);

        current.winUI.gameObject.SetActive(false);
        current.winPointsText.gameObject.SetActive(false);

        current.bossBar.gameObject.SetActive(false);

        // set score text to 0
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

    public static void UpdateBossHealthUI(float health, float maxHealth)
    {
        if (current == null)
            return;

        current.bossBar.gameObject.SetActive(true);
        current.bossBarParent.SetActive(true);
        float fillAmount = health / maxHealth;
        current.bossBar.DOFillAmount(fillAmount, 0.5f); // Adjust duration as needed
    }

    public static void HideBossHealthBar()
    {
        if (current == null)
            return;

         // Deactivate the boss health bar
          current.bossBar.gameObject.SetActive(false);
        current.bossBarParent.SetActive(false);
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

    // Activate the game over text
    current.gameOver.gameObject.SetActive(true);

    Vector3 targetScale = current.gameOver.transform.localScale * 1f; // Double the current scale

    current.gameOver.transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBack);
    }

    public static void Home()
    {
        if(current == null)
        return;

        // Enable the home button
        current.homeButton.gameObject.SetActive(true);
    }

    public static void Restart()
    {
        if(current == null)
        return;

        current.restartButton.gameObject.SetActive(true);
    }


    public static void AddHomeButton()
    {
        GameManager.LoadHome();
    }


    public static void AddRestartButton()
    {
        GameManager.GameReset();
        GameManager.ReloadCurrentScene();
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
