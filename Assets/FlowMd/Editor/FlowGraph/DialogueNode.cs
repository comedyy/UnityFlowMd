using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;

public class EditorNodeFactory
{
    public static FlowEditorNode Create(FlowNodeAsset node, ConnectionsInfo info)
    {
        FlowEditorNode editorNode = null;
        if(node.nodeType == FlowDefine.START_NODE_STR) editorNode = new StartEditorFlowNode();
        else if(node.nodeType == FlowDefine.END_NODE_STR) editorNode = new EndEditorFlowNode();
        else if(node.nodeType == FlowDefine.OPERATION_NODE_STR) editorNode = new OperationEditorFlowNode();
        else if(node.nodeType == FlowDefine.CONDITION_NODE_STR) editorNode = new ConditionEditorFlowNode();
        else if(node.nodeType == FlowDefine.INPUTOUTPUT_NODE_STR) editorNode = new InputOutputEditorlowNode();
        else throw new Exception($"node not implement {node}");

        editorNode.Init(node.title, info);

        return editorNode;
    }
}

public class FlowEditorNode : Node
{
    public void Init(string title, ConnectionsInfo info)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        this.title = title;

        // Port
        var inputPort = this is StartEditorFlowNode ? 0 : 1;
        for(int i = 0; i < inputPort; i++)
        {
            var portInput = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            portInput.portName = "";
            inputContainer.Add(portInput);
        }

        for(int i = 0; i < info.ports.Count; i++)
        {
            var portOutputYes = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            portOutputYes.portName = "";
            outputContainer.Add(portOutputYes);

            portOutputYes.portName = info.ports[i].portName;
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
}

public class StartEditorFlowNode : FlowEditorNode
{
    
}

public class OperationEditorFlowNode : FlowEditorNode
{
    
}

public class EndEditorFlowNode : FlowEditorNode
{
}

public class InputOutputEditorlowNode : FlowEditorNode
{
}
