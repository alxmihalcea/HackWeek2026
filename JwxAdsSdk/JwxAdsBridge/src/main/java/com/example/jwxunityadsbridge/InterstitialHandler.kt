package com.example.jwxunityadsbridge

import android.app.Activity
import android.os.Handler
import android.os.Looper

class InterstitialHandler(activity: Activity) : AdHandler(activity) {
    val timerHandler = Handler(Looper.getMainLooper())
    val timeUntilShowSkipButton = 15_000L

    override fun renderAd() {
        super.renderAd()

        timerHandler.postDelayed({
            webViewHandler.showCloseButton()
        }, timeUntilShowSkipButton)
    }
}
