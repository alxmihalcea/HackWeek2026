using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button playButton;
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Awake()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(Play);
        }
    }

    private void Start()
    {
        AdsManager.Initialize();
        AdsManager.LoadInterstitialAd();
        AdsManager.LoadRewardedAd();
    }

    public void Play()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("StartScreenController: game scene name is empty.");
            return;
        }
        

        if (playButton != null)
        {
            playButton.interactable = false;
        }

        if (!AdsManager.IsInterstitialLoaded)
        {
            AdsManager.LoadInterstitialAd();
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        AdsManager.ShowInterstitialThen(() => SceneManager.LoadScene(gameSceneName));
    }
}
