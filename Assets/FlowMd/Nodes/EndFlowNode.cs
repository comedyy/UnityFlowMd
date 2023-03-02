using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EndFlowNode : FlowNode
{
    public EndFlowNode(string name, string title, MethodInfo method) : base(name, title, method)
    {
    }

    public override FlowNode CloneNode()
    {
        return new EndFlowNode(this.name, this.title, this.methodInfo);
    }
}

