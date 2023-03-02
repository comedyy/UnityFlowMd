using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FlowNodeAsset
{
    public string nodeType;
    public string name;
    public string title;
    public MethodInfo methodInfo;
    public bool asyncMethod;

    public FlowNodeAsset(string nodeType, string name, string title, MethodInfo info)
    {
        this.nodeType = nodeType;
        this.title = title;
        this.name = name;
        this.methodInfo = info;

        if(this.methodInfo != null)
        {
            this.asyncMethod = info.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }
    }
}
