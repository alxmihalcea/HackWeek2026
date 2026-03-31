package com.example.jwxunityadsbridge.webview

import android.annotation.SuppressLint
import android.app.Activity
import android.graphics.Color
import android.view.ViewGroup
import android.webkit.WebView
import android.widget.FrameLayout
import android.webkit.JavascriptInterface
import android.webkit.WebViewClient

interface WebViewListener {
    fun onWebViewLoaded()
    fun onWebviewClosed()
}

class WebViewHandler(private val activity: Activity) {
    private var webView: WebView? = null
    private var listener: WebViewListener? = null
    private var isPageLoaded = false

    public fun setListener(listener: WebViewListener) {
        this.listener = listener
    }

    @SuppressLint("SetJavaScriptEnabled")
    public fun load() {
        activity.runOnUiThread {
            if (webView != null) return@runOnUiThread

            webView = WebView(activity).apply {
                layoutParams = FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT
                )

                setBackgroundColor(Color.WHITE)

                settings.javaScriptEnabled = true
                settings.domStorageEnabled = true

                addJavascriptInterface(WebAppBridge(), "AndroidBridge")

                webViewClient = object : WebViewClient() {
                    override fun onPageFinished(view: WebView?, url: String?) {
                        isPageLoaded = true
                        // TODO this event should wait for the sax sdk to be initialised and the ad to be loaded once we have sax sdk implemented
                        listener?.onWebViewLoaded()
                    }
                }

                loadUrl("file:///android_asset/index.html")
            }
        }
    }

    public fun render() {
        activity.runOnUiThread {
            activity.addContentView(
                webView,
                FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT
                )
            )
        }
    }

    public fun showCloseButton() {
        activity.runOnUiThread {
            if (!isPageLoaded) return@runOnUiThread
            webView?.evaluateJavascript("showCloseButton();", null)
        }
    }

    fun close() {
        activity.runOnUiThread {
            webView?.let {
                (it.parent as? ViewGroup)?.removeView(it)
                it.destroy()
            }
            webView = null
            isPageLoaded = false
            listener?.onWebviewClosed()
        }
    }

    private inner class WebAppBridge {
        @JavascriptInterface
        fun closeWebView() {
            close()
        }

        @JavascriptInterface
        fun onWebviewError(error: String) {
            TODO("Handle webview error")
        }
    }
}