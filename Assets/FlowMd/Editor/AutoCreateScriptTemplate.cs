using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

struct CreateMethodInfo
{
    public string methodName;
    public bool isCondition;
    public bool isAsync;
    public bool isInputOutput;
    public string comment;

    public string[] allPortNames;
}

public class AutoCreateScriptTemplate
{
    [MenuItem("Assets/AutoCreateScriptTemplate")]
    public static void Func()
    {
        var x = Selection.activeObject as TextAsset;
        if(x == null)
        {
            Debug.LogError("当前单位不是textAsset");
            return;
        }

        List<CreateMethodInfo> lst = new List<CreateMethodInfo>();
        FlowAsset asset = FlowAsset.Create(x, false);
        for(int i = 0; i < asset._allNodes.Count; i++)
        {
            var item = asset._allNodes[i];
            var connection = asset._allConnection[i];
            lst.Add(GenerateMethodStruct(item.methodName, item.title, item.nodeType, item.isAsyncInMd, connection));
        }

        var name = x.name;
        StringBuilder builder = new StringBuilder();
        
        var path = Application.dataPath + "/../" + AssetDatabase.GetAssetPath(x);
        path = Path.ChangeExtension(path, "cs");

        if(!File.Exists(path))
        {
            builder.AppendLine($"using System.Threading.Tasks;");
            builder.AppendLine($"public class {name} : I_{name} {{");

            foreach (var line in lst)
            {
                GenerateMethod(line, builder, $"I_{name}");
            }

            builder.AppendLine($"}}");

            File.WriteAllText(path, builder.ToString());
            AssetDatabase.Refresh();
        }

        builder.Clear();
        builder.AppendLine($"using System.Threading.Tasks;");
        builder.AppendLine($"public interface I_{name} {{");

        foreach (var line in lst)
        {
            GenerateInterface(line, builder);
        }
        builder.AppendLine($"}}");

        // generate inputpoutput string
        foreach (var line in lst)
        {
            GererateInputOutputString(line, builder);
        }

        path = Application.dataPath + "/../" + AssetDatabase.GetAssetPath(x);
        path =  Path.GetDirectoryName(path) + $"/I_{name}.cs";
        File.WriteAllText(path, builder.ToString());
        AssetDatabase.Refresh();
    }

    private static void GererateInputOutputString(CreateMethodInfo line, StringBuilder builder)
    {
        if(!line.isInputOutput) return;

        builder.AppendLine($"public class {line.methodName}Const {{");
        foreach (var item in line.allPortNames)
        {
            builder.AppendLine($"   public string _{item}=\"{item}\";");
        }
        builder.AppendLine($"}}");
    }

    private static void GenerateInterface(CreateMethodInfo line, StringBuilder builder)
    {
        var isCondition = line.isCondition;
        var isInputOutput = line.isInputOutput;
        var needAsync = line.isAsync;
        var method = line.methodName;
        var comment = line.comment;

        if(isInputOutput) return;

        var retStr = isCondition ? "bool" : (needAsync ? "Task" : "void");
        var param = isInputOutput ? "object param" : "";
        builder.AppendLine($"   //{comment}");
        builder.AppendLine($"   {retStr} {method}({param});\n");
    }

    private static CreateMethodInfo GenerateMethodStruct(string method, string comment, string nodeType, bool isAsync, ConnectionsInfo info)
    {
        var isCondition = nodeType == FlowDefine.CONDITION_NODE_STR;
        var isInputOutput = nodeType == FlowDefine.INPUTOUTPUT_NODE_STR;

        return new CreateMethodInfo(){
            methodName = method,
            isAsync = isAsync,
            isCondition = isCondition,
            isInputOutput = isInputOutput,
            comment = comment,
            allPortNames = info.ports.Select(m=>m.portName).ToArray()
        };
    }

    private static void GenerateMethod(CreateMethodInfo info, StringBuilder builder, string interfaceName)
    {
        var isCondition = info.isCondition;
        var isInputOutput = info.isInputOutput;
        var needAsync = info.isAsync;
        var method = info.methodName;
        var comment = info.comment;

        if(isInputOutput) return; // input output 不需要函数

        var retStr = isCondition ? "bool" : (needAsync ? "async Task" : "void");
        var param = isInputOutput ? "object param" : "";
        builder.AppendLine($"   //{comment}");
        builder.AppendLine($"   {retStr} {interfaceName}.{method}({param}) {{");

        if(isCondition)
        {
            builder.AppendLine($"       return false;");
        }
        
        builder.AppendLine($"   }}\n");
    }
}
