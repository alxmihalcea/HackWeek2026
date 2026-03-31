using UnityEngine;

namespace JwxAdsSDK
{
public class AdsListenerProxy : AndroidJavaProxy
{
    private readonly JwxAdsManager manager;

    public AdsListenerProxy(JwxAdsManager managerInstance)
        : base(AndroidAdsBridge.ListenerInterfaceName)
    {
        manager = managerInstance;
    }

    public void onInitialized()
    {
        manager.HandleInitialized();
    }

    public void onInitializationFailed(string error)
    {
        manager.HandleInitializationFailed(error);
    }

    public void onRewardedLoaded()
    {
        manager.HandleRewardedLoaded();
    }

    public void onRewardedFailedToLoad(string error)
    {
        manager.HandleRewardedFailedToLoad(error);
    }

    public void onRewardedShown()
    {
        manager.HandleRewardedShown();
    }

    public void onRewardedFailedToShow(string error)
    {
        manager.HandleRewardedFailedToShow(error);
    }

    public void onRewardedClosed()
    {
        manager.HandleRewardedClosed();
    }

    public void onRewardedEarned()
    {
        manager.HandleRewardedEarned();
    }

    public void onInterstitialLoaded()
    {
        manager.HandleInterstitialLoaded();
    }

    public void onInterstitialFailedToLoad(string error)
    {
        manager.HandleInterstitialFailedToLoad(error);
    }

    public void onInterstitialShown()
    {
        manager.HandleInterstitialShown();
    }

    public void onInterstitialFailedToShow(string error)
    {
        manager.HandleInterstitialFailedToShow(error);
    }

    public void onInterstitialClosed()
    {
        manager.HandleInterstitialClosed();
    }

    public void onLog(string message)
    {
        manager.HandleLog(message);
    }
}
}
