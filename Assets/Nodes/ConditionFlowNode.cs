using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class ConditionFlowNode : FlowNode
{
    public FlowNode nextFlowNo;
    public ConditionFlowNode(string name, string title, MethodInfo method) : base(name, title, method)
    {
    }

    internal void SetNoCondition(FlowNode next)
    {
        nextFlowNo = next;
    }

    public override void OnValidate()
    {
        base.OnValidate();
        Assert.IsNotNull(nextFlowNo, $"condition节点【{name}】 【{title}】 不存在nocondition");
    }
}

