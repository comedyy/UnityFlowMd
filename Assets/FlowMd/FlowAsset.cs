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

    static FlowParserFlow s_FlowParser = new FlowParserFlow();
    static FlowParserMermaid s_MermaidParser = new FlowParserMermaid();

    internal static FlowAsset Create(TextAsset asset)
    {
        IParser parser = null;
        if(asset.text.StartsWith("```mermaid"))
        {
            parser = s_MermaidParser;
        }
        else
        {
            parser = s_FlowParser;
        }

        return parser.Parse(asset);
    }
}
