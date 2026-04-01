package com.example.jwxunityadsbridge.webview

import android.annotation.SuppressLint
import android.app.Activity
import android.content.Intent
import android.graphics.Color
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.webkit.ConsoleMessage
import android.webkit.WebView
import android.widget.FrameLayout
import android.webkit.JavascriptInterface
import android.webkit.WebChromeClient
import android.webkit.WebResourceError
import android.webkit.WebResourceRequest
import android.webkit.WebResourceResponse
import android.webkit.WebViewClient

interface WebViewListener {
    fun onWebViewLoaded()
    fun onWebviewClosed()

    fun onWebViewLog(log: String)
}

class WebViewHandler(private val activity: Activity) {
    private var webView: WebView? = null
    private var listener: WebViewListener? = null
    private var isPageLoaded = false
    private var pendingShow = false
    private val handlerId: Int
    private var closeAction: (() -> Unit)? = null

    companion object {
        private const val DefaultUrl =
            "https://assets.connatix.com/Elements/0a34019a-f275-4aac-a280-55114dffd5e4/hackweek_webview_html.html"
        private var activeUrl: String = DefaultUrl
        private var nextId = 1
        private val handlers = mutableMapOf<Int, WebViewHandler>()

        fun getHandler(id: Int): WebViewHandler? = handlers[id]

        fun getUrl(): String = activeUrl

        fun setUrl(url: String) {
            activeUrl = url
        }
    }

    init {
        handlerId = nextId++
        handlers[handlerId] = this
    }

    public fun setListener(listener: WebViewListener) {
        this.listener = listener
    }

    public fun setCloseAction(action: (() -> Unit)?) {
        closeAction = action
    }

    @SuppressLint("SetJavaScriptEnabled")
    public fun load() {
        activity.runOnUiThread {
            if (webView != null) return@runOnUiThread

            WebView.setWebContentsDebuggingEnabled(true)

            activity.window.setFlags(
                WindowManager.LayoutParams.FLAG_HARDWARE_ACCELERATED,
                WindowManager.LayoutParams.FLAG_HARDWARE_ACCELERATED
            )
            activity.window.decorView.setLayerType(View.LAYER_TYPE_HARDWARE, null)

            val created = WebView(activity).apply {
                layoutParams = FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT
                )

                setBackgroundColor(Color.WHITE)
                setLayerType(View.LAYER_TYPE_HARDWARE, null)

                settings.javaScriptEnabled = true
                settings.domStorageEnabled = true
                settings.mediaPlaybackRequiresUserGesture = false
                settings.loadsImagesAutomatically = true
                settings.allowFileAccess = true
                settings.allowContentAccess = true
                settings.javaScriptCanOpenWindowsAutomatically = true
                settings.useWideViewPort = true
                settings.loadWithOverviewMode = true
                settings.mixedContentMode = android.webkit.WebSettings.MIXED_CONTENT_ALWAYS_ALLOW

                listener?.onWebViewLog("WebView HW: ${this.isHardwareAccelerated}")
                listener?.onWebViewLog("Window HW: ${activity.window.decorView.isHardwareAccelerated}")

                requestFocus()
                requestFocusFromTouch()

                addJavascriptInterface(WebAppBridge(), "AndroidBridge")

                webViewClient = object : WebViewClient() {
                    override fun onPageFinished(view: WebView?, url: String?) {
                        isPageLoaded = true
                        // TODO this event should wait for the sax sdk to be initialised and the ad to be loaded once we have sax sdk implemented
                        listener?.onWebViewLoaded()
                        if (pendingShow) {
                            startAdBreakIfReady()
                        }
                    }

                    override fun onReceivedError(
                        view: WebView?,
                        request: WebResourceRequest?,
                        error: WebResourceError?
                    ) {
                        listener?.onWebViewLog(
                            "WebView error: url=${request?.url}, code=${error?.errorCode}, desc=${error?.description}"
                        )
                    }

                    override fun onReceivedHttpError(
                        view: WebView?,
                        request: WebResourceRequest?,
                        errorResponse: WebResourceResponse?
                    ) {
                        listener?.onWebViewLog(
                            "HTTP error: url=${request?.url}, status=${errorResponse?.statusCode}, reason=${errorResponse?.reasonPhrase}"
                        )
                    }
                }

                webChromeClient = object : WebChromeClient() {
                    override fun onConsoleMessage(consoleMessage: ConsoleMessage): Boolean {
                        listener?.onWebViewLog(
                            "WebView [${consoleMessage.messageLevel()}] ${consoleMessage.message()} " +
                                    "(${consoleMessage.sourceId()}:${consoleMessage.lineNumber()})"
                        )
                        return true
                    }
                }

                loadUrl(WebViewHandler.getUrl())
            }
            webView = created
        }
    }

    public fun render() {
        activity.runOnUiThread {
            val intent = Intent(activity, AdActivity::class.java)
            intent.putExtra("webview_id", handlerId)
            pendingShow = true
            activity.startActivity(intent)
            startAdBreakIfReady()
        }
    }

    fun getWebView(): WebView? = webView

    fun startAdBreakIfReady() {
        if (!isPageLoaded) return
        webView?.evaluateJavascript("startAdBreak()", null)
        pendingShow = false
    }

    public fun showCloseButton() {
        activity.runOnUiThread {
            if (!isPageLoaded) return@runOnUiThread
            webView?.evaluateJavascript("showCloseButton();", null)
        }
    }

    public fun showSkipButton() {
        activity.runOnUiThread {
            if (!isPageLoaded) return@runOnUiThread
            webView?.evaluateJavascript("showSkipButton()", null)
        }
    }

    fun close() {
        activity.runOnUiThread {
            webView?.let { (it.parent as? ViewGroup)?.removeView(it) }

            webView = null
            isPageLoaded = false
            listener?.onWebviewClosed()
        }
    }

    private inner class WebAppBridge {
        @JavascriptInterface
        fun closeWebView() {
            closeAction?.invoke()
            close()
        }

        @JavascriptInterface
        fun onWebviewError(error: String) {
            TODO("Handle webview error")
        }
    }
}