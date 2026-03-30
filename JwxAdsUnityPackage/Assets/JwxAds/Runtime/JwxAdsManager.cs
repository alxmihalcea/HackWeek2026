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

    private bool isInitialized;
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

    public void Initialize()
    {
        using var activity = AndroidAdsBridge.GetCurrentActivity();
        if (AndroidAdsBridge.TryCallBridge("initialize", out string errorMessage, appId, activity))
        {
            SetMessage("Initialize call sent", false);
            return;
        }

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

    public void HandleInitialized()
    {
        SetMessage("Event: Initialized", false);
        isInitialized = true;
        RaiseInitialized();
    }

    public void HandleInitializationFailed(string errorMessage)
    {
        SetMessage($"Event: Initialization failed: {errorMessage}", true);
        isInitialized = false;
        RaiseInitializationFailed(errorMessage);
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
