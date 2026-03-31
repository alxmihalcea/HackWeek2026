using System.Collections.Generic;
using UnityEngine;

namespace JwxAdsSDK
{
public class JwxAdsOnScreenLogger : MonoBehaviour
{
    private const int MaxEntries = 12;
    private const float LineHeight = 28f;
    private const float Padding = 16f;

    private static JwxAdsOnScreenLogger instance;
    private static readonly List<LogEntry> entries = new List<LogEntry>(MaxEntries);

    private struct LogEntry
    {
        public string Message;
        public bool IsError;
    }

    public static void Log(string message)
    {
        EnsureInstance();
        AddEntry(message, false);
    }

    public static void LogError(string message)
    {
        EnsureInstance();
        AddEntry(message, true);
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

    private static void AddEntry(string message, bool isError)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        if (entries.Count >= MaxEntries)
        {
            entries.RemoveAt(0);
        }

        entries.Add(new LogEntry { Message = message, IsError = isError });
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

        float boxHeight = contentHeight + (Padding * 2f);
        float yStart = Screen.height - boxHeight - Padding;
        if (yStart < Padding)
        {
            yStart = Padding;
        }

        Color previousColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.6f);
        GUI.Box(new Rect(Padding, yStart, width, boxHeight), GUIContent.none);
        GUI.color = previousColor;

        float y = yStart + Padding;
        for (int i = 0; i < entries.Count; i++)
        {
            float entryHeight = Mathf.Max(LineHeight, style.CalcHeight(new GUIContent(entries[i].Message), width));
            style.normal.textColor = entries[i].IsError ? Color.red : Color.green;
            GUI.Label(new Rect(Padding * 2f, y, width - Padding, entryHeight), entries[i].Message, style);
            y += entryHeight;
        }
    }
}
}
