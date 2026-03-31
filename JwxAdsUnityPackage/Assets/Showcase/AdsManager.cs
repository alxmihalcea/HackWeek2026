using UnityEngine;

public static class AdsManager
{
    private static bool subscribed;
    private static bool rewardedLoaded;
    private static bool interstitialLoaded;
    private static bool isShowingRewarded;
    private static bool isShowingInterstitial;
    private static bool rewardedShownAtScore;
    private static bool pendingRewardedAtScore;
    private static bool pendingShowRewarded;
    private static bool pendingInterstitialLoad = true;
    private static bool pendingShowInterstitial;
    private static int rewardedScoreThreshold = -1;
    private static System.Action pendingRewardedComplete;
    private static System.Action pendingInterstitialComplete;
    private static bool isInitialized;
    private static bool pendingRewardedLoad = true;
    private static bool initializeCalled;

    public static bool IsRewardedLoaded => rewardedLoaded;
    public static bool IsInterstitialLoaded => interstitialLoaded;

    public static void Initialize()
    {
        EnsureSubscriptions();
        initializeCalled = true;
        JwxAdsSDK.JwxAdsManager.InitializeAds();
    }

    public static void LoadRewardedAd()
    {
        if (!initializeCalled)
        {
            Debug.LogWarning("JwxAds not initialized");
            return;
        }

        if (!isInitialized)
        {
            pendingRewardedLoad = true;
            return;
        }
        JwxAdsSDK.JwxAdsManager.LoadRewardedAd();
    }

    public static void ShowRewardedThen(System.Action onComplete)
    {
        if (!initializeCalled)
        {
            Debug.LogWarning("JwxAds not initialized");
            return;
        }

        if (isShowingRewarded)
        {
            return;
        }

        if (!isInitialized)
        {
            pendingRewardedLoad = true;
            pendingShowRewarded = true;
            pendingRewardedComplete = onComplete;
            return;
        }

        if (!rewardedLoaded)
        {
            pendingShowRewarded = true;
            pendingRewardedComplete = onComplete;
            JwxAdsSDK.JwxAdsManager.LoadRewardedAd();
            return;
        }

        isShowingRewarded = true;
        pendingRewardedComplete = onComplete;
        JwxAdsSDK.JwxAdsManager.ShowRewardedAd();
    }

    public static void LoadInterstitialAd()
    {
        if (!initializeCalled)
        {
            Debug.LogWarning("JwxAds not initialized");
            return;
        }

        if (!isInitialized)
        {
            pendingInterstitialLoad = true;
            return;
        }

        JwxAdsSDK.JwxAdsManager.LoadInterstitialAd();
    }

    public static void ShowInterstitialThen(System.Action onComplete)
    {
        if (!initializeCalled)
        {
            Debug.LogWarning("JwxAds not initialized");
            return;
        }

        if (isShowingInterstitial)
        {
            return;
        }

        if (!isInitialized)
        {
            pendingInterstitialLoad = true;
            pendingShowInterstitial = true;
            pendingInterstitialComplete = onComplete;
            return;
        }

        if (!interstitialLoaded)
        {
            pendingShowInterstitial = true;
            pendingInterstitialComplete = onComplete;
            JwxAdsSDK.JwxAdsManager.LoadInterstitialAd();
            return;
        }

        isShowingInterstitial = true;
        pendingInterstitialComplete = onComplete;
        JwxAdsSDK.JwxAdsManager.ShowInterstitialAd();
    }

    public static void SetRewardedScoreThreshold(int threshold)
    {
        rewardedScoreThreshold = threshold;
    }

    public static void NotifyScore(int score)
    {
        if (!initializeCalled)
        {
            Debug.LogWarning("JwxAds not initialized");
            return;
        }

        if (rewardedScoreThreshold <= 0 || rewardedShownAtScore)
        {
            return;
        }

        if (score < rewardedScoreThreshold)
        {
            return;
        }

        if (isShowingRewarded)
        {
            return;
        }

        if (!isInitialized)
        {
            pendingRewardedAtScore = true;
            pendingRewardedLoad = true;
            return;
        }

        if (rewardedLoaded)
        {
            rewardedShownAtScore = true;
            ShowRewardedThen(null);
            return;
        }

        pendingRewardedAtScore = true;
        JwxAdsSDK.JwxAdsManager.LoadRewardedAd();
    }

    public static void SetInterstitialLoaded(bool loaded)
    {
        interstitialLoaded = loaded;
    }

