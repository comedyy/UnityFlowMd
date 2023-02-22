using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FlowScript_log
{
    public static async Task Start()
    {
        Debug.Log("enter Start");
        await Task.Delay(1000);
        Debug.Log("Exit Start");
    }

    //等待我输入
    public static void WaitMyEnter(object param) 
    {
    }

    public static async Task Say()
    {
        await Task.Delay(2000);
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
