// Singleton pattern -- Structural example  
using System;
using UnityEngine;

// "Singleton"
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static readonly Lazy<T> lazy =
        new Lazy<T>(() =>{
            GameObject o = new GameObject(typeof(T).Name + "singleton");
            DontDestroyOnLoad(o);
            return o.AddComponent<T>();
        });

    public static T Instance => lazy.Value;
}
