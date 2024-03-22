using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SpriteShadersUltimate;

public class UIManager : MonoBehaviour
{
    static UIManager current;
    public GameObject gameOver;
    public GameObject restartButton;
    public GameObject restartWin;
    public GameObject homeButton;
    public GameObject homeWin;
    public GameObject continueButton;
    public Image healthBar;
    public Image bossBar;
    public GameObject bossBarParent;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI loseScore;
    public TextMeshProUGUI bombs;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI coinRevive;
    public GameObject winUI;
    public TextMeshProUGUI winPointsText;
    public GameObject MobileCanvas;
    public GameObject Currency;
    public GameObject CurrentCurrency;
    public GameObject backButton;
    public GameObject buyButton;
    public GameObject shopUI;
    public GameObject RateOfFireUI;
    public Image RateOfFireSlide;
    public Image healthBorder;
    public  CurrentCurrencyDisplay currentCurrencyDisplay;

    public Image characterAvatar;

    static bool currencyDisplayInProgress = false;

    void Awake()
    {
        if (current)
            Destroy(current);
        current = this;

        characterAvatar.sprite = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.FullAvatar;

        current.gameOver.SetActive(false);
        current.restartButton.gameObject.SetActive(false);
        current.homeButton.gameObject.SetActive(false);
        current.continueButton.gameObject.SetActive(false);
        current.winUI.gameObject.SetActive(false);
        current.winPointsText.gameObject.SetActive(false);
        current.bossBar.gameObject.SetActive(false);
        current.homeWin.gameObject.SetActive(false);
        current.restartWin.gameObject.SetActive(false);
        current.backButton.gameObject.SetActive(false);
        current.buyButton.gameObject.SetActive(false);
        current.shopUI.gameObject.SetActive(false);


        UpdateScoreUI();
        UpdateBombsUI();
        if (isMobile())
        { MobileCanvas.SetActive(true); }
        else
        { MobileCanvas.SetActive(false); }
    }


    [DllImport("__Internal")]
    private static extern bool IsMobile();
    public static bool isMobile()
    { 
#if !UNITY_EDITOR && UNITY_WEBGL
        return IsMobile();
#endif
        return false;
    }

    private void SetInitialAlpha(Image image, float alpha)
    {
        Color tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }

    public static void UpdateScoreUI()
    {
        if (current == null)
            return;

        current.scoreText.SetText(GameManager.GetScore().ToString());
        current.loseScore.SetText(GameManager.GetScore().ToString());
    }

    public static void DisplayCurrency()
    {
        if (current == null || currencyDisplayInProgress)
            return;

        currencyDisplayInProgress = true;


        // Set initial alpha to 0f
        CanvasGroup canvasGroup = current.CurrentCurrency.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = current.Currency.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;

        // Use DOTween to fade in the Currency UI over 1 second
        canvasGroup.DOFade(1f, 1f)
            .OnKill(() =>
            {
                // After fade-in is complete, use DOTween to fade out the Currency UI after 3 seconds
                canvasGroup.DOFade(0f, 1f).SetDelay(10f)
                    .OnComplete(() =>
                    {
                        currencyDisplayInProgress = false;
                    });
            });
    }

    public static void TriggerRateOfFireUI(float duration){
        current.RateOfFireSlide.DOFillAmount(0f, duration)
    .From(1f)
    .SetEase(Ease.InOutCubic)
    .OnComplete(() => {
        current.RateOfFireUI.SetActive(false);
    })
    .OnStart(() => {
        current.RateOfFireUI.SetActive(true);
    });

    }

    private static void SetInitialAlpha(GameObject gameObject, float alpha)
{
    CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
    if (canvasGroup == null)
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    canvasGroup.alpha = alpha;
}


    public static void UpdateBossHealthUI(float health, float maxHealth)
    {
        if (current == null)
            return;

        current.bossBar.gameObject.SetActive(true);
        current.bossBarParent.SetActive(true);
        float fillAmount = health / maxHealth;
        current.bossBar.DOFillAmount(fillAmount, 0.5f);
    }

