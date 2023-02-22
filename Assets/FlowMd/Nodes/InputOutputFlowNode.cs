using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InputOutputFlowNode : FlowNode
{
    public InputOutputFlowNode(string name, string title, MethodInfo method) : base(name, title, method)
    {
    }
}

