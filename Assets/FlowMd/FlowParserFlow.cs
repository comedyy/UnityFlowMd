using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public interface IParser
{
    FlowAsset Parse(TextAsset asset);
}

public class FlowParserFlow : IParser
{
    public Type _scriptType;
    public string _scriptName;
    public int _entryIndex;

    public List<FlowNodeAsset> _allNodes;
    public ConnectionsInfo[] _allConnection;

    void CleanUp()
    {
        _scriptType = default;
        _scriptName = default;
        _entryIndex = default;
        _allConnection = default;
        _allNodes = default;
    }

    public FlowAsset Parse(TextAsset asset)
    {
        CleanUp();
        _allNodes = new List<FlowNodeAsset>();

        string title = asset.name;
        string mdFile = asset.text;
        _scriptName = title;

        string[] lines = mdFile.Split(new string[]{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        if(lines[0] != "```flow" || lines[lines.Length - 1] != "```")
        {
            throw new System.Exception("format error");
        }

        foreach (var line in lines)
        {
            if(line.Contains("=>"))
            {
                ParseNode(line);
            }
        }

        _allConnection = new ConnectionsInfo[_allNodes.Count];
        for(int i = 0; i < _allConnection.Length; i++)
        {
            _allConnection[i] = new ConnectionsInfo()
            {
                ports = new List<PortInfo>()
            };
        }
        foreach(var line in lines)
        {
            if(line.Contains("->"))
            {
                ParseConnection(line);
            }
        }

        Assert.IsTrue(_entryIndex >= 0, $"入口未找到 脚本：{_scriptName}");
        return new FlowAsset(){
            _scriptType = _scriptType,
            _scriptName = _scriptName,
            _entryIndex = _entryIndex,
            _allNodes = _allNodes,
            _allConnection = _allConnection
        };
    }

    void ParseNode(string line)
    {
        var x = line.Split(new string[]{"=>", ":", "|"}, StringSplitOptions.RemoveEmptyEntries);

        Assert.IsTrue(x.Length > 3);

        var name = x[0];
        var nodeType = x[1];
        var title = x[2];
        var method = x[3].Replace("!", "");

        var node = new FlowNodeAsset(nodeType, name, title, method, method != x[3]);
        _allNodes.Add(node);

        if(node.nodeType == FlowDefine.START_NODE_STR)
        {
            _entryIndex = _allNodes.Count - 1;
        }
    }

    void ParseConnection(string line)
    {
        var x = line.Split(new string[]{"->"}, StringSplitOptions.RemoveEmptyEntries);
        (var currentIndex, var isCurrentConditionNo) = GetNode(x[0]);
        for(int i = 1; i < x.Length; i++)
        {
            (var nextIndex, var isNextConditionNo) = GetNode(x[i]);
            if(_allConnection[currentIndex].ports.Exists(m=>m.portName == isCurrentConditionNo))
            {
                throw new Exception($"exist samePort {line}");
            }

            _allConnection[currentIndex].ports.Add(new PortInfo(){
                portName = isCurrentConditionNo,
                nextIndex = nextIndex
            });

            currentIndex = nextIndex;
            isCurrentConditionNo = isNextConditionNo;
        }
    }

    public (int, string) GetNode(string context)
    {
        var x = context.Split(new string[]{"(", ",", ")"}, StringSplitOptions.RemoveEmptyEntries);
        var name = x[0];
        var nodeIndex = _allNodes.FindIndex(m=>m.name == name);

        Assert.IsTrue(nodeIndex >= 0, $"查找node为空{name}, context = {context}，脚本：{_scriptName}");
        var isNo = FlowDefine.PORT_DEFULT;
        if(x.Length > 1 && x[1] == "no")
        {
            isNo = FlowDefine.CONDITION_NO;
        }

        return (nodeIndex, isNo);
    }
}
