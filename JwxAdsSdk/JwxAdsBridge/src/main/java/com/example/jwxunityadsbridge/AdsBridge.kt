package com.example.jwxunityadsbridge

import android.app.Activity

interface AdsListener {
    fun onInitialized()
    fun onInitializationFailed(error: String)
    fun onRewardedLoaded()
    fun onRewardedFailedToLoad(error: String)
    fun onRewardedShown()
    fun onRewardedFailedToShow(error: String)
    fun onRewardedClosed()
    fun onRewardedEarned()
    fun onInterstitialLoaded()
    fun onInterstitialFailedToLoad(error: String)
    fun onInterstitialShown()
    fun onInterstitialFailedToShow(error: String)
    fun onInterstitialClosed()
    fun onLog(message: String)
}

class AdsBridge {
    companion object {
        private var initialized = false
        private var loadedPlacementId: String? = null
        private var listener: AdsListener? = null

        private var activity: Activity? = null
        private var rewardedWebViewHandler: WebViewHandler? = null
        private var interstitialWebViewHandler: WebViewHandler? = null

        @JvmStatic
        fun setListener(adsListener: AdsListener?) {
            listener = adsListener
        }

        @JvmStatic
        fun initialize(appId: String, activity: Activity) {
            sendLog("initialization started")

            Companion.activity = activity
            rewardedWebViewHandler = WebViewHandler(activity)
            interstitialWebViewHandler = WebViewHandler(activity)

            initialized = appId.isNotBlank()

            if (initialized) {
                android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                    listener?.onInitialized()
                }, 2000L)
            } else {
                val error = "Initialization failed: appId is empty"
                listener?.onInitializationFailed(error)
            }
        }

        @JvmStatic
        fun loadRewarded(placementId: String) {
            if (!initialized) {
                val error = "Load failed: SDK not initialized"
                listener?.onRewardedFailedToLoad(error)
                return
            }
            if (placementId.isBlank()) {
                val error = "Load failed: placementId is empty"
                listener?.onRewardedFailedToLoad(error)
                return
            }
            if (rewardedWebViewHandler == null) {
                val error = "Load failed: WebView not initialized"
                listener?.onRewardedFailedToLoad(error)
                return;
            }

            loadedPlacementId = placementId
            rewardedWebViewHandler!!.load()

            android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                listener?.onRewardedLoaded()
            }, 2000L)
        }

        @JvmStatic
        fun showRewarded(placementId: String) {
            if (!initialized) {
                val error = "Show failed: SDK not initialized"
                listener?.onRewardedFailedToShow(error)
                return
            }
            if (loadedPlacementId != placementId) {
                val error = "Show failed: placement not loaded"
                listener?.onRewardedFailedToShow(error)
                return
            }
            if (rewardedWebViewHandler == null) {
                val error = "Show failed: WebView not initialized"
                listener?.onRewardedFailedToShow(error)
                return
            }

            rewardedWebViewHandler!!.render()

            android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                listener?.onRewardedShown()
            }, 2000L)

            android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                // Simulare pentru demo
                listener?.onRewardedEarned()
                listener?.onRewardedClosed()
            }, 5000L)
        }

        @JvmStatic
        fun loadInterstitial(placementId: String) {
            if (!initialized) {
                val error = "Load failed: SDK not initialized"
                listener?.onInterstitialFailedToLoad(error)
                return
            }
            if (placementId.isBlank()) {
                val error = "Load failed: placementId is empty"
                listener?.onInterstitialFailedToLoad(error)
                return
            }
            if (interstitialWebViewHandler == null) {
                val error = "Load failed: WebView not initialized"
                listener?.onRewardedFailedToLoad(error)
                return;
            }

            loadedPlacementId = placementId
            interstitialWebViewHandler!!.load()

            android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                listener?.onInterstitialLoaded()
            }, 2000L)
        }

        @JvmStatic
        fun showInterstitial(placementId: String) {
            if (!initialized) {
                val error = "Show failed: SDK not initialized"
                listener?.onInterstitialFailedToShow(error)
                return
            }
            if (loadedPlacementId != placementId) {
                val error = "Show failed: placement not loaded"
                listener?.onInterstitialFailedToShow(error)
                return
            }
            if (interstitialWebViewHandler == null) {
                val error = "Show failed: WebView not initialized"
                listener?.onInterstitialFailedToShow(error)
                return
            }

            interstitialWebViewHandler!!.render();

            android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                listener?.onInterstitialShown()
            }, 2000L)

            android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                // Simulare pentru demo
                listener?.onInterstitialClosed()
            }, 5000L)
        }

        fun sendLog(message: String) {
            listener?.onLog(message)
        }
    }
}