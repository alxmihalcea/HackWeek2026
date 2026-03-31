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
                    }
                }

                loadDataWithBaseURL(
                    null,
                    """
                    <html>
                        <head>
                            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                        </head>
                        <body style="
                            margin:0;
                            display:flex;
                            flex-direction: column;
                            justify-content:center;
                            align-items:center;
                            gap: 100px;
                            height:100vh;
                            background:white;
                            font-family:sans-serif;
                            position:relative;
                        ">
                            

                            <h1>Hello World!</h1>
                            
                            <button
                                id="close-button"
                                onclick="closeAd()"
                                style="
                                    border: 4px solid black;
                                    border-radius: 10px;
                                    padding: 10px 20px;
                                    background-color: white;
                                    font-size: 18px;
                                    font-weight: 500;
                                    display: none
                                "
                            >
                                Close
                            </button>

                            <script>
                                function closeAd() {
                                    if (window.AndroidBridge && window.AndroidBridge.closeWebView) {
                                        window.AndroidBridge.closeWebView();
                                    }
                                }
                                
                                function showCloseButton() {
                                    const closeButton = document.getElementById("close-button")
                                    if (closeButton) {
                                        closeButton.style.display = "block"
                                    }
                                }
                            </script>
                        </body>
                    </html>
                    """.trimIndent(),
                    "text/html",
                    "UTF-8",
                    null
                )
            }
        }

        // TODO this event should wait for the sax sdk to be initialised and the ad to be loaded once we have sax sdk implemented
        listener?.onWebViewLoaded()
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
        }
    }

    private inner class WebAppBridge {
        @JavascriptInterface
        fun closeWebView() {
            close()
        }
    }
}