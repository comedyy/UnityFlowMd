using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
    
public sealed class FlowNeedInjectAttribute : Attribute { }

public class Flow
{
    public FlowNode Entry{get; private set;}
    List<FlowNode> _allNodes = new List<FlowNode>();
    public IList<FlowNode> AllNodes => _allNodes;
    Type _scriptType;
    object _scriptObject;
    string _title;
    string _scriptName;
    public bool IsEnd{get; private set;}

    public Flow(string title, string mdFile, string flowName)
    {
        _title = title + " - " + flowName;
        _scriptName = title;

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
            else if(line.Contains("->"))
            {
                ParseConnection(line);
            }
        }

        foreach (var item in _allNodes)
        {
            item.OnValidate();
        }

        Assert.IsNotNull(Entry, $"入口未找到 脚本：{_title}");

        CurrentNode = Entry;
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
            Entry = node;
        }
    }

    void ParseConnection(string line)
    {
        var x = line.Split(new string[]{"->"}, StringSplitOptions.RemoveEmptyEntries);
        (var current, var isCurrentConditionNo, var isDirChagne) = GetNode(x[0]);
        for(int i = 1; i < x.Length; i++)
        {
            (var next, var isNextConditionNo, var nextDirChagne) = GetNode(x[i]);

            if(isCurrentConditionNo && current is ConditionFlowNode conditionFlow)
            {
                conditionFlow.SetNoCondition(next, isDirChagne);
            }
            else
            {
                current.SetNextFlow(next, isDirChagne);
            }

            current = next;
            isCurrentConditionNo = isNextConditionNo;
            isDirChagne= nextDirChagne;
        }
    }

    public (FlowNode, bool, bool) GetNode(string context)
    {
        var x = context.Split(new string[]{"(", ",", ")"}, StringSplitOptions.RemoveEmptyEntries);
        var name = x[0];
        var node = _allNodes.Find(m=>m.name == name);

        Assert.IsNotNull(node, $"查找node为空{name}, context = {context}，脚本：{_title}");
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

        return (node, isNo, isDirChange);
    }

    public FlowNode CurrentNode{get; private set;}
    public string Title => _title;

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

            var nextNode = CurrentNode.RunTimeNextFlow;
            CurrentNode.Exit();

            CurrentNode = nextNode;
        }

        IsEnd = CurrentNode == null;
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
