package com.example.jwxunityadsbridge.adhandler

import android.app.Activity
import android.os.Handler
import android.os.Looper

class RewardedHandler(activity: Activity) : AdHandler(activity) {
    val timerHandler = Handler(Looper.getMainLooper())
    val timeUntilShowSkipButton = 5000L

    override fun renderAd() {
        super.renderAd()

        timerHandler.postDelayed({
            webViewHandler.showCloseButton()
        }, timeUntilShowSkipButton)
    }
}
