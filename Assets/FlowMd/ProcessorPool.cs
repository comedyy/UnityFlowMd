using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ProcessorPool
{
    public static INodeProcessor Get(object scriptObj, FlowNodeAsset asset)
    {
        if(asset == null) return null;

        var nodeType = asset.nodeType;
        INodeProcessor processor;

        if (nodeType == FlowDefine.START_NODE_STR 
            || nodeType == FlowDefine.END_NODE_STR 
            || nodeType == FlowDefine.OPERATION_NODE_STR)
        {
            processor = new NormalFlowProcessor();
        } 
        else if(nodeType == FlowDefine.INPUTOUTPUT_NODE_STR) processor = new InputoutputFlowProcessor();
        else if(nodeType == FlowDefine.CONDITION_NODE_STR) processor = new ConditionFlowProcessor();
        else throw new System.Exception($"unknown NodeType {nodeType}");

        processor.Init(scriptObj, asset);

        return processor;
    }
}

public class NormalFlowProcessor : INodeProcessor
{
    object methodInfoScript;
    FlowNodeAsset asset;
    Task _curentTask;

    public string Result => FlowDefine.PORT_DEFULT;
    public bool IsDone => _curentTask == null || _curentTask.IsCompleted;

    public void Enter()
    {
        if(asset.methodInfo == null) return;

        if(asset.asyncMethod)
        {
            _curentTask = (Task)asset.methodInfo.Invoke(methodInfoScript, null);
        }
        else
        {
            asset.methodInfo.Invoke(methodInfoScript, null);
        }
    }
   
    public void Init(object scriptObj, FlowNodeAsset asset)
    {
        this.methodInfoScript = scriptObj;
        this.asset = asset;
    }

    public void Exit()
    {
        this.methodInfoScript = null;
        this.asset = null;
        this._curentTask = null;
    }
}

public class ConditionFlowProcessor : INodeProcessor
{
    object methodInfoScript;
    FlowNodeAsset asset;

    public bool IsDone => true;
    public string Result{get; private set;}

    public void Init(object methodInfoScript, FlowNodeAsset asset)
    {
        this.methodInfoScript = methodInfoScript;
        this.asset = asset;
    }

    public void Enter()
    {
        Result = (bool)this.asset.methodInfo.Invoke(methodInfoScript, null) ? FlowDefine.PORT_DEFULT : FlowDefine.CONDITION_NO;
    }

    public void Exit()
    {
        this.methodInfoScript = null;
        this.asset = null;
    }
}

public class InputoutputFlowProcessor : INodeProcessor
{
    object methodInfoScript;
    FlowNodeAsset asset;

    public string Result{get; private set;}
    public bool IsDone { get; private set; }

    public void Init(object scriptObj, FlowNodeAsset asset)
    {
        this.methodInfoScript = scriptObj;
        this.asset = asset;
    }

    public void Enter()
    {
        IsDone = false;
    }

    public void Exit()
    {
        this.methodInfoScript = null;
        this.asset = null;
    }

    public void SetInput(object o)
    {
        asset.methodInfo.Invoke(methodInfoScript, new object[]{o});
        Result = (string)o;
        IsDone = true;
    }
}