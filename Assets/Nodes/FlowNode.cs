using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public enum NodeType
{
    Start = 0,
    Condition = 1,
    Operation = 2,
    InputOutput = 3,
    End = 4
}

public abstract class FlowNode
{
    public string name;
    public string title;
    public FlowNode nextFlow;
    MethodInfo methodInfo;

    public FlowNode(string name, string title, MethodInfo info)
    {
        this.title = title;
        this.name = name;
        this.methodInfo = info;
    }

    internal void SetNextFlow(FlowNode next)
    {
        nextFlow = next;
    }

    public virtual void OnValidate()
    {
        Assert.IsNotNull(nextFlow, $"节点{name}  【{title}】 不存在下一个节点");
    }
}
