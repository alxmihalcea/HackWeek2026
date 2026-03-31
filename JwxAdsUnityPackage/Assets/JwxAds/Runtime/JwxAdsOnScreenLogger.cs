using System.Collections.Generic;
using UnityEngine;

namespace JwxAdsSDK
{
public class JwxAdsOnScreenLogger : MonoBehaviour
{
    private const float LineHeight = 28f;
    private const float Padding = 16f;
    private const float MaxHeightFraction = 0.3f;

    private static JwxAdsOnScreenLogger instance;
    private static readonly List<LogEntry> entries = new List<LogEntry>();
    private Vector2 scrollPosition;
    private bool autoScroll = true;
    private bool isDragging;
    private Vector2 lastDragPosition;

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
        float contentHeightWithPadding = contentHeight + Padding;

        float maxBoxHeight = Mathf.Max(LineHeight + (Padding * 2f), Screen.height * MaxHeightFraction);
        float boxHeight = Mathf.Min(contentHeightWithPadding + (Padding * 2f), maxBoxHeight);
        float yStart = Screen.height - boxHeight - Padding;
        if (yStart < Padding)
        {
            yStart = Padding;
        }

        var boxRect = new Rect(Padding, yStart, width, boxHeight);

        Color previousColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.6f);
        GUI.Box(boxRect, GUIContent.none);
        GUI.color = previousColor;

        float viewHeight = boxHeight - (Padding * 2f);
        var viewRect = new Rect(Padding * 2f, yStart + Padding, width - (Padding * 3f), viewHeight);
        var contentRect = new Rect(0f, 0f, viewRect.width, contentHeightWithPadding);

        if (HandleDragScroll(boxRect, contentHeight, viewHeight) || HandleTouchScroll(boxRect, contentHeight, viewHeight))
        {
            autoScroll = false;
        }

        scrollPosition = GUI.BeginScrollView(viewRect, scrollPosition, contentRect, GUIStyle.none, GUIStyle.none);

        float y = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            float entryHeight = Mathf.Max(LineHeight, style.CalcHeight(new GUIContent(entries[i].Message), viewRect.width));
            style.normal.textColor = GetEntryColor(entries[i].Kind);
            GUI.Label(new Rect(0f, y, viewRect.width, entryHeight), entries[i].Message, style);
            y += entryHeight;
        }

        if (Event.current.type == EventType.ScrollWheel && viewRect.Contains(Event.current.mousePosition))
        {
            scrollPosition.y += Event.current.delta.y * 25f;
            scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max(0f, contentHeight - viewHeight));
            autoScroll = false;
            Event.current.Use();
        }

        float maxScroll = Mathf.Max(0f, contentHeightWithPadding - viewHeight);
        if (scrollPosition.y >= maxScroll - 2f)
        {
            autoScroll = true;
        }

        if (autoScroll && !isDragging && Input.touchCount == 0 && Event.current.type == EventType.Repaint)
        {
            scrollPosition.y = maxScroll;
        }

        GUI.EndScrollView();
    }

    private bool HandleDragScroll(Rect viewRect, float contentHeight, float viewHeight)
    {
        var currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseDown && viewRect.Contains(currentEvent.mousePosition))
        {
            isDragging = true;
            lastDragPosition = currentEvent.mousePosition;
            currentEvent.Use();
            return true;
        }

        if (currentEvent.type == EventType.MouseDrag && isDragging)
        {
            var delta = currentEvent.mousePosition - lastDragPosition;
            scrollPosition.y -= delta.y * 1.25f;
            scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max(0f, contentHeight - viewHeight));
            lastDragPosition = currentEvent.mousePosition;
            currentEvent.Use();
            return true;
        }

        if (currentEvent.type == EventType.MouseUp && isDragging)
        {
            isDragging = false;
            currentEvent.Use();
            return true;
        }

        return false;
    }

    private bool HandleTouchScroll(Rect viewRect, float contentHeight, float viewHeight)
    {
        if (Input.touchCount == 0)
        {
            return false;
        }

        var touch = Input.GetTouch(0);
        var touchPosition = new Vector2(touch.position.x, Screen.height - touch.position.y);

        if (touch.phase == TouchPhase.Began && viewRect.Contains(touchPosition))
        {
            isDragging = true;
            lastDragPosition = touchPosition;
            return true;
        }

        if (touch.phase == TouchPhase.Moved && isDragging)
        {
            var delta = touchPosition - lastDragPosition;
            scrollPosition.y -= delta.y * 1.5f;
            scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max(0f, contentHeight - viewHeight));
            lastDragPosition = touchPosition;
            return true;
        }

        if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isDragging)
        {
            isDragging = false;
            return true;
        }

        return false;
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
