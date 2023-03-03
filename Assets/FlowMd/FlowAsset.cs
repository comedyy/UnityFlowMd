using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
    

public class FlowAsset
{
    public Type _scriptType;
    public string _scriptName;
    public int _entryIndex;

    public List<FlowNodeAsset> _allNodes = new List<FlowNodeAsset>();
    public int[] _allConnection;

    public FlowAsset(TextAsset asset)
    {
        string title = asset.name;
        string mdFile = asset.text;
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

            if(node.nodeType == FlowDefine.CONDITION_NODE_STR)
            {
                Assert.IsTrue(connection / 100 > 0, $"condition节点不存在no节点 {node.title}");
                Assert.IsTrue(node.methodInfo.ReturnType == typeof(bool), $"condition节点返回值不是bool {node.title}");
            }
            
            if(node.nodeType != FlowDefine.END_NODE_STR)
            {
                Assert.IsTrue(connection % 100 > 0, $"不存在下一个节点 {node.title}");
            }
        }

        Assert.IsTrue(_entryIndex >= 0, $"入口未找到 脚本：{_scriptName}");
    }

    private void ParseScriptType(string line)
    {
        _scriptType = Type.GetType(line);
        Assert.IsNotNull(_scriptType, $"找不到对应得脚本文件：{line} line={line}，脚本：{_scriptName}");
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
        Assert.IsNotNull(methodInfo, $"处理节点出错，无法找到函数节点{method}，脚本：{_scriptName}");

        var node = new FlowNodeAsset(nodeType, name, title, methodInfo);
        _allNodes.Add(node);

        if(node.nodeType == FlowDefine.START_NODE_STR)
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

            if(isCurrentConditionNo && _allNodes[currentIndex].nodeType == FlowDefine.CONDITION_NODE_STR)
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

        Assert.IsTrue(nodeIndex >= 0, $"查找node为空{name}, context = {context}，脚本：{_scriptName}");
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
}
