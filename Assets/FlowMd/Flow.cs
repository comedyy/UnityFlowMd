using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public interface ICleanUp
{
    void CleanUp();
}

public struct FlowNodeContext
{
    public UniTask task;
    public string result;
    public bool isEntered;
}
    
public sealed class FlowNeedInjectAttribute : Attribute { }
public interface INodeProcessor
{
    void Enter<T>(T scriptObj, FlowNodeAsset asset, ref FlowNodeContext context);
    bool IsDone(FlowNodeAsset asset, ref FlowNodeContext context);
}

public class Flow<T> : Flow
{
    T _scriptObject;

    public static Flow Instantiate(FlowAsset flowTemplate, string flowName)
    {
        var flow = new Flow<T>
        {
            asset = flowTemplate,
            _scriptObject = Activator.CreateInstance<T>(),
            _currentIndex = flowTemplate._entryIndex,
            IsEnd = false
        };
        flow.SetName(flowName);

        return flow;
    }

    public override void Update()
    {
        if(IsEnd)
        {
            return;
        }

        while(_currentIndex >= 0)
        {
            var processor = ProcessorPool.Get(CurrentAsset);
            if(!_context.isEntered)
            {
                _context.isEntered = true;
                processor.Enter(_scriptObject, CurrentAsset, ref _context);
                OnEnterEvent?.Invoke(CurrentAsset);
            }

            if(!processor.IsDone(CurrentAsset, ref _context))
            {
                break;
            }

            var result = _context.result;            
            _currentIndex = GetNextNode(_currentIndex, result);

            _context = default;
        }

        IsEnd = _currentIndex < 0;
    }

    protected override object ScriptObj => _scriptObject;
    public override void OnDispose()
    {
        base.OnDispose();

        try
        {
            ((ICleanUp)_scriptObject).CleanUp();
        }
        catch(Exception e)
        {
            Debug.LogError($"回收流程图异常 {Title} {e.Message} {e.StackTrace}");
        }
    }
}

public abstract class Flow
{
    protected int _currentIndex;
    public FlowAsset asset{get; protected set;}
    public FlowNodeAsset CurrentAsset => _currentIndex < 0 ? null : asset._allNodes[_currentIndex];
    string name;
    CancellationTokenSource _destroyCancellation;
    protected FlowNodeContext _context;

    internal void SetName(string name)
    {
        this.name = name;
    }

    public bool IsEnd{get; protected set;}

    // getter
    public IList<FlowNodeAsset> AllNodes => asset._allNodes;
    public string Title => asset._scriptName + " " + name;

    public int EntryIndex => asset._entryIndex;

    public Action<FlowNodeAsset> OnEnterEvent { get; set; }

    internal void Reset()
    {
        IsEnd = false;
        _currentIndex = asset._entryIndex;
        _context = default;

        _destroyCancellation = new CancellationTokenSource();
        Inject(_destroyCancellation);
    }

    public virtual void OnDispose()
    {
        _destroyCancellation.Cancel();
        _destroyCancellation = null;
    }

    protected Flow(){}

    public int GetNextNode(int index, string result)
    {
        if(index < 0 || index >= asset._allConnection.Length) throw new Exception($"out of range {index}");

        var portList = asset._allConnection[index].ports;

        // END
        if(portList.Count == 0)
        {
            return -1;
        }

        for(int i = 0; i < portList.Count; i++)
        {
            if(portList[i].portName == result) return portList[i].nextIndex;
        }

        throw new Exception($"not find port {result} node:{asset._allNodes[index].title}");
    }

    internal void SetException()
    {
        IsEnd = true;
    }

    public void SetParam(object o)
    {
        if(CurrentAsset == null)
        {
            Debug.LogError($"currentasset == null set param {o}");
            return;
        }

        var processor = ProcessorPool.Get(CurrentAsset) as InputoutputFlowProcessor;

        Assert.IsNotNull(processor, $"当前不是inputoutput节点，无法setparam，当前是 {CurrentAsset.name}");

        processor.SetInput(o, ref _context);
    }

    public void Inject(object o)
    {
        var needInjectType = typeof(FlowNeedInjectAttribute);
        foreach (var f in asset._scriptType.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            if (!Attribute.IsDefined (f, needInjectType)) {
                continue;
            }
     
            if (f.FieldType.IsAssignableFrom (o.GetType())) {
                f.SetValue (ScriptObj, o);
                break;
            }
        }
    }

    public abstract void Update();
    protected abstract object ScriptObj{get;}
}
