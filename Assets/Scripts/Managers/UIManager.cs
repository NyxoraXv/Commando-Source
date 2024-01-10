// This script is a Manager that controls the UI HUD (deaths, health, and score) for the 
// project. All HUD UI commands are issued through the static methods of this class

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;


public class UIManager : MonoBehaviour
{
    //This class holds a static reference to itself to ensure that there will only be
    //one in existence. This is often referred to as a "singleton" design pattern. Other
    //scripts access this one through its public static methods
    static UIManager current;
    public Image gameOver;   //Text element showing the Game Over message
    public GameObject restartButton;
    public GameObject homeButton;
    public GameObject continueButton;
    public Image healthBar;
    public Image bossBar;
    public GameObject bossBarParent;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI loseScore;
    public TextMeshProUGUI bombs;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI coinRevive;
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
        current.continueButton.gameObject.SetActive(false);

        current.winUI.gameObject.SetActive(false);
        current.winPointsText.gameObject.SetActive(false);

        current.bossBar.gameObject.SetActive(false);

        SetInitialAlpha(current.gameOver.GetComponent<Image>(), 0f);
        SetInitialAlpha(current.homeButton.GetComponent<Image>(), 0f);
        SetInitialAlpha(current.restartButton.GetComponent<Image>(), 0f);
        SetInitialAlpha(current.continueButton.GetComponent<Image>(), 0f);

        // set score text to 0
        UpdateScoreUI();
        UpdateBombsUI();
    }

    private void SetInitialAlpha(Image image, float alpha)
    {
        Color tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }

    public static void UpdateScoreUI()
    {
        //If there is no current UIManager, exit
        if (current == null)
            return;

        //Refresh the score
        current.scoreText.SetText(GameManager.GetScore().ToString());
        current.loseScore.SetText(GameManager.GetScore().ToString());
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

    current.StartCoroutine(FadeImage(current.gameOver.GetComponent<Image>(), 0f, 1f, 0.5f));
    }

    public static void Home()
    {
        if(current == null)
        return;

        // Enable the home button
        current.homeButton.gameObject.SetActive(true);

        current.StartCoroutine(FadeImage(current.homeButton.GetComponent<Image>(), 0f, 1f, 0.5f));
    }

    public static void Restart()
    {
        if(current == null)
        return;

        current.restartButton.gameObject.SetActive(true);

        current.StartCoroutine(FadeImage(current.restartButton.GetComponent<Image>(), 0f, 1f, 0.5f));
    }

    public static void Continue()
    {
        if(current == null)
        return;

        current.continueButton.gameObject.SetActive(true);

        current.StartCoroutine(FadeImage(current.continueButton.GetComponent<Image>(), 0f, 1f, 0.5f));
    }


    public static void AddHomeButton()
    {
        GameManager.LoadHome();
    }


    public static void AddRestartButton()
    {
        GameManager.GameReset();
    }

    public static void Revive()
    {

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

    private static IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        Color startColor = image.color;
        startColor.a = startAlpha;
        image.color = startColor;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float percentageComplete = elapsedTime / duration;
            Color newColor = image.color;
            newColor.a = Mathf.Lerp(startAlpha, endAlpha, percentageComplete);
            image.color = newColor;
            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = endAlpha;
        image.color = finalColor;
    }
}
