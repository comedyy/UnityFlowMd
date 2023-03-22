using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
    
public sealed class FlowNeedInjectAttribute : Attribute { }
public interface INodeProcessor
{
    void Init(object scriptObj, FlowNodeAsset asset);
    void Dispose();
    void Enter();
    void Exit();
    string Result{get;}
    bool IsDone{get;}
}

public class Flow
{
    object _scriptObject;
    int _currentIndex;
    public FlowAsset asset{get; private set;}
    public FlowNodeAsset CurrentAsset => _currentIndex < 0 ? null : asset._allNodes[_currentIndex];
    string name;

    internal void SetName(string name)
    {
        this.name = name;
    }

    public bool IsEnd{get; private set;}

    // getter
    public IList<FlowNodeAsset> AllNodes => asset._allNodes;
    public string Title => asset._scriptName + " " + name;

    public int EntryIndex => asset._entryIndex;

    public Action<FlowNodeAsset> OnEnterEvent { get; set; }

    public static Flow Instantiate(FlowAsset flowTemplate, string flowName)
    {
        var flow = new Flow();
        flow.asset = flowTemplate;
        flow._scriptObject = Activator.CreateInstance(flowTemplate._scriptType, true);
        flow._currentIndex = flowTemplate._entryIndex;
        flow.IsEnd = false;
        flow.SetName(flowName);

        return flow;
    }

    internal void Reset()
    {
        IsEnd = false;
        _currentIndex = asset._entryIndex;
    }

    private Flow()
    {

    }

    INodeProcessor _processor;
    public void Update()
    {
        if(IsEnd)
        {
            return;
        }

        if(_processor == null)
        {
            _processor = ProcessorPool.Get(_scriptObject, CurrentAsset);
            _processor.Enter();
            OnEnterEvent?.Invoke(CurrentAsset);
        }

        while(_processor != null)
        {
            if(!_processor.IsDone)
            {
                break;
            }

            _processor.Exit();

            _currentIndex = GetNextNode(_currentIndex, _processor.Result);
            _processor = ProcessorPool.Get(_scriptObject, CurrentAsset);

            if(_processor != null)
            {
                _processor.Enter();
                OnEnterEvent?.Invoke(CurrentAsset);
            }
        }

        IsEnd = _processor == null;
    }


    public int GetNextNode(int index, string result)
    {
        if(index < 0 || index >= asset._allConnection.Length) throw new Exception($"out of range {index}");

        var portList = asset._allConnection[index].ports;
        for(int i = 0; i < portList.Count; i++)
        {
            if(portList[i].portName == result) return portList[i].nextIndex;
        }

        return -1;
    }

    internal void SetException()
    {
        IsEnd = true;
    }

    public void SetParam(object o)
    {
        if(_processor != null && _processor is InputoutputFlowProcessor inputoutput)
        {
            inputoutput.SetInput(o);
        }
        else
        {
            Debug.LogWarning("输入参数的时候，不在input状态");
        }
    }

    public void Inject(object o)
    {
        var needInjectType = typeof(FlowNeedInjectAttribute);
        foreach (var f in asset._scriptType.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            if (!Attribute.IsDefined (f, needInjectType)) {
                continue;
            }
     
            if (f.FieldType.IsAssignableFrom (o.GetType())) {
                f.SetValue (_scriptObject, o);
                break;
            }
        }
    }
}
