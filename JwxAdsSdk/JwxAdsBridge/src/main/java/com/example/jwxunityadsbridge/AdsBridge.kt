package com.example.jwxunityadsbridge

import kotlinx.coroutines.*

interface AdsListener {
    fun onInitialized()
    fun onInitializationFailed(error: String)
    fun onRewardedLoaded()
    fun onRewardedFailedToLoad(error: String)
    fun onRewardedShown()
    fun onRewardedFailedToShow(error: String)
    fun onRewardedClosed()
    fun onRewardedEarned()
}

class AdsBridge {
    companion object {
        private var initialized = false
        private var loadedPlacementId: String? = null
        private var listener: AdsListener? = null

        @JvmStatic
        fun setListener(adsListener: AdsListener?) {
            listener = adsListener
        }

        @JvmStatic
        fun initialize(appId: String) {
            initialized = appId.isNotBlank()
            if (initialized) {
                listener?.onInitialized()

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

            listener?.onRewardedShown()
            // Simulare pentru demo
            listener?.onRewardedEarned()
            listener?.onRewardedClosed()
        }
    }
}