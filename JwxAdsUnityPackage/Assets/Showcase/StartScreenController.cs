using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace JwxAdsSDK
{
    public class StartScreenController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button playButton;
        [Header("Scene")]
        [SerializeField] private string gameSceneName = "GameScene";

        private bool waitingForRewarded;

        private void OnEnable()
        {
            JwxAdsManager.OnRewardedClosed += HandleRewardedClosed;
            JwxAdsManager.OnRewardedFailedToShow += HandleRewardedFailedToShow;
        }

        private void OnDisable()
        {
            JwxAdsManager.OnRewardedClosed -= HandleRewardedClosed;
            JwxAdsManager.OnRewardedFailedToShow -= HandleRewardedFailedToShow;
        }

        private void Awake()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(Play);
            }
        }

        private void Start()
        {
            JwxAdsManager.InitializeAds();
            JwxAdsManager.LoadRewardedAd();
        }

        public void Play()
        {
            if (string.IsNullOrWhiteSpace(gameSceneName))
            {
                Debug.LogError("StartScreenController: game scene name is empty.");
                return;
            }

            if (!waitingForRewarded)
            {
                waitingForRewarded = true;
                if (playButton != null)
                {
                    playButton.interactable = false;
                }
                JwxAdsManager.ShowRewardedAd();
                return;
            }

            SceneManager.LoadScene(gameSceneName);
        }

        private void HandleRewardedClosed()
        {
            if (!waitingForRewarded)
            {
                return;
            }

            waitingForRewarded = false;
            SceneManager.LoadScene(gameSceneName);
        }

        private void HandleRewardedFailedToShow(string errorMessage)
        {
            if (!waitingForRewarded)
            {
                return;
            }

            waitingForRewarded = false;
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
