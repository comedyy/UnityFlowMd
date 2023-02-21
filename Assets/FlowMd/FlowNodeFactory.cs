using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class FlowNodeFactory
{
    public static FlowNode Create(string nodeType, string name, string title, MethodInfo method)
    {
        if(nodeType == "start") return new StartFlowNode(name, title, method);
        if(nodeType == "end") return new EndFlowNode(name, title, method);
        if(nodeType == "condition") return new ConditionFlowNode(name, title, method);
        if(nodeType == "operation") return new OperationFlowNode(name, title, method);
        throw new System.Exception($"unknown NodeType {name}");
    }
}
