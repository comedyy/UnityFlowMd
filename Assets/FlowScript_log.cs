using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FlowScript_log
{
    public static async Task Start()
    {
        Debug.Log("enter Start");
        await Task.Delay(2000);
        Debug.Log("Exit Start");
    }

    public static void Say()
    {
        Debug.Log("enter say");
    }

    public static bool IsShit()
    {
        Debug.Log("isShit");
        return Random.value > 0.5f;
    }

    public static void End()
    {
        Debug.Log("End");
    }
}
