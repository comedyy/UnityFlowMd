using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FlowScript_log
{
    [FlowNeedInject] string context;
     string noInject = "noInject";

    public async Task Start()
    {
        Debug.Log($"enter Start {context}");
        await Task.Delay(1000);
        Debug.Log($"Exit Start {context} - {noInject}");
    }

    //等待我输入
    public void WaitMyEnter(object param) 
    {
    }

    public async Task Say()
    {
        await Task.Delay(2000);
        Debug.Log($"enter say  {context}");
    }

    public bool IsShit()
    {
        Debug.Log($"isShit  {context}");
        return Random.value > 0.5f;
    }

    public void End()
    {
        Debug.Log($"End  {context}");
    }
}
