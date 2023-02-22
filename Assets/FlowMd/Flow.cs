using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class Flow
{
    public FlowNode Entry{get; private set;}
    List<FlowNode> _allNodes = new List<FlowNode>();
    public IList<FlowNode> AllNodes => _allNodes;
    Type _scriptType;
    string _title;
    public bool IsEnd{get; private set;}

    public Flow(string title, string mdFile)
    {
        _title = title;

        string[] lines = mdFile.Split(new string[]{"\r\n", "\n"}, StringSplitOptions.None);
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
    }

    void ParseNode(string line)
    {
        var x = line.Split(new string[]{"=>", ":", "|"}, StringSplitOptions.None);

        Assert.IsTrue(x.Length > 3);

        var name = x[0];
        var nodeType = x[1];
        var title = x[2];

        Assert.IsNotNull(_scriptType, $"找不到对应得脚本文件，脚本：{_title}");

        var method = x[3];
        MethodInfo methodInfo = _scriptType.GetMethod(method, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        Assert.IsNotNull(methodInfo, $"处理节点出错，无法找到函数节点{method}，脚本：{_title}");

        var node = FlowNodeFactory.Create(nodeType, name, title, methodInfo);
        _allNodes.Add(node);

        if(node is StartFlowNode)
        {
            Entry = node;
        }
    }

    void ParseConnection(string line)
    {
        var x = line.Split(new string[]{"->"}, StringSplitOptions.None);
        (var current, var isCurrentConditionNo) = GetNode(x[0]);
        for(int i = 1; i < x.Length; i++)
        {
            (var next, var isNextConditionNo) = GetNode(x[i]);

            if(isCurrentConditionNo && current is ConditionFlowNode conditionFlow)
            {
                conditionFlow.SetNoCondition(next);
            }
            else
            {
                current.SetNextFlow(next);
            }

            current = next;
            isCurrentConditionNo = isNextConditionNo;
        }
    }

    public (FlowNode, bool) GetNode(string context)
    {
        var x = context.Split(new string[]{"(", ",", ")"}, StringSplitOptions.None);
        var name = x[0];
        var node = _allNodes.Find(m=>m.name == name);

        Assert.IsNotNull(node, $"查找node为空{name}, context = {context}，脚本：{_title}");

        for(int i = 1; i < x.Length; i++)
        {
            if(x[i] == "no")
            {
                return (node, true);
            }
        }

        return (node, false);
    }

    public FlowNode CurrentNode{get; private set;}
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
}