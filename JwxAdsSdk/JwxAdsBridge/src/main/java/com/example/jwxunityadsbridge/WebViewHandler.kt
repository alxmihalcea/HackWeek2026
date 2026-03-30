package com.example.jwxunityadsbridge

import android.annotation.SuppressLint
import android.app.Activity
import android.graphics.Color
import android.view.ViewGroup
import android.webkit.WebView
import android.widget.FrameLayout
import android.webkit.JavascriptInterface

class WebViewHandler(private val activity: Activity) {
    private var webView: WebView? = null

    @SuppressLint("SetJavaScriptEnabled")
    fun showHelloWorld() {
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
                                onclick="closeAd()"
                                style="
                                    border: 4px solid black;
                                    border-radius: 10px;
                                    padding: 10px 20px;
                                    background-color: white;
                                    font-size: 18px;
                                    font-weight: 500;
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
                            </script>
                        </body>
                    </html>
                    """.trimIndent(),
                    "text/html",
                    "UTF-8",
                    null
                )
            }

            activity.addContentView(
                webView,
                FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT
                )
            )
        }
    }

    fun close() {
        activity.runOnUiThread {
            webView?.let {
                (it.parent as? ViewGroup)?.removeView(it)
                it.destroy()
            }
            webView = null
        }
    }

    private inner class WebAppBridge {
        @JavascriptInterface
        fun closeWebView() {
            close()
        }
    }
}