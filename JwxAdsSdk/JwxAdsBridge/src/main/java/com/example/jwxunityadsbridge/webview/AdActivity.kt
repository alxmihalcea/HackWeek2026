package com.example.jwxunityadsbridge.webview

import android.annotation.SuppressLint
import android.app.Activity
import android.graphics.Color
import android.os.Bundle
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.webkit.ConsoleMessage
import android.webkit.JavascriptInterface
import android.webkit.WebChromeClient
import android.webkit.WebResourceError
import android.webkit.WebResourceRequest
import android.webkit.WebResourceResponse
import android.webkit.WebView
import android.webkit.WebViewClient
import android.widget.FrameLayout

class AdActivity : Activity() {
    private var webView: WebView? = null

    @SuppressLint("SetJavaScriptEnabled")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        window.setFlags(
            WindowManager.LayoutParams.FLAG_HARDWARE_ACCELERATED,
            WindowManager.LayoutParams.FLAG_HARDWARE_ACCELERATED
        )
        window.decorView.setLayerType(View.LAYER_TYPE_HARDWARE, null)

        val handlerId = intent?.getIntExtra("webview_id", -1) ?: -1
        val handler = WebViewHandler.getHandler(handlerId)
        if (handler == null) {
            finish()
            return
        }

        handler.setCloseAction { finish() }

        webView = handler.getWebView()
        if (webView == null) {
            handler.load()
            webView = handler.getWebView()
        }

        webView?.let { current ->
            current.layoutParams = FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.MATCH_PARENT
            )
            (current.parent as? ViewGroup)?.removeView(current)
            setContentView(current)
            handler.startAdBreakIfReady()
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        webView?.let { (it.parent as? ViewGroup)?.removeView(it) }
        webView = null
    }

    override fun onBackPressed() {
        // Disable back while ad is showing.
    }

    private inner class WebAppBridge {
        @JavascriptInterface
        fun closeWebView() {
            finish()
        }
    }
}
