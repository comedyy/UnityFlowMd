using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public enum FlowStatus
{
    NotStart,
    Working,
    Done
}

public abstract class FlowNode
{
    public string name;
    public string title;
    public FlowNode nextFlow;
    public MethodInfo methodInfo;
    bool asyncMethod;

    public FlowNode(string name, string title, MethodInfo info)
    {
        this.title = title;
        this.name = name;
        this.methodInfo = info;

        if(this.methodInfo != null)
        {
            this.asyncMethod = GetAsync(methodInfo);
        }
    }

    internal void SetNextFlow(FlowNode next)
    {
        nextFlow = next;
    }

    public virtual void OnValidate()
    {
        Assert.IsNotNull(nextFlow, $"节点{name}  【{title}】 不存在下一个节点");
    }

    public virtual FlowNode NextFlow => nextFlow;
    public virtual void Excute()
    {
        IsDone = false;

        if(methodInfo == null) return;
        dynamic result = methodInfo.Invoke(null, null);

        if(asyncMethod)
        {
            result.Wait();
        }

        IsDone = true;
    }

    public bool IsDone{get; protected set;}

    public static bool GetAsync(MethodInfo info)
    {
        return info.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
    }
}
