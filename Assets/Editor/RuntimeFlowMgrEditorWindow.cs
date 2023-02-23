using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RuntimeFlowMgrEditorWindow : EditorWindow
{
    [MenuItem("Flow/OpenFlowMgr")]
    public static void OpenFlowMgr()
    {
        EditorWindow.GetWindow(typeof(RuntimeFlowMgrEditorWindow));
    }

    void OnGUI()
    {
        if(!Application.isPlaying) return;

        if(FlowMgr.Instance == null)
        {
            return;
        }

        EditorGUILayout.BeginVertical();
        foreach(var x in FlowMgr.Instance.AllFlow)
        {
            DrawOnFlow(x);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawOnFlow(Flow x)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(x.Title);
        if(GUILayout.Button("打开"))
        {
            var window = EditorWindow.GetWindow<StoryGraph>();
            window.titleContent = new GUIContent(x.Title);
            window.OpenWithFlow(x);
        }
        EditorGUILayout.EndHorizontal();
    }
}
