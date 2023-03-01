using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class OperationFlowNode : FlowNode
{
    public OperationFlowNode(string name, string title, MethodInfo method) : base(name, title, method)
    {
    }

    public override FlowNode CloneNode()
    {
        return new OperationFlowNode(this.name, this.title, this.methodInfo);
    }
}

