using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowPool
{
    Dictionary<TextAsset, FlowAsset> _dicAssetFlowTemplate = new Dictionary<TextAsset, FlowAsset>();
    Dictionary<FlowAsset, Queue<Flow>> _dicCachedFlow = new Dictionary<FlowAsset, Queue<Flow>>();

    public Flow GetFlowByAsset(TextAsset asset, string name)
    {
        if(!_dicAssetFlowTemplate.TryGetValue(asset, out var flowTemplate))
        {
            flowTemplate = new FlowAsset(asset);
            _dicAssetFlowTemplate.Add(asset, flowTemplate);
        }

        if(_dicCachedFlow.TryGetValue(flowTemplate, out var queue) && queue.Count > 0)
        {
            var x = queue.Dequeue();
            x.SetName(name);
            return x;
        }


        var flow = Flow.Instantiate(flowTemplate, name);
        return flow;
    }

    public void Release(Flow flow)
    {
        var asset = flow.asset;
        if(!_dicCachedFlow.TryGetValue(asset, out var queue))
        {
            queue = new Queue<Flow>();
            _dicCachedFlow.Add(asset, queue);
        }

        flow.Reset();
        queue.Enqueue(flow);
    }
}
