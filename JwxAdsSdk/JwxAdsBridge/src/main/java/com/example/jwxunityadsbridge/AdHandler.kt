package com.example.jwxunityadsbridge

import android.app.Activity

interface AdHandlerListener {
    fun onAdLoaded()
}

open class AdHandler(activity: Activity) : WebViewListener {
    private val listeners = mutableListOf<AdHandlerListener>()

    protected var webViewHandler: WebViewHandler = WebViewHandler(activity);
    private var isWebViewLoaded = false

    init {
        webViewHandler.setListener(this)
    }

    fun addListener(listener: AdHandlerListener) {
        listeners += listener
    }

    fun removeListener(listener: AdHandlerListener) {
        listeners -= listener
    }

    public fun loadAd() {
        webViewHandler.load()
    }

    open fun renderAd() {
        if (!isWebViewLoaded) {
            // TODO("Send webview not loaded event")
            return
        }

        isWebViewLoaded = false

        webViewHandler.render()
    }

    override fun onWebViewLoaded() {
        isWebViewLoaded = true
    }
}
