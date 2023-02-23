using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InputOutputFlowNode : FlowNode
{
    public InputOutputFlowNode(string name, string title, MethodInfo method) : base(name, title, method)
    {
    }

    bool _isSetParam = false;
    public override void OnEnter()
    {
        _isSetParam = false;
        Debug.Log("InputOutputFlowNode wait param");
    }

    public void SetInput(object o)
    {
        methodInfo.Invoke(methodInfoScript, new object[]{o});
        _isSetParam = true;
    }

    public override bool IsDone => _isSetParam;
}

