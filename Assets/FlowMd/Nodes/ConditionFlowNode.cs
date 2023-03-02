using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class ConditionFlowNode : FlowNode
{
    public ConditionFlowNode(string name, string title, MethodInfo method) : base(name, title, method)
    {
    }

    public override void OnEnter()
    {
        Result = (bool)this.methodInfo.Invoke(methodInfoScript, null);
    }

    public override FlowNode CloneNode()
    {
        return new ConditionFlowNode(this.name, this.title, this.methodInfo);
    }
}

