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

    public void onRewardedLoaded(string placementId)
    {
        manager.HandleRewardedLoaded();
    }

    public void onRewardedFailedToLoad(string error)
    {
        manager.HandleRewardedFailedToLoad(error);
    }

    public void onRewardedShown(string placementId)
    {
        manager.HandleRewardedShown();
    }

    public void onRewardedFailedToShow(string error)
    {
        manager.HandleRewardedFailedToShow(error);
    }

    public void onRewardedClosed(string placementId)
    {
        manager.HandleRewardedClosed();
    }

    public void onRewardedEarned(string placementId)
    {
        manager.HandleRewardedEarned();
    }

    public void onLog(string message)
    {
        manager.HandleLog(message);
    }
}
}
