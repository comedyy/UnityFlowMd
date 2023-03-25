using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public static class ProcessorPool
{
    static readonly Dictionary<string, INodeProcessor> _dicProcessors = new Dictionary<string, INodeProcessor>(){
        {FlowDefine.START_NODE_STR, new NormalFlowProcessor()},
        {FlowDefine.END_NODE_STR, new NormalFlowProcessor()},
        {FlowDefine.OPERATION_NODE_STR, new NormalFlowProcessor()},
        {FlowDefine.INPUTOUTPUT_NODE_STR, new InputoutputFlowProcessor()},
        {FlowDefine.CONDITION_NODE_STR, new ConditionFlowProcessor()},
    };
    public static INodeProcessor Get(FlowNodeAsset asset)
    {
        if(asset == null) return null;

        var nodeType = asset.nodeType;
        if(_dicProcessors.TryGetValue(nodeType, out var processor))
        {
            return processor;
        }

        throw new System.Exception($"unknown NodeType {nodeType}");
    }
}

public class NormalFlowProcessor : INodeProcessor
{
    public bool IsDone(FlowNodeAsset asset, ref FlowNodeContext context)
    {
        if(context.task.Status == UniTaskStatus.Faulted)
        {
            Debug.LogError($"Excute Task Node Faild {asset}");
            context.task.GetAwaiter().GetResult();
        }

        return context.task.Status == UniTaskStatus.Succeeded;
    }

    public void Enter<T>(T scriptObj, FlowNodeAsset asset, ref FlowNodeContext context)
    {
        Assert.IsNotNull(asset.methodDelegate);
        
        if(asset.asyncMethod)
        {
            context.task = ((Func<T, UniTask>)asset.methodDelegate).Invoke(scriptObj);
        }
        else
        {
            ((Action<T>)asset.methodDelegate).Invoke(scriptObj);
        }

        context.result = PortNameConst.PORT_DEFULT;
    }
}

public class ConditionFlowProcessor : INodeProcessor
{
    public bool IsDone(FlowNodeAsset asset, ref FlowNodeContext context)
    {
        return true;
    }

    public void Enter<T>(T scriptObj, FlowNodeAsset asset, ref FlowNodeContext context)
    {
        Assert.IsNotNull(asset.methodDelegate);

        context.result = ((Func<T, bool>)asset.methodDelegate).Invoke(scriptObj) ? PortNameConst.CONDITION_YES : PortNameConst.CONDITION_NO;
    }
}

public class InputoutputFlowProcessor : INodeProcessor
{
    public bool IsDone(FlowNodeAsset asset, ref FlowNodeContext context)
    {
        return !string.IsNullOrEmpty(context.result);
    }

    public void Enter<T>(T scriptObj, FlowNodeAsset asset, ref FlowNodeContext context)
    {
    }

    public void SetInput(object o, ref FlowNodeContext context)
    {
        context.result = (string)o;
    }
}