    public static void HideBossHealthBar()
    {
        if (current == null)
            return;

        current.bossBar.gameObject.SetActive(false);
        current.bossBarParent.SetActive(false);
    }

    public static void UpdateBombsUI()
    {
        if (current == null)
            return;

        current.bombs.SetText(GameManager.GetBombs().ToString());
    }

    public static void UpdateAmmoUI()
    {
        if (current == null)
            return;

        if (GameManager.getAmmo() == 0)
        {
            current.ammoText.SetText("oo");
        }
        else
        {
            current.ammoText.SetText(GameManager.getAmmo().ToString());
        }
    }

    public static void DisplayGameOverText()
{
    if (current == null)
        return;

    // Check if the game over condition is met (you may need to customize this condition based on your game logic)
    bool isGameOver = true;

    if (isGameOver)
    {
        current.gameOver.gameObject.SetActive(true);
    }
}

    private void OnCollisionEnter(Collision collision)
{
    if (collision.collider.CompareTag("Water Dead"))
    {
        DisableGameOver();
    }
    else
    {
        DisplayGameOverText(); 
    }
}

    public static void DisableGameOver()
{
    if (current == null)
        return;

    // Check if the game over condition is met (you may need to customize this condition based on your game logic)
    bool isGameOver = true;

    if (isGameOver)
    {
        current.gameOver.gameObject.SetActive(false);
    }
}

    public static void Home()
    {
        if (current == null)
            return;

        current.homeButton.gameObject.SetActive(true);
    }

    public static void Restart()
    {
        if (current == null)
            return;

        current.restartButton.gameObject.SetActive(true);
    }

    public static void Continue()
    {
        if (current == null)
            return;
        
        current.continueButton.gameObject.SetActive(true);
        
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
        if (CurrencyManager.Instance.spendFRG(1)) { GameManager.GetPlayer().GetComponent<MainPlayer>().Revive(); }

        GameManager.Revive();
        DisplayCurrency();
    }

    public static void AddBackButton()
    {
        current.shopUI.gameObject.SetActive(false);
        current.backButton.gameObject.SetActive(false);
        current.buyButton.gameObject.SetActive(false);

        DisplayCurrency();
    }

    public static void AddShopButton()
    {
        current.shopUI.gameObject.SetActive(true);
        current.backButton.gameObject.SetActive(true);
        current.buyButton.gameObject.SetActive(true);

        DisplayCurrency();
    }

    public static void refreshCurrency()
    {
        current.currentCurrencyDisplay.refresh();
    }

    public static void DisableReviveUI()
    {
        current.continueButton.gameObject.SetActive(false);
        current.restartButton.gameObject.SetActive(false);
        current.homeButton.gameObject.SetActive(false);
        current.gameOver.gameObject.SetActive(false);
    }

    public static void UpdateHealthUI(float health, float maxHealth)
    {
        if (current == null)
            return;

        float fillAmount = health / maxHealth;
        current.healthBar.DOFillAmount(fillAmount, 0.5f);
        /*current.healthBorder.DOColor(Color.Lerp(Color.white, Color.red, 1 - fillAmount), 0.5f)
            .OnComplete(() =>
            {
                current.healthBorder.DOColor(Color.white, 0.5f);
            });*/
    }

    public static void DisplayWinUI()
    {
        if (current == null)
            return;
        Debug.Log("Line 332");
        current.winUI.gameObject.SetActive(true);
        Debug.Log("Line 334");
        current.winPointsText.SetText(GameManager.GetScore().ToString());
        Debug.Log("Line 336");
        current.winPointsText.gameObject.SetActive(true);
        Debug.Log("Line 338");
        current.homeWin.gameObject.SetActive(true);
        Debug.Log("Line 340");
        current.restartWin.gameObject.SetActive(true);
        Debug.Log("Line 342");
    }

    public void BuyAmmo()
    {
        if (CurrencyManager.Instance.spendFRG(1f))
        {
            GameManager.addAmmo(150);
        }
        DisplayCurrency();
    }
}
