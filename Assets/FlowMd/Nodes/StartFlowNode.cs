using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StartFlowNode : FlowNode
{
    public StartFlowNode(string name, string title, MethodInfo method) : base(name, title, method)
    {
    }
}

