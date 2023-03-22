using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public struct PortInfo
{
    public string portName;
    public int nextIndex;
}

public class ConnectionsInfo
{
    public List<PortInfo> ports;
}
    

public class FlowAsset
{
    public Type _scriptType;
    public string _scriptName;
    public int _entryIndex;

    public List<FlowNodeAsset> _allNodes = new List<FlowNodeAsset>();
    public ConnectionsInfo[] _allConnection;

    static FlowParserFlow s_FlowParser = new FlowParserFlow();
    static FlowParserMermaid s_MermaidParser = new FlowParserMermaid();

    public static FlowAsset Create(TextAsset asset, bool needInit = true)
    {
        IParser parser;
        if (asset.text.StartsWith("```mermaid"))
        {
            parser = s_MermaidParser;
        }
        else
        {
            parser = s_FlowParser;
        }

        var flow =  parser.Parse(asset);

        for(int i = 0; i < flow._allNodes.Count; i++)
        {
            var node = flow._allNodes[i];
            var connection = flow._allConnection[i];

            if(node.nodeType == FlowDefine.CONDITION_NODE_STR)
            {
                Assert.IsTrue(connection.ports.Exists(m=>m.portName == FlowDefine.CONDITION_NO), $"condition没有NO节点 {node.title}");
                // Assert.IsTrue(node.methodInfo.ReturnType == typeof(bool), $"condition节点返回值不是bool {node.title}");
            }
            
            if(node.nodeType != FlowDefine.END_NODE_STR)
            {
                Assert.IsTrue(connection.ports.Exists(m=>m.portName == FlowDefine.PORT_DEFULT), $"不存在下一个节点 {node.title}");
            }
        }

        Assert.IsTrue(flow._entryIndex >= 0, $"入口未找到 脚本：{flow._scriptName}");
        
        if(needInit)
        {
            flow.Init();
        }

        return flow;
    }

    private void Init()
    {
        ParseScriptType(_scriptName);

        foreach (var item in _allNodes)
        {
            if(item.nodeType == FlowDefine.INPUTOUTPUT_NODE_STR) continue;

            MethodInfo methodInfo = _scriptType.GetMethod($"I_{_scriptName}."+ item.methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            
            Assert.IsNotNull(methodInfo, $"处理节点出错，无法找到函数节点{item.methodName}，脚本：{_scriptName}");

            item.Init(methodInfo);
        }
    }

    
    private void ParseScriptType(string line)
    {
        _scriptType = Type.GetType(line);
        Assert.IsNotNull(_scriptType, $"找不到对应得脚本文件：{line} line={line}，脚本：{_scriptName}");
    }
}
