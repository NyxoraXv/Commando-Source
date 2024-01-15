using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float health;
    public bool immortal;
    private bool isPlayer;

    public delegate void OnDamageEvent(float damage);
    public OnDamageEvent onHit;
    public OnDamageEvent onDead;

    [SerializeField] private Image bloodDripImage;
    [SerializeField] private float bloodDripDuration = 1f;
    private bool isShowingBloodDrip = false;

    private void Start()
    {
        isPlayer = GetComponent<PlayerController>() != null;
        var difficulty = GameManager.GetDifficultyMode();

        if (!isPlayer)
        {
            if (difficulty == GameManager.Difficulty.Easy)
                maxHealth *= 0.7f;
            else if (difficulty == GameManager.Difficulty.Hard)
                maxHealth *= 1.3f;
        }

        health = maxHealth;

        // Initialize blood drip image
        if (bloodDripImage != null)
        {
            InitializeBloodDrip();
        }
    }

    private void InitializeBloodDrip()
    {
        var canvasGroup = bloodDripImage.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Set initial alpha to 0 (invisible)
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public void IncreaseHealth(float heal)
    {
        health += heal;
        health = Mathf.Min(health, maxHealth);
        UIManager.UpdateHealthUI(health, maxHealth);
    }

    public void Hit(float damage)
    {
        if (!IsAlive() || GameManager.IsGameOver() || immortal)
            return;

        if (isPlayer)
        {
            var difficulty = GameManager.GetDifficultyMode();
            if (difficulty == GameManager.Difficulty.Easy)
                damage *= 0.7f;
            else if (difficulty == GameManager.Difficulty.Hard)
                damage *= 1.3f;
        }

        health -= damage;
        onHit?.Invoke(damage);

        if (!IsAlive())
        {
            onDead?.Invoke(damage);
            ShowBloodDrip();
        }
        else
        {
            ShowBloodDrip(); // Optionally show blood drip when hit
        }
    }

    public void Revive()
    {
        if (!IsAlive())
        {
            health = maxHealth;
            UIManager.UpdateHealthUI(health, maxHealth);
            HideBloodDrip();
        }
    }

    private void ShowBloodDrip()
    {
        if (bloodDripImage != null && !isShowingBloodDrip)
        {
            isShowingBloodDrip = true;

            var canvasGroup = bloodDripImage.GetComponent<CanvasGroup>();
            canvasGroup.DOFade(0.6f, bloodDripDuration)
                .OnComplete(() =>
                {
                    isShowingBloodDrip = false;
                    HideBloodDrip();
                });
        }
    }

    private void HideBloodDrip()
    {
        if (bloodDripImage != null)
        {
            var canvasGroup = bloodDripImage.GetComponent<CanvasGroup>();
            canvasGroup.DOFade(0f, bloodDripDuration);
        }
    }
}
