using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingAnimation : MonoBehaviour
{
    public static LoadingAnimation Instance { get; private set; }

    [SerializeField] private GameObject loadingAnimScene;

    // Define fade duration
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void toggleLoading()
    {
        loadingAnimScene.SetActive(true);
        // Fade in animation
        loadingAnimScene.GetComponent<CanvasGroup>().DOFade(1f, fadeDuration);
    }

    public void stopLoading()
    {
        // Fade out animation
        loadingAnimScene.GetComponent<CanvasGroup>().DOFade(0f, fadeDuration).OnComplete(() => {
            loadingAnimScene.SetActive(false);
        });
    }
}
