package com.example.jwxunityadsbridge.adhandler

import com.example.jwxunityadsbridge.webview.WebViewHandler
import com.example.jwxunityadsbridge.webview.WebViewListener
import android.app.Activity

interface AdHandlerListener {
    fun onAdLoaded()
    fun onAdStopped()

    fun onWebViewLog(log: String)
}

open class AdHandler(activity: Activity) : WebViewListener {
    private val listeners = mutableListOf<AdHandlerListener>()

    protected var webViewHandler: WebViewHandler = WebViewHandler(activity);
    private var isWebViewLoaded = false
    private var isShowing = false

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
        if (isShowing) {
            return
        }
        isShowing = true

        webViewHandler.render()
    }

    override fun onWebViewLoaded() {
        isWebViewLoaded = true
        listeners.forEach { it.onAdLoaded() }
    }

    override fun onWebviewClosed() {
        isWebViewLoaded = false
        isShowing = false
        listeners.forEach { it.onAdStopped() }
    }

    override fun onWebViewLog(log: String) {
        listeners.forEach { it.onWebViewLog(log) }
    }
}
