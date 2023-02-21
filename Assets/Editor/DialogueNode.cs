using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;

public class EditorNodeFactory
{
    public static FlowEditorNode Create(FlowNode node)
    {
        FlowEditorNode editorNode = null;
        if(node is StartFlowNode) editorNode = new StartEditorFlowNode();
        else if(node is EndFlowNode) editorNode = new EndEditorFlowNode();
        else if(node is OperationFlowNode) editorNode = new OperationEditorFlowNode();
        else if(node is ConditionFlowNode) editorNode = new ConditionEditorFlowNode();
        else throw new Exception($"node not implement {node}");

        editorNode.Init(node.title, node);

        return editorNode;
    }
}

public abstract class FlowEditorNode : Node
{
    public abstract int InputNodeCount{get;}
    public abstract int OutNodeCount{get;}

    public void Init(string title, FlowNode node)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        this.title = title;

        // Port
        for(int i = 0; i < InputNodeCount; i++)
        {
            var portInput = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            portInput.portName = "";
            inputContainer.Add(portInput);
        }

        for(int i = 0; i < OutNodeCount; i++)
        {
            var portOutputYes = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            portOutputYes.portName = "";
            outputContainer.Add(portOutputYes);

            if(OutNodeCount == 2 && i == 0) portOutputYes.portName = "Yes";
            if(OutNodeCount == 2 && i == 1) portOutputYes.portName = "No";
        }

        // capabilities &= ~Capabilities.Movable;
        capabilities &= ~Capabilities.Deletable;

        RefreshExpandedState();
        RefreshPorts();

        Vector2 pos = new Vector2(0, 0);
        SetPosition(new Rect(pos, new Vector2(100, 150)));
    }
}

public class ConditionEditorFlowNode : FlowEditorNode
{
    public override int InputNodeCount => 1;

    public override int OutNodeCount => 2;
}

public class StartEditorFlowNode : FlowEditorNode
{
    public override int InputNodeCount => 0;

    public override int OutNodeCount => 1;
}

public class OperationEditorFlowNode : FlowEditorNode
{
    public override int InputNodeCount => 1;

    public override int OutNodeCount => 1;
}

public class EndEditorFlowNode : FlowEditorNode
{
    public override int InputNodeCount => 1;

    public override int OutNodeCount => 0;
}
