﻿using System.Collections;
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


    public MethodInfo methodInfo;
    public bool asyncMethod;

    public FlowNodeAsset(string nodeType, string name, string title, string method, bool isAsyncInMd)
    {
        this.nodeType = nodeType;
        this.title = title;
        this.name = name;
        this.methodName = method;
        this.isAsyncInMd = isAsyncInMd;
    }

    public void Init(MethodInfo info)
    {
        this.methodInfo = info;

        if(this.methodInfo != null)
        {
            this.asyncMethod = info.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }
    }
}
