using JwxAdsSDK;
using UnityEngine;

public class JwxAdsShowcase : MonoBehaviour
{
    private string statusMessage = "Waiting for events...";

    private void OnEnable()
    {
        JwxAdsManager.OnInitialized += HandleInitialized;
        JwxAdsManager.OnInitializationFailed += HandleInitializationFailed;
        JwxAdsManager.OnRewardedLoaded += HandleRewardedLoaded;
        JwxAdsManager.OnRewardedFailedToLoad += HandleRewardedFailedToLoad;
        JwxAdsManager.OnRewardedShown += HandleRewardedShown;
        JwxAdsManager.OnRewardedFailedToShow += HandleRewardedFailedToShow;
        JwxAdsManager.OnRewardedClosed += HandleRewardedClosed;
        JwxAdsManager.OnRewardedEarned += HandleRewardedEarned;
    }

    private void OnDisable()
    {
        JwxAdsManager.OnInitialized -= HandleInitialized;
        JwxAdsManager.OnInitializationFailed -= HandleInitializationFailed;
        JwxAdsManager.OnRewardedLoaded -= HandleRewardedLoaded;
        JwxAdsManager.OnRewardedFailedToLoad -= HandleRewardedFailedToLoad;
        JwxAdsManager.OnRewardedShown -= HandleRewardedShown;
        JwxAdsManager.OnRewardedFailedToShow -= HandleRewardedFailedToShow;
        JwxAdsManager.OnRewardedClosed -= HandleRewardedClosed;
        JwxAdsManager.OnRewardedEarned -= HandleRewardedEarned;
    }

    private void OnGUI()
    {
        float buttonWidth = 420f;
        float buttonHeight = 110f;
        float x = (Screen.width - buttonWidth) * 0.5f;

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 26,
            wordWrap = true
        };

        if (GUI.Button(new Rect(x, 140, buttonWidth, buttonHeight), "Initialize Ads"))
        {
            JwxAdsManager.InitializeAds();
        }

        if (GUI.Button(new Rect(x, 280, buttonWidth, buttonHeight), "Load Rewarded"))
        {
            JwxAdsManager.LoadRewardedAd();
        }

        if (GUI.Button(new Rect(x, 420, buttonWidth, buttonHeight), "Show Rewarded"))
        {
            JwxAdsManager.ShowRewardedAd();
        }
    }

    private void HandleInitialized()
    {
        statusMessage = "Initialized";
    }

    private void HandleInitializationFailed(string error)
    {
        statusMessage = $"Init failed: {error}";
    }

    private void HandleRewardedLoaded()
    {
        statusMessage = "Rewarded loaded";
    }

    private void HandleRewardedFailedToLoad(string error)
    {
        statusMessage = $"Load failed: {error}";
    }

    private void HandleRewardedShown()
    {
        statusMessage = "Rewarded shown";
    }

    private void HandleRewardedFailedToShow(string error)
    {
        statusMessage = $"Show failed: {error}";
    }

    private void HandleRewardedClosed()
    {
        statusMessage = "Rewarded closed";
    }

    private void HandleRewardedEarned()
    {
        statusMessage = "Reward earned";
    }
}
