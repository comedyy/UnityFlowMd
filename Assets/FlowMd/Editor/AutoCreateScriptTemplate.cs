using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        string[] lines = x.text.Split(new string[]{"\r\n", "\n"}, StringSplitOptions.None);
        foreach (var line in lines)
        {
            if(line.Contains("=>"))
            {
                var split = line.Split(new string[]{"=>", ":", "|"}, StringSplitOptions.None);
                var method = split[3];

                lst.Add(GenerateMethodStruct(method, split[2], split[1]));
            }
            else
            {
                var split = line.Split(new string[]{"{", "(", "[", "}", "]", ")"}, StringSplitOptions.RemoveEmptyEntries);
                if(split.Length == 2)
                {
                    var method = split[0];
                    var nodeType = FlowParserMermaid.GetNodeType(line.Substring(method.Length, line.Length - method.Length).Replace(split[1], "").Trim());
                    lst.Add(GenerateMethodStruct(method, split[1], nodeType));
                }
            }
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

        path = Application.dataPath + "/../" + AssetDatabase.GetAssetPath(x);
        path =  Path.GetDirectoryName(path) + $"/I_{name}.cs";
        File.WriteAllText(path, builder.ToString());
        AssetDatabase.Refresh();
    }

    private static void GenerateInterface(CreateMethodInfo line, StringBuilder builder)
    {
        var isCondition = line.isCondition;
        var isInputOutput = line.isInputOutput;
        var needAsync = line.isAsync;
        var method = line.methodName;
        var comment = line.comment;

        var retStr = isCondition ? "bool" : (needAsync ? "Task" : "void");
        var param = isInputOutput ? "object param" : "";
        builder.AppendLine($"   //{comment}");
        builder.AppendLine($"   {retStr} {method}({param});\n");
    }

    private static CreateMethodInfo GenerateMethodStruct(string method, string comment, string nodeType)
    {
        var isCondition = nodeType == FlowDefine.CONDITION_NODE_STR;
        var isInputOutput = nodeType == FlowDefine.INPUTOUTPUT_NODE_STR;
        var needAsync = method.Contains("!");
        method = method.Replace("!", "");

        return new CreateMethodInfo(){
            methodName = method,
            isAsync = needAsync,
            isCondition = isCondition,
            isInputOutput = isInputOutput,
            comment = comment
        };
    }

    private static void GenerateMethod(CreateMethodInfo info, StringBuilder builder, string interfaceName)
    {
        var isCondition = info.isCondition;
        var isInputOutput = info.isInputOutput;
        var needAsync = info.isAsync;
        var method = info.methodName;
        var comment = info.comment;

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
