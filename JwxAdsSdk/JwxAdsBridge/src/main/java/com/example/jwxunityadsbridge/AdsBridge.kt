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

        private var rewardedHandler: RewardedHandler? = null
        private var interstitialHandler: InterstitialHandler? = null

        @JvmStatic
        fun setListener(adsListener: AdsListener?) {
            listener = adsListener
        }

        @JvmStatic
        fun initialize(appId: String, activity: Activity) {
            sendLog("initialization started")

            rewardedHandler = RewardedHandler(activity)
            rewardedHandler!!.addListener(object : AdHandlerListener {
                override fun onAdLoaded() {
                    onRewardedAdLoaded()
                }
            })

            interstitialHandler = InterstitialHandler(activity)
            interstitialHandler!!.addListener(object : AdHandlerListener {
                override fun onAdLoaded() {
                    onInterstitialAdLoaded()
                }
            })

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
            if (rewardedHandler == null) {
                val error = "Load failed: WebView not initialized"
                listener?.onRewardedFailedToLoad(error)
                return
            }

            loadedPlacementId = placementId
            rewardedHandler!!.loadAd()
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
            if (rewardedHandler == null) {
                val error = "Show failed: WebView not initialized"
                listener?.onRewardedFailedToShow(error)
                return
            }

            rewardedHandler!!.renderAd()

            listener?.onRewardedShown()

            // Simulare pentru demo
            listener?.onRewardedEarned()
            listener?.onRewardedClosed()
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
            if (interstitialHandler == null) {
                val error = "Load failed: WebView not initialized"
                listener?.onRewardedFailedToLoad(error)
                return;
            }

            loadedPlacementId = placementId
            interstitialHandler!!.loadAd()
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
            if (interstitialHandler == null) {
                val error = "Show failed: WebView not initialized"
                listener?.onInterstitialFailedToShow(error)
                return
            }

            interstitialHandler!!.renderAd();

            listener?.onInterstitialShown()

            // Simulare pentru demo
            listener?.onInterstitialClosed()
        }

        private fun sendLog(message: String) {
            listener?.onLog(message)
        }

        private fun onRewardedAdLoaded() {
            listener?.onRewardedLoaded()
        }

        private fun onInterstitialAdLoaded() {
            listener?.onInterstitialLoaded()
        }
    }
}