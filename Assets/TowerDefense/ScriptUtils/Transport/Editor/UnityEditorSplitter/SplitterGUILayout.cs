#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public static class SplitterGUILayout 
{
    static public float splitterWidth = 5;

    static Dictionary<string, Data> datas = new Dictionary<string, Data>();
    static List<Data> queue = new List<Data>();

    class Data
    {
        // public float width;
        public bool dragging = false;
        public Rect splitterRect;
        public float Value = -1; // current width/height
        public Rect firstPaneRect;
        public Vector2 scrollLeft, scrollRight;
    }

    // give a unique key
    static public void DrawHorizontal(string key, float startingWidth, Action leftPane, Action rightPane)
    {
        if(!datas.ContainsKey(key))
            datas[key] = new Data() { Value = startingWidth };

        queue.Add(datas[key]);

        GUILayout.BeginHorizontal(); // master layout
        var data = datas[key];

        // left pane layout
        data.firstPaneRect = EditorGUILayout.BeginHorizontal(
            GUILayout.Width(data.Value),
            GUILayout.MaxWidth(data.Value),
            GUILayout.MinWidth(data.Value));

        using (var scroll = new GUILayout.ScrollViewScope( data.scrollLeft )) {
            data.scrollLeft = scroll.scrollPosition;
            leftPane.Invoke();
        }
        SplitHorizontal();
        using (var scroll = new GUILayout.ScrollViewScope( data.scrollRight )) {
            data.scrollRight = scroll.scrollPosition;
            rightPane();
        }
        EndHorizontal();
    }
    static public void DrawVertical(string key, float height, Action topPane, Action bottomPane)
    {
        if(!datas.ContainsKey(key))
            datas[key] = new Data() { Value = height };

        queue.Add(datas[key]);

        GUILayout.BeginVertical(); // master layout
        var data = datas[key];

        // left pane layout
        data.firstPaneRect = EditorGUILayout.BeginVertical(
            GUILayout.Height(data.Value),
            GUILayout.MaxHeight(data.Value),
            GUILayout.MinHeight(data.Value));

        topPane();
        SplitVertical();
        bottomPane();
        EndVertical();
    }

    // adds the splitter between the two panes. you shouldn't call this more that once in a splitter layout
    static void SplitHorizontal(GUIStyle splitterBoxStyle = null)
    {
        // left pane
        EditorGUILayout.EndHorizontal();

        var dat = queue.Last();
        dat.splitterRect = EditorGUILayout.BeginHorizontal(
            GUILayout.Width(splitterWidth),
            GUILayout.MaxWidth(splitterWidth),
            GUILayout.MinWidth(splitterWidth)
        );

        GUILayout.Box(GUIContent.none, splitterBoxStyle ?? GUI.skin.button, 
            GUILayout.ExpandHeight(true),
            GUILayout.Width(splitterWidth),
            GUILayout.MaxWidth(splitterWidth),
            GUILayout.MinWidth(splitterWidth));
        
        EditorGUILayout.EndHorizontal();

        // right pane
        EditorGUILayout.BeginHorizontal();
    }
    // adds the splitter between the two panes. you shouldn't call this more that once in a splitter layout
    static void SplitVertical(GUIStyle splitterBoxStyle = null)
    {
        // left pane
        EditorGUILayout.EndVertical();

        var dat = queue.Last();
        dat.splitterRect = EditorGUILayout.BeginVertical(
            GUILayout.Height(splitterWidth),
            GUILayout.MaxHeight(splitterWidth),
            GUILayout.MinHeight(splitterWidth)
        );

        GUILayout.Box(GUIContent.none, splitterBoxStyle ?? GUI.skin.button, 
            GUILayout.ExpandWidth(true),
            GUILayout.Height(splitterWidth),
            GUILayout.MaxHeight(splitterWidth),
            GUILayout.MinHeight(splitterWidth));
        
        EditorGUILayout.EndVertical();

        // right pane
        EditorGUILayout.BeginVertical();
    }
    

    static void EndHorizontal()
    {
        // right pane
        GUILayout.EndHorizontal();
        // master pane
        GUILayout.EndHorizontal();

        // Splitter events
        if (Event.current == null) return;

        var data = queue.Last();

        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (data.splitterRect.Contains(Event.current.mousePosition))
                {
                    data.dragging = true;
                }
                break;
            case EventType.MouseDrag:
                if (data.dragging)
                {
                    if(data.Value == -1)
                        data.Value = data.firstPaneRect.width;
                    data.Value += Event.current.delta.x;
                    UnityEditor.EditorWindow.focusedWindow.Repaint();
                }
                break;
            case EventType.MouseUp:
                if (data.dragging)
                {
                    data.dragging = false;
                }
                break;
        }

        EditorGUIUtility.AddCursorRect(queue.Last().splitterRect, MouseCursor.ResizeHorizontal);

        queue.RemoveAt(queue.Count - 1); // remove last ( most recent )
    }
    static void EndVertical()
    {
        // right pane
        GUILayout.EndVertical();
        // master pane
        GUILayout.EndVertical();

        // Splitter events
        if (Event.current == null) return;

        var data = queue.Last();

        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (data.splitterRect.Contains(Event.current.mousePosition))
                {
                    data.dragging = true;
                }
                break;
            case EventType.MouseDrag:
                if (data.dragging)
                {
                    if(data.Value == -1)
                        data.Value = data.firstPaneRect.width;
                    data.Value += Event.current.delta.y;
                    UnityEditor.EditorWindow.focusedWindow.Repaint();
                }
                break;
            case EventType.MouseUp:
                if (data.dragging)
                {
                    data.dragging = false;
                }
                break;
        }

        EditorGUIUtility.AddCursorRect(queue.Last().splitterRect, MouseCursor.ResizeVertical);

        queue.RemoveAt(queue.Count - 1); // remove last ( most recent )
    }


}
#endif