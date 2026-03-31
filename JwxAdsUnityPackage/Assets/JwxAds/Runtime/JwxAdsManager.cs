using System;
using UnityEngine;

namespace JwxAdsSDK
{
    public class JwxAdsManager : MonoBehaviour
    {
        [Header("SDK Settings")]
        [SerializeField] private string appId = "demo-app-id";

        [Header("Placements")]
        [SerializeField] private string rewardedPlacementId = "rewarded-demo";
        [SerializeField] private string interstitialPlacementId = "interstitial-demo";

        [Header("Debug")]
        [SerializeField] private bool debugLogging = false;

        public static JwxAdsManager Instance { get; private set; }

        public static event Action OnInitialized;
        public static event Action<string> OnInitializationFailed;
        public static event Action OnRewardedLoaded;
        public static event Action<string> OnRewardedFailedToLoad;
        public static event Action OnRewardedShown;
        public static event Action<string> OnRewardedFailedToShow;
        public static event Action OnRewardedClosed;
        public static event Action OnRewardedEarned;

        public static event Action OnInterstitialLoaded;
        public static event Action<string> OnInterstitialFailedToLoad;
        public static event Action OnInterstitialShown;
        public static event Action<string> OnInterstitialFailedToShow;
        public static event Action OnInterstitialClosed;

