package com.example.jwxunityadsbridge

import com.unity3d.player.UnityPlayer

interface AdsListener {
    fun onInitialized()
    fun onInitializationFailed(error: String)
    fun onRewardedLoaded()
    fun onRewardedFailedToLoad(error: String)
    fun onRewardedShown()
    fun onRewardedFailedToShow(error: String)
    fun onRewardedClosed()
    fun onRewardedEarned()
    fun onLog(message: String)
}

class AdsBridge {
    companion object {
        private var initialized = false
        private var loadedPlacementId: String? = null
        private var listener: AdsListener? = null

        private var rewardedWebViewHandler: WebViewHandler? = null

        @JvmStatic
        fun setListener(adsListener: AdsListener?) {
            listener = adsListener
        }

        @JvmStatic
        fun initialize(appId: String) {
            sendLog("initialization started")

            val activity = UnityPlayer.currentActivity ?: return
            rewardedWebViewHandler = WebViewHandler(activity)

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
            loadedPlacementId = placementId

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
                val erorr = "Show failed: WebView not initialized"
                listener?.onRewardedFailedToShow(erorr)
                return
            }

            rewardedWebViewHandler!!.showHelloWorld();

            android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                listener?.onRewardedShown()
            }, 2000L)

            android.os.Handler(android.os.Looper.getMainLooper()).postDelayed({
                // Simulare pentru demo
                listener?.onRewardedEarned()
                listener?.onRewardedClosed()
            }, 5000L)
        }

        fun sendLog(message: String) {
            listener?.onLog(message)
        }
    }
}