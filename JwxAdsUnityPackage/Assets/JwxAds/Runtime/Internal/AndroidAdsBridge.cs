using System;
using UnityEngine;

namespace JwxAdsSDK
{
    public static class AndroidAdsBridge
    {
        private const string AdsBridgeClassName = "com.example.jwxunityadsbridge.AdsBridge";
        private const string AdsListenerInterfaceName = "com.example.jwxunityadsbridge.AdsListener";
        private const string NotRunningOnAndroidMessage = "Not running on Android device";

        public static string ListenerInterfaceName => AdsListenerInterfaceName;
        public static bool TryCallBridge(string methodName, out string errorMessage, params object[] arguments)
        {
    #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var adsBridgeClass = new AndroidJavaClass(AdsBridgeClassName);
                errorMessage = null;
                adsBridgeClass.CallStatic(methodName, arguments);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
    #else
            errorMessage = NotRunningOnAndroidMessage;
            return false;
    #endif
        }

        public static AndroidJavaObject GetCurrentActivity()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#else
            return null;
#endif
        }


        public static void RegisterListener(AdsListenerProxy proxy)
        {
    #if UNITY_ANDROID && !UNITY_EDITOR
            using var adsBridgeClass = new AndroidJavaClass(AdsBridgeClassName);
            adsBridgeClass.CallStatic("setListener", proxy);
    #endif
        }

        public static void UnregisterListener()
        {
    #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var adsBridgeClass = new AndroidJavaClass(AdsBridgeClassName);
                adsBridgeClass.CallStatic("setListener", null);
            }
            catch (Exception)
            {
                // Ignore cleanup failures during shutdown.
            }
    #endif
        }
    }
}