        private bool isInitialized;
        private bool initializeInProgress;
        private AdsListenerProxy listenerProxy;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _ = appId;
            _ = rewardedPlacementId;
        }
#endif

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            isInitialized = false;
            RegisterListener();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                UnregisterListener();
                Instance = null;
            }
        }

        public static void InitializeAds()
        {
            if (Instance == null)
            {
                JwxAdsOnScreenLogger.LogError("JwxAds: No instance found. Add JwxAds to a GameObject in the scene.");
                return;
            }

            Instance.Initialize();
        }

        public static void LoadRewardedAd()
        {
            if (Instance == null)
            {
                JwxAdsOnScreenLogger.LogError("JwxAds: No instance found. Add JwxAds to a GameObject in the scene.");
                return;
            }

            Instance.LoadRewardedAdInternal();
        }

        public static void ShowRewardedAd()
        {
            if (Instance == null)
            {
                JwxAdsOnScreenLogger.LogError("JwxAds: No instance found. Add JwxAds to a GameObject in the scene.");
                return;
            }

            Instance.ShowRewardedAdInternal();
        }

         public static void LoadInterstitialAd()
        {
            if (Instance == null)
            {
                JwxAdsOnScreenLogger.LogError("JwxAds: No instance found. Add JwxAds to a GameObject in the scene.");
                return;
            }

            Instance.LoadInterstitialAdInternal();
        }

        public static void ShowInterstitialAd()
        {
            if (Instance == null)
            {
                JwxAdsOnScreenLogger.LogError("JwxAds: No instance found. Add JwxAds to a GameObject in the scene.");
                return;
            }

            Instance.ShowInterstitialAdInternal();
        }

        public void Initialize()
        {
            if (isInitialized || initializeInProgress)
            {
                return;
            }

            initializeInProgress = true;
            using var activity = AndroidAdsBridge.GetCurrentActivity();
            if (AndroidAdsBridge.TryCallBridge("initialize", out string errorMessage, appId, activity))
            {
                SetMessage("Initialize call sent", false);
                return;
            }

            initializeInProgress = false;
            SetMessage("ERROR: " + errorMessage, true);
        }

        public void HandleRewardedClosed()
        {
            SetMessage("Event: Rewarded closed", false);
            RaiseRewardedClosed();
        }

        public void HandleRewardedEarned()
        {
            SetMessage("Event: Rewarded earned", false);
            RaiseRewardedEarned();
        }

        public void HandleRewardedLoaded()
        {
            SetMessage("Event: Rewarded loaded", false);
            RaiseRewardedLoaded();
        }

        public void HandleRewardedFailedToLoad(string errorMessage)
        {
            SetMessage($"Event: Rewarded failed to load: {errorMessage}", true);
            RaiseRewardedFailedToLoad(errorMessage);
        }

        public void HandleRewardedShown()
        {
            SetMessage("Event: Rewarded shown", false);
            RaiseRewardedShown();
        }

        public void HandleRewardedFailedToShow(string errorMessage)
        {
            SetMessage($"Event: Rewarded failed to show: {errorMessage}", true);
            RaiseRewardedFailedToShow(errorMessage);
        }

        public void HandleInterstitialLoaded()
        {
            SetMessage("Event: Interstitial loaded", false);
            RaiseInterstitialLoaded();
        }

        public void HandleInterstitialFailedToLoad(string errorMessage)
        {
            SetMessage($"Event: Interstitial failed to load: {errorMessage}", true);
            RaiseInterstitialFailedToLoad(errorMessage);
        }

        public void HandleInterstitialShown()
        {
            SetMessage("Event: Interstitial shown", false);
            RaiseInterstitialShown();
        }

        public void HandleInterstitialFailedToShow(string errorMessage)
        {
            SetMessage($"Event: Interstitial failed to show: {errorMessage}", true);
            RaiseInterstitialFailedToShow(errorMessage);
        }

        public void HandleInterstitialClosed()
        {
            SetMessage("Event: Interstitial closed", false);
            RaiseInterstitialClosed();
        }

        public void HandleInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            SetMessage("Event: Initialized", false);
            isInitialized = true;
            initializeInProgress = false;
            RaiseInitialized();
        }

        public void HandleInitializationFailed(string errorMessage)
        {
            SetMessage($"Event: Initialization failed: {errorMessage}", true);
            isInitialized = false;
            initializeInProgress = false;
            RaiseInitializationFailed(errorMessage);
        }

        public void HandleLog(string message)
        {
            SetMessage($"Event: Log: {message}", false);
        }

        private void LoadRewardedAdInternal()
        {
            if (!isInitialized)
            {
                SetMessage("SDK not initialized yet. Sending initialize call.", true);
                Initialize();
                return;
            }

            string resolvedPlacementId = rewardedPlacementId;
            if (AndroidAdsBridge.TryCallBridge("loadRewarded", out string errorMessage, resolvedPlacementId))
            {
                SetMessage("Load rewarded call sent", false);
                return;
            }

            SetMessage("ERROR: " + errorMessage, true);
        }

        private void ShowRewardedAdInternal()
        {
            if (!isInitialized)
            {
                SetMessage("SDK not initialized yet. Sending initialize call.", true);
                Initialize();
                return;
            }

            string resolvedPlacementId = rewardedPlacementId;
            if (AndroidAdsBridge.TryCallBridge("showRewarded", out string errorMessage, resolvedPlacementId))
            {
                SetMessage("Show rewarded call sent", false);
                return;
            }

            SetMessage("ERROR: " + errorMessage, true);
        }
        private void LoadInterstitialAdInternal()
        {
            if (!isInitialized)
            {
                SetMessage("SDK not initialized yet. Sending initialize call.", true);
                Initialize();
                return;
            }

            string resolvedPlacementId = interstitialPlacementId;
            if (AndroidAdsBridge.TryCallBridge("loadInterstitial", out string errorMessage, resolvedPlacementId))
            {
                SetMessage("Load interstitial call sent", false);
                return;
            }
            SetMessage("ERROR: " + errorMessage, true);
        }

        private void ShowInterstitialAdInternal()
        {
            if (!isInitialized)
            {
                SetMessage("SDK not initialized yet. Sending initialize call.", true);
                Initialize();
                return;
            }

            string resolvedPlacementId = interstitialPlacementId;
            if (AndroidAdsBridge.TryCallBridge("showInterstitial", out string errorMessage, resolvedPlacementId))
            {
                SetMessage("Show interstitial call sent", false);
                return;
            }
            SetMessage("ERROR: " + errorMessage, true);
        }
        private void RaiseInitialized()
        {
            OnInitialized?.Invoke();
        }

        private void RaiseInitializationFailed(string errorMessage)
        {
            OnInitializationFailed?.Invoke(errorMessage);
        }

        private void RaiseRewardedLoaded()
        {
            OnRewardedLoaded?.Invoke();
        }

        private void RaiseRewardedFailedToLoad(string errorMessage)
        {
            OnRewardedFailedToLoad?.Invoke(errorMessage);
        }

        private void RaiseRewardedShown()
        {
            OnRewardedShown?.Invoke();
        }

        private void RaiseRewardedFailedToShow(string errorMessage)
        {
            OnRewardedFailedToShow?.Invoke(errorMessage);
        }

        private void RaiseRewardedClosed()
        {
            OnRewardedClosed?.Invoke();
        }

        private void RaiseRewardedEarned()
        {
            OnRewardedEarned?.Invoke();
        }

        private void RaiseInterstitialLoaded()
        {
            OnInterstitialLoaded?.Invoke();
        }

        private void RaiseInterstitialFailedToLoad(string errorMessage)
        {
            OnInterstitialFailedToLoad?.Invoke(errorMessage);
        }

        private void RaiseInterstitialShown()
        {
            OnInterstitialShown?.Invoke();
        }

        private void RaiseInterstitialFailedToShow(string errorMessage)
        {
            OnInterstitialFailedToShow?.Invoke(errorMessage);
        }

        private void RaiseInterstitialClosed()
        {
            OnInterstitialClosed?.Invoke();
        }

        private void SetMessage(string message, bool isError)
        {
            if (!debugLogging)
            {
                return;
            }

            if (isError)
            {
                JwxAdsOnScreenLogger.LogError(message);
                return;
            }

            JwxAdsOnScreenLogger.Log(message);
        }

        private void RegisterListener()
        {
            if (listenerProxy != null)
            {
                return;
            }

            listenerProxy = new AdsListenerProxy(this);
            AndroidAdsBridge.RegisterListener(listenerProxy);
        }

        private void UnregisterListener()
        {
            AndroidAdsBridge.UnregisterListener();
            listenerProxy = null;
        }
    }
}
