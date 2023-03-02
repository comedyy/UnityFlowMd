using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
    
public sealed class FlowNeedInjectAttribute : Attribute { }

public class Flow
{
    Type _scriptType;
    object _scriptObject;
    string _title;
    string _scriptName;
    public TextAsset asset{get; private set;}

    int _currentIndex;
    int _entryIndex;

    List<FlowNode> _allNodes = new List<FlowNode>();
    int[] _allConnection;

    public FlowNode Entry => _allNodes[_entryIndex];
    public FlowNode CurrentNode => _currentIndex == -1 ? null : _allNodes[_currentIndex];


    internal void SetName(string name)
    {
        _title = $"{_scriptName} - {name}";
    }

    public bool IsEnd{get; private set;}

    // getter
    public IList<FlowNode> AllNodes => _allNodes;
    public IList<int> AllConnection => _allConnection;
    public string Title => _title;

    public int EntryIndex => _entryIndex;

    internal static Flow Instantiate(Flow flowTemplate, string flowName)
    {
        var flow = new Flow();
        flow._scriptType = flowTemplate._scriptType;
        flow._scriptObject = Activator.CreateInstance(flow._scriptType, true);
        flow._title = $"{flowTemplate._scriptName} - {flowName}";
        flow._scriptName = flowTemplate._scriptName;
        flow.asset = flowTemplate.asset;

        flow._entryIndex = flowTemplate._entryIndex;
        flow._currentIndex = flowTemplate._currentIndex;

        flow._allNodes = new List<FlowNode>();
        foreach(var x in flowTemplate._allNodes)
        {
            var cloneNode = x.CloneNode();
            cloneNode.methodInfoScript = flow._scriptObject;
            flow._allNodes.Add(cloneNode);
        }
        flow._allConnection = flowTemplate._allConnection;

        flow.IsEnd = flowTemplate.IsEnd;
        return flow;
    }

    internal void Reset()
    {
        IsEnd = false;
        _currentIndex = _entryIndex;
    }

    private Flow()
    {

    }

    public Flow(TextAsset asset, string flowName)
    {
        string title = asset.name;
        string mdFile = asset.text;
        _title = title + " - " + flowName;
        _scriptName = title;
        this.asset = asset;

        string[] lines = mdFile.Split(new string[]{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        if(lines[0] != "```flow" || lines[lines.Length - 1] != "```")
        {
            throw new System.Exception("format error");
        }

        ParseScriptType(title);
        foreach (var line in lines)
        {
            if(line.Contains("=>"))
            {
                ParseNode(line);
            }
        }

        _allConnection = new int[_allNodes.Count];
        foreach(var line in lines)
        {
            if(line.Contains("->"))
            {
                ParseConnection(line);
            }
        }

        for(int i = 0; i < _allNodes.Count; i++)
        {
            var node = _allNodes[i];
            var connection = _allConnection[i];

            if(node is ConditionFlowNode conditionNode)
            {
                Assert.IsTrue(connection / 100 > 0, $"condition节点不存在no节点 {node.title}");
                Assert.IsTrue(conditionNode.methodInfo.ReturnType == typeof(bool), $"condition节点返回值不是bool {node.title}");
            }
            
            if(!(node is EndFlowNode))
            {
                Assert.IsTrue(connection % 100 > 0, $"不存在下一个节点 {node.title}");
            }
        }

        Assert.IsNotNull(Entry, $"入口未找到 脚本：{_title}");

        _currentIndex = _entryIndex;
    }

    private void ParseScriptType(string line)
    {
        _scriptType = Type.GetType(line);

        Assert.IsNotNull(_scriptType, $"找不到对应得脚本文件：{line} line={line}，脚本：{_title}");

        _scriptObject = Activator.CreateInstance(_scriptType, true);
    }

    void ParseNode(string line)
    {
        var x = line.Split(new string[]{"=>", ":", "|"}, StringSplitOptions.RemoveEmptyEntries);

        Assert.IsTrue(x.Length > 3);

        var name = x[0];
        var nodeType = x[1];
        var title = x[2];

        var method = x[3];
        method = method.Replace("!", "");
        MethodInfo methodInfo = _scriptType.GetMethod($"I_{_scriptName}."+method, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(methodInfo, $"处理节点出错，无法找到函数节点{method}，脚本：{_title}");

        var node = FlowNodeFactory.Create(nodeType, name, title, methodInfo);
        node.methodInfoScript = _scriptObject;
        _allNodes.Add(node);

        if(node is StartFlowNode)
        {
            _entryIndex = _allNodes.Count - 1;
        }
    }

    void ParseConnection(string line)
    {
        var x = line.Split(new string[]{"->"}, StringSplitOptions.RemoveEmptyEntries);
        (var currentIndex, var isCurrentConditionNo, var isDirChagne) = GetNode(x[0]);
        for(int i = 1; i < x.Length; i++)
        {
            (var nextIndex, var isNextConditionNo, var nextDirChagne) = GetNode(x[i]);

            if(isCurrentConditionNo && _allNodes[currentIndex] is ConditionFlowNode)
            {
                _allConnection[currentIndex] += (nextIndex + 1) * 100;
            }
            else
            {
                _allConnection[currentIndex] += nextIndex + 1;
            }

            currentIndex = nextIndex;
            isCurrentConditionNo = isNextConditionNo;
            isDirChagne= nextDirChagne;
        }
    }

    public (int, bool, bool) GetNode(string context)
    {
        var x = context.Split(new string[]{"(", ",", ")"}, StringSplitOptions.RemoveEmptyEntries);
        var name = x[0];
        var nodeIndex = _allNodes.FindIndex(m=>m.name == name);

        Assert.IsTrue(nodeIndex >= 0, $"查找node为空{name}, context = {context}，脚本：{_title}");
        var isNo = false;
        if(x.Length > 1)
        {
            isNo = x[1] == "no";    
        }
        
        var isDirChange = false;
        if(x.Length > 2)
        {
            var isDownDir = x[2] == "bottom";
            isDirChange = isNo ? isDownDir : !isDownDir;
        }

        return (nodeIndex, isNo, isDirChange);
    }

    public void Update()
    {
        if(IsEnd)
        {
            return;
        }

        while(CurrentNode != null)
        {
            if(!CurrentNode.IsEntered)
            {
                CurrentNode.Enter();
            }

            if(!CurrentNode.IsDone)
            {
                break;
            }

            CurrentNode.Exit();

            _currentIndex = GetNextNode(_currentIndex, CurrentNode.Result);
        }

        IsEnd = CurrentNode == null;
    }

    public int GetNextNode(int index, bool result)
    {
        if (result)
        {
            return _allConnection[index] % 100 - 1;
        }
        else
        {
            return _allConnection[index] / 100 - 1;
        }
    }

    internal void SetException()
    {
        IsEnd = true;
    }

    public void SetParam(object o)
    {
        if(CurrentNode != null && CurrentNode is InputOutputFlowNode inputNode)
        {
            inputNode.SetInput(o);
        }
        else
        {
            Debug.LogWarning("输入参数的时候，不在input状态");
        }
    }

    public void Inject(object o)
    {
        var needInjectType = typeof(FlowNeedInjectAttribute);
        foreach (var f in _scriptType.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
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
