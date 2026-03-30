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

    public static bool InitializeAds()
    {
        if (Instance == null)
        {
            JwxAdsOnScreenLogger.LogError("JwxAds: No instance found. Add JwxAds to a GameObject in the scene.");
            return false;
        }

        return Instance.Initialize();
    }

    public static bool LoadRewardedAd()
    {
        if (Instance == null)
        {
            JwxAdsOnScreenLogger.LogError("JwxAds: No instance found. Add JwxAds to a GameObject in the scene.");
            return false;
        }

        return Instance.LoadRewardedAdInternal();
    }

    public static bool ShowRewardedAd()
    {
        if (Instance == null)
        {
            JwxAdsOnScreenLogger.LogError("JwxAds: No instance found. Add JwxAds to a GameObject in the scene.");
            return false;
        }

        return Instance.ShowRewardedAdInternal();
    }

    public bool Initialize()
    {
        if (AndroidAdsBridge.TryCallBridge("initialize", appId, out string errorMessage))
        {
            SetMessage("Initialize call sent", false);
            return true;
        }

        SetMessage("ERROR: " + errorMessage, true);
        RaiseInitializationFailed(errorMessage);
        return false;
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

    private bool LoadRewardedAdInternal()
    {
        if (!EnsureInitialized())
        {
            return false;
        }

        string resolvedPlacementId = rewardedPlacementId;
        if (AndroidAdsBridge.TryCallBridge("loadRewarded", resolvedPlacementId, out string errorMessage))
        {
            SetMessage("Load rewarded call sent", false);
            return true;
        }

        SetMessage("ERROR: " + errorMessage, true);
        RaiseRewardedFailedToLoad(errorMessage);
        return false;
    }

    private bool ShowRewardedAdInternal()
    {
        if (!EnsureInitialized())
        {
            return false;
        }

        string resolvedPlacementId = rewardedPlacementId;
        if (AndroidAdsBridge.TryCallBridge("showRewarded", resolvedPlacementId, out string errorMessage))
        {
            SetMessage("Show rewarded call sent", false);
            return true;
        }

        SetMessage("ERROR: " + errorMessage, true);
        RaiseRewardedFailedToShow(errorMessage);
        return false;
    }

    private bool EnsureInitialized()
    {
        if (isInitialized)
        {
            return true;
        }

        return Initialize();
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
