using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ViewGraphFromTextAsset
{
    [MenuItem("Assets/ViewGraphFromTextAsset")]
    public static void Func()
    {
        var x = Selection.activeObject as TextAsset;
        if(x == null)
        {
            Debug.LogError("当前单位不是textAsset");
            return;
        }

        StoryGraph.asset = x;
        var window = EditorWindow.GetWindow<StoryGraph>();
        window.titleContent = new GUIContent(x.name);
    }
}