    private static void EnsureSubscriptions()
    {
        if (subscribed)
        {
            return;
        }

        subscribed = true;
        JwxAdsSDK.JwxAdsManager.OnInitialized += HandleInitialized;
        JwxAdsSDK.JwxAdsManager.OnInitializationFailed += HandleInitializationFailed;
        JwxAdsSDK.JwxAdsManager.OnRewardedLoaded += HandleRewardedLoaded;
        JwxAdsSDK.JwxAdsManager.OnRewardedFailedToLoad += HandleRewardedFailedToLoad;
        JwxAdsSDK.JwxAdsManager.OnRewardedShown += HandleRewardedShown;
        JwxAdsSDK.JwxAdsManager.OnRewardedFailedToShow += HandleRewardedFailedToShow;
        JwxAdsSDK.JwxAdsManager.OnRewardedClosed += HandleRewardedClosed;
        JwxAdsSDK.JwxAdsManager.OnInterstitialLoaded += HandleInterstitialLoaded;
        JwxAdsSDK.JwxAdsManager.OnInterstitialFailedToLoad += HandleInterstitialFailedToLoad;
        JwxAdsSDK.JwxAdsManager.OnInterstitialShown += HandleInterstitialShown;
        JwxAdsSDK.JwxAdsManager.OnInterstitialFailedToShow += HandleInterstitialFailedToShow;
        JwxAdsSDK.JwxAdsManager.OnInterstitialClosed += HandleInterstitialClosed;
    }

    private static void HandleInitialized()
    {
        isInitialized = true;
        if (pendingRewardedLoad)
        {
            pendingRewardedLoad = false;
            JwxAdsSDK.JwxAdsManager.LoadRewardedAd();
        }

        if (pendingInterstitialLoad)
        {
            pendingInterstitialLoad = false;
            JwxAdsSDK.JwxAdsManager.LoadInterstitialAd();
        }
    }

    private static void HandleInitializationFailed(string error)
    {
        isInitialized = false;
    }

    private static void HandleRewardedLoaded()
    {
        rewardedLoaded = true;
        if (pendingShowRewarded && !isShowingRewarded)
        {
            pendingShowRewarded = false;
            ShowRewardedThen(pendingRewardedComplete);
            return;
        }

        if (pendingRewardedAtScore && !rewardedShownAtScore)
        {
            rewardedShownAtScore = true;
            pendingRewardedAtScore = false;
            ShowRewardedThen(null);
        }
    }

    private static void HandleRewardedFailedToLoad(string error)
    {
        rewardedLoaded = false;
        pendingShowRewarded = false;
    }

    private static void HandleRewardedShown()
    {
        rewardedLoaded = false;
    }

    private static void HandleRewardedFailedToShow(string error)
    {
        rewardedLoaded = false;
        pendingShowRewarded = false;
        FinishRewardedFlow();
        LoadRewardedAd();
    }

    private static void HandleRewardedClosed()
    {
        rewardedLoaded = false;
        pendingShowRewarded = false;
        FinishRewardedFlow();
        LoadRewardedAd();
    }

    private static void HandleInterstitialLoaded()
    {
        interstitialLoaded = true;
        if (pendingShowInterstitial && !isShowingInterstitial)
        {
            pendingShowInterstitial = false;
            ShowInterstitialThen(pendingInterstitialComplete);
        }
    }

    private static void HandleInterstitialFailedToLoad(string error)
    {
        interstitialLoaded = false;
        pendingShowInterstitial = false;
    }

    private static void HandleInterstitialShown()
    {
        interstitialLoaded = false;
    }

    private static void HandleInterstitialFailedToShow(string error)
    {
        interstitialLoaded = false;
        pendingShowInterstitial = false;
        FinishInterstitialFlow();
        LoadInterstitialAd();
    }

    private static void HandleInterstitialClosed()
    {
        interstitialLoaded = false;
        pendingShowInterstitial = false;
        FinishInterstitialFlow();
        LoadInterstitialAd();
    }

    private static void FinishRewardedFlow()
    {
        if (!isShowingRewarded)
        {
            return;
        }

        isShowingRewarded = false;
        var callback = pendingRewardedComplete;
        pendingRewardedComplete = null;
        callback?.Invoke();
    }

    private static void FinishInterstitialFlow()
    {
        if (!isShowingInterstitial)
        {
            return;
        }

        isShowingInterstitial = false;
        var callback = pendingInterstitialComplete;
        pendingInterstitialComplete = null;
        callback?.Invoke();
    }
}
