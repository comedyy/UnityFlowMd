using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class OperationFlowNode : FlowNode
{
    public OperationFlowNode(string name, string title, MethodInfo method) : base(name, title, method)
    {
    }
}

