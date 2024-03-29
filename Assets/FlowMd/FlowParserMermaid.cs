﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class FlowParserMermaid : IParser
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
        if(lines[0] != "```mermaid" || lines[lines.Length - 1] != "```")
        {
            throw new System.Exception("format error");
        }

        foreach (var line in lines)
        {
            ParseNode(line);
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
            if(line.Contains("-->"))
            {
                ParseConnection(line);
            }
        }

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
        var x = line.Split(new string[]{"{", "(", "[", "}", "]", ")"}, StringSplitOptions.RemoveEmptyEntries);

        if(x.Length < 2)
        {
            return;
        }

        var name = x[0];
        var title = x[1];
        var method = x[0].Replace("!", "");
        var nodeType = GetNodeType(line.Substring(name.Length, line.Length - name.Length).Replace(title, "").Trim());
        
        var node = new FlowNodeAsset(nodeType, name, title, method, method != x[0]);
        _allNodes.Add(node);

        if(node.nodeType == FlowDefine.START_NODE_STR)
        {
            _entryIndex = _allNodes.Count - 1;
        }
    }

    public static string GetNodeType(string v)
    {
        if(v == "()") return FlowDefine.START_NODE_STR;
        else if(v == "[]") return FlowDefine.OPERATION_NODE_STR;
        else if(v == "{}") return FlowDefine.CONDITION_NODE_STR;
        else if(v == "(())") return FlowDefine.INPUTOUTPUT_NODE_STR;
        else if(v == "([])") return FlowDefine.END_NODE_STR;
        else throw new Exception($"GetNodeTypeError {v}");
    }

    void ParseConnection(string line)
    {
        var x = line.Split(new string[]{"-->"}, StringSplitOptions.RemoveEmptyEntries);
        (var currentIndex, var _) = GetNode(x[0]);
        for(int i = 1; i < x.Length; i++)
        {
            (var nextIndex, var portName) = GetNode(x[i]);
            if(_allConnection[currentIndex].ports.Exists(m=>m.portName == portName))
            {
                throw new Exception($"exist samePort {line} {portName}");
            }

            _allConnection[currentIndex].ports.Add(new PortInfo(){
                portName = portName,
                nextIndex = nextIndex
            });

            currentIndex = nextIndex;
        }
    }

    public (int, string) GetNode(string context)
    {
        var x = context.Trim().Split(new string[]{"|"}, StringSplitOptions.RemoveEmptyEntries);
        if(x.Length == 1)
        {
            var name = x[0].Trim();
            var nodeIndex = _allNodes.FindIndex(m=>m.name == name);
            return (nodeIndex, PortNameConst.PORT_DEFULT);
        }
        else if(x.Length == 2)
        {
            var name = x[1].Trim();
            var nodeIndex = _allNodes.FindIndex(m=>m.name == name);
            return (nodeIndex, x[0]);
        }
        else
        {
            throw new Exception($"GetNodeError {context} {x.Length} {string.Join(",", x)}");
        }
    }
}
