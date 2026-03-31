using System.Collections.Generic;
using UnityEngine;

namespace JwxAdsSDK
{
public class JwxAdsOnScreenLogger : MonoBehaviour
{
    private const int MaxEntries = 12;
    private const float LineHeight = 28f;
    private const float Padding = 16f;
    private const float MaxHeightFraction = 0.2f;

    private static JwxAdsOnScreenLogger instance;
    private static readonly List<LogEntry> entries = new List<LogEntry>(MaxEntries);
    private Vector2 scrollPosition;

    private enum LogKind
    {
        Default,
        Event,
        Error
    }

    private struct LogEntry
    {
        public string Message;
        public LogKind Kind;
    }

    public static void Log(string message)
    {
        EnsureInstance();
        AddEntry(message, LogKind.Default);
    }

    public static void LogError(string message)
    {
        EnsureInstance();
        AddEntry(message, LogKind.Error);
    }

    public static void LogEvent(string message)
    {
        EnsureInstance();
        AddEntry(message, LogKind.Event);
    }

    private static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        GameObject loggerObject = new GameObject("JwxAdsLogs");
        instance = loggerObject.AddComponent<JwxAdsOnScreenLogger>();
        DontDestroyOnLoad(loggerObject);
    }

    private static void AddEntry(string message, LogKind kind)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        if (entries.Count >= MaxEntries)
        {
            entries.RemoveAt(0);
        }

        entries.Add(new LogEntry { Message = message, Kind = kind });
    }

    private void OnGUI()
    {
        if (entries.Count == 0)
        {
            return;
        }

        float width = Screen.width - (Padding * 2f);

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            wordWrap = true
        };

        float contentHeight = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            contentHeight += Mathf.Max(LineHeight, style.CalcHeight(new GUIContent(entries[i].Message), width));
        }

        float maxBoxHeight = Mathf.Max(LineHeight + (Padding * 2f), Screen.height * MaxHeightFraction);
        float boxHeight = Mathf.Min(contentHeight + (Padding * 2f), maxBoxHeight);
        float yStart = Screen.height - boxHeight - Padding;
        if (yStart < Padding)
        {
            yStart = Padding;
        }

        Color previousColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.6f);
        GUI.Box(new Rect(Padding, yStart, width, boxHeight), GUIContent.none);
        GUI.color = previousColor;

        float viewHeight = boxHeight - (Padding * 2f);
        var viewRect = new Rect(Padding * 2f, yStart + Padding, width - (Padding * 3f), viewHeight);
        var contentRect = new Rect(0f, 0f, viewRect.width, contentHeight);
        scrollPosition = GUI.BeginScrollView(viewRect, scrollPosition, contentRect);

        float y = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            float entryHeight = Mathf.Max(LineHeight, style.CalcHeight(new GUIContent(entries[i].Message), viewRect.width));
            style.normal.textColor = GetEntryColor(entries[i].Kind);
            GUI.Label(new Rect(0f, y, viewRect.width, entryHeight), entries[i].Message, style);
            y += entryHeight;
        }

        GUI.EndScrollView();
    }

    private static Color GetEntryColor(LogKind kind)
    {
        return kind switch
        {
            LogKind.Error => Color.red,
            LogKind.Event => new Color(0.0f, 0.6f, 0.0f, 1f),
            _ => Color.white
        };
    }
}
}
