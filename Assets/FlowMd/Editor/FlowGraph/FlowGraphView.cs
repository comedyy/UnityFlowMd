using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FlowGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
    public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);

    Dictionary<FlowNodeAsset, FlowEditorNode> _nodeMap = new Dictionary<FlowNodeAsset, FlowEditorNode>();
    List<FlowEditorNode> AllNodes = new List<FlowEditorNode>();

    void OnNodeEnter(FlowNodeAsset node)
    {
        foreach(var x in AllNodes)
        {
            x.selected = false;
        }

        var editorNode = _nodeMap[node];
        editorNode.selected = true;
    }

    public FlowGraphView(Flow flow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        for(int i = 0; i < flow.AllNodes.Count; i++)
        {
            var x = flow.AllNodes[i];
            var editorNode = EditorNodeFactory.Create(x, flow.asset._allConnection[i]);
            _nodeMap[x] = editorNode;
            AllNodes.Add(editorNode);

            flow.OnEnterEvent -= OnNodeEnter;
            flow.OnEnterEvent += OnNodeEnter;
        }

        foreach(var x in AllNodes)
        {
            AddElement(x);
        }

        List<int> lst = new List<int>();
        SetNodePos(flow, flow.EntryIndex, new Vector2(100, 100), lst);

        for(int i = 0; i< flow.AllNodes.Count; i++)
        {
            var x = flow.AllNodes[i];
            var editorNode = _nodeMap[x];

            var ports = flow.asset._allConnection[i].ports;
            for (int j = 0; j < ports.Count; j++)
            {
                var item = ports[j];
                var nextNodeIndex = item.nextIndex;
                var nextFlow = GetNextFlow(flow, i, item.portName);

                var editorNodeNext = _nodeMap[nextFlow];
                var edge = (editorNode.outputContainer.ElementAt(j) as Port).ConnectTo(editorNodeNext.inputContainer.Children().First() as Port);
                AddElement(edge);
            }
        }

        if(flow.CurrentAsset != null)
        {
            OnNodeEnter(flow.CurrentAsset);
        }
    }

    FlowNodeAsset GetNextFlow(Flow flow, int index, string result)
    {
        var nextIndex = flow.GetNextNode(index, result);
        return nextIndex >= 0 ? flow.asset._allNodes[nextIndex] : null;
    }

    private void SetNodePos(Flow flow, int nodeIndex, Vector2 pos, List<int> lst)
    {
        if(nodeIndex < 0) return;
        if(lst.Contains(nodeIndex)) return;

        var node = flow.AllNodes[nodeIndex];
        var editorNode = _nodeMap[node];

        editorNode.SetPosition(new Rect(pos, new Vector2(100, 150)));
        lst.Add(nodeIndex);

        var ports = flow.asset._allConnection[nodeIndex].ports;
        var segmentCount = (ports.Count - 1) / 2f;

        for (int i = 0; i < ports.Count; i++)
        {
            var item = ports[i];
            var nextNodeIndex = item.nextIndex;
            var diff = new Vector2(200, (segmentCount - i) * (-100f));
            SetNodePos(flow, nextNodeIndex, pos + diff, lst);
        }

    }
}