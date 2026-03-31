package com.example.jwxunityadsbridge.webview

import android.annotation.SuppressLint
import android.app.Activity
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

    public fun setListener(listener: WebViewListener) {
        this.listener = listener
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

            webView = WebView(activity).apply {
                layoutParams = FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT
                )

                setBackgroundColor(Color.WHITE)

                settings.javaScriptEnabled = true
                settings.domStorageEnabled = true
                settings.mediaPlaybackRequiresUserGesture = false
                settings.loadsImagesAutomatically = true
                settings.allowFileAccess = true
                settings.allowContentAccess = true
                settings.mixedContentMode = android.webkit.WebSettings.MIXED_CONTENT_ALWAYS_ALLOW
                settings.javaScriptCanOpenWindowsAutomatically = true

                setLayerType(View.LAYER_TYPE_HARDWARE, null)
                requestFocus()
                requestFocusFromTouch()

                addJavascriptInterface(WebAppBridge(), "AndroidBridge")

                webViewClient = object : WebViewClient() {
                    override fun onPageFinished(view: WebView?, url: String?) {
                        isPageLoaded = true
                        // TODO this event should wait for the sax sdk to be initialised and the ad to be loaded once we have sax sdk implemented
                        listener?.onWebViewLoaded()
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

                loadUrl("https://assets.connatix.com/Elements/0a34019a-f275-4aac-a280-55114dffd5e4/hackweek_webview_html.html")
            }
        }
    }

    public fun render() {
        activity.runOnUiThread {
            val currentWebView = webView ?: return@runOnUiThread
            if (currentWebView.parent != null) return@runOnUiThread
            val rootView = activity.findViewById<ViewGroup>(android.R.id.content)
            rootView.addView(
                currentWebView,
                FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT
                )
            )
            currentWebView.visibility = View.VISIBLE
            currentWebView.bringToFront()
        }
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