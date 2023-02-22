using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

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


        var name = x.name;
        string[] lines = x.text.Split(new string[]{"\r\n", "\n"}, StringSplitOptions.None);
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"public static class {name}{{");

        foreach (var line in lines)
        {
            if(line.Contains("=>"))
            {
                var split = line.Split(new string[]{"=>", ":", "|"}, StringSplitOptions.None);
                var method = split[3];
                var isCondition = split[1] == "condition";

                GenerateMethod(method, split[2], isCondition, builder);
            }
        }

        builder.AppendLine($"}}");

        var path = Application.dataPath + "/../" + AssetDatabase.GetAssetPath(x);
        path = Path.ChangeExtension(path, "cs");
        File.WriteAllText(path, builder.ToString());
        AssetDatabase.Refresh();
    }

    private static void GenerateMethod(string method, string comment, bool isCondition, StringBuilder builder)
    {
        var retStr = (isCondition ? "bool" : "void");
        builder.AppendLine($"   //{comment}");
        builder.AppendLine($"   public static {retStr} {method}() {{");

        if(isCondition)
        {
            builder.AppendLine($"       return false;");
        }
        
        builder.AppendLine($"   }}\n");
    }
}
