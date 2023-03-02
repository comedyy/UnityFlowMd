using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public enum FlowStatus
{
    NotStart,
    Working,
    Done
}

public abstract class FlowNode
{
    public string name;
    public string title;
    public bool isPortDirChange;  // output default dir change
    public MethodInfo methodInfo;
    bool asyncMethod;
    public object methodInfoScript;
    public bool Result{get; protected set;}

    public event Action<FlowNode> OnEnterEvent;
    public event Action<FlowNode> OnExitEvent;

    public FlowNode(string name, string title, MethodInfo info)
    {
        this.title = title;
        this.name = name;
        this.methodInfo = info;

        if(this.methodInfo != null)
        {
            this.asyncMethod = GetAsync(methodInfo);
        }

        Result = true;
    }

    public bool IsEntered{get; private set;}
    Task _curentTask;
    public void Enter()
    {
        _curentTask = null;
        IsEntered = true;
        OnEnterEvent?.Invoke(this);

        OnEnter();
    }

    public virtual void OnEnter()
    {
        if(methodInfo == null) return;

        if(asyncMethod)
        {
            _curentTask = (Task)methodInfo.Invoke(methodInfoScript, null);
        }
        else
        {
            methodInfo.Invoke(methodInfoScript, null);
        }
    }

    public void Exit()
    {
        IsEntered = false;

        OnExitEvent?.Invoke(this);
    }

    public virtual bool IsDone => _curentTask == null || _curentTask.IsCompleted;

    public static bool GetAsync(MethodInfo info)
    {
        return info.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
    }

    public abstract FlowNode CloneNode();
}
