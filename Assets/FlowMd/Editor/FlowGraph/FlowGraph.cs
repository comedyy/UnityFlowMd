using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FlowGraph : EditorWindow
{
    private FlowGraphView _graphView;

    public void OpenWithAsset(TextAsset x)
    {
        var flowAsset = new FlowAsset(x);
        var flow = Flow.Instantiate(flowAsset, "");
        OpenWithFlow(flow);
    }

    public void OpenWithFlow(Flow flow)
    {
        _graphView = new FlowGraphView(flow)
        {
            name = flow.Title,
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);

        GenerateMiniMap();
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap {anchored = true};
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
        miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        _graphView.Add(miniMap);
    }

    private void OnDisable()
    {
        if(_graphView != null)
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}