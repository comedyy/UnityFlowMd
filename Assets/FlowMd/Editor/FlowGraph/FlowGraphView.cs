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

        foreach(var x in flow.AllNodes)
        {
            var editorNode = EditorNodeFactory.Create(x);
            _nodeMap[x] = editorNode;
            AllNodes.Add(editorNode);

            // x.OnExitEvent -= OnNodeExit;
            // x.OnExitEvent += OnNodeExit;
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

            var nextFlow = GetNextFlow(flow, i, FlowDefine.PORT_DEFULT);
            if(nextFlow != null)
            {
                var editorNodeNext = _nodeMap[nextFlow];
                var edge = (editorNode.outputContainer.Children().First() as Port).ConnectTo(editorNodeNext.inputContainer.Children().First() as Port);
                AddElement(edge);
            }
            
            if(x.nodeType == FlowDefine.CONDITION_NODE_STR)
            {
                var nextFlowIndexNo = GetNextFlow(flow, i, FlowDefine.CONDITION_NO);
                var flowNo = _nodeMap[nextFlowIndexNo];
                var edgeNo = (editorNode.outputContainer.Children().Last() as Port).ConnectTo(flowNo.inputContainer.Children().First() as Port);
                AddElement(edgeNo);
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

    Vector2[] dirsNormal = new Vector2[]{new Vector2(200, 0), new Vector2(0, -200)};
    Vector2[] dirsRotate = new Vector2[]{new Vector2(0, -200), new Vector2(200, 0)};

    private void SetNodePos(Flow flow, int nodeIndex, Vector2 pos, List<int> lst)
    {
        if(nodeIndex < 0) return;
        if(lst.Contains(nodeIndex)) return;

        var node = flow.AllNodes[nodeIndex];
        var editorNode = _nodeMap[node];

        editorNode.SetPosition(new Rect(pos, new Vector2(100, 150)));
        lst.Add(nodeIndex);

        var nodePortDirChange = false;
        var dirs = nodePortDirChange ? dirsRotate : dirsNormal;

        // children
        var nextNodeIndex = flow.GetNextNode(nodeIndex, FlowDefine.PORT_DEFULT);
        if(nextNodeIndex >= 0)
        {
            SetNodePos(flow, nextNodeIndex, pos + dirs[0], lst);
        }

        var nextNodeNoIndex = flow.GetNextNode(nodeIndex, FlowDefine.CONDITION_NO);
        if(nextNodeNoIndex >= 0)
        {
            SetNodePos(flow, nextNodeNoIndex, pos + dirs[1], lst);
        }
    }
}