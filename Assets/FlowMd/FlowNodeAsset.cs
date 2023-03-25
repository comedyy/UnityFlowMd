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
    public string methodName;
    public bool isAsyncInMd;


    public bool asyncMethod;
    public object methodDelegate;

    public FlowNodeAsset(string nodeType, string name, string title, string method, bool isAsyncInMd)
    {
        this.nodeType = nodeType;
        this.title = title;
        this.name = name;
        this.methodName = method;
        this.isAsyncInMd = isAsyncInMd;
    }

    public void Init(object methodDelegate, bool async)
    {
        this.methodDelegate = methodDelegate;
        this.asyncMethod = async;
    }
}
