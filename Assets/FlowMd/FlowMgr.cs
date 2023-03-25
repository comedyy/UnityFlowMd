using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowMgr : MonoSingleton<FlowMgr>
{
    List<Flow> _allFlow = new List<Flow>();
    public IReadOnlyList<Flow> AllFlow => _allFlow;
    FlowPool _pool = new FlowPool();

    bool _lockUpdate = false;
    public void Update()
    {
        _lockUpdate = true;
        for(int i = _allFlow.Count - 1; i >= 0; i--)
        {
            var item = _allFlow[i];

            try
            {
                item.Update();
            }
            catch(Exception e)
            {
                Debug.LogError($"flow excute Error {item} {e.Message} \n{e.StackTrace}");
                item.SetException();
            }

            if(item.IsEnd)
            {
                var flow = _allFlow[i];
                _allFlow.RemoveAt(i);
                _pool.Release(flow);
            }
        }
        _lockUpdate = false;
    }

    public Flow AddFlow<T>(TextAsset asset, string name)
    {
        if(_lockUpdate)
        {
            Debug.LogError("lock情况下无法添加flow");
            return null;
        }

        var flow = _pool.GetFlowByAsset<T>(asset, name);
        flow.Reset();
        _allFlow.Insert(0, flow);
        Debug.Log($"AddFlow {flow.Title}");

        return flow;
    }

    public void RemoveFlow(Flow flow)
    {
        if(flow == null) return;

        if(_lockUpdate)
        {
            Debug.LogError("lock情况下无法移除flow");
            return;
        }

        Debug.Log($"releaseFlow {flow.Title}");
        flow.OnDispose();
        _allFlow.Remove(flow);
        _pool.Release(flow);
    }
}
