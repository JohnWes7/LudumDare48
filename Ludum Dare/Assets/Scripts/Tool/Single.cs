using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Single<T> where T:new()
{
    /// <summary>
    /// 单例的实例
    /// </summary>
    private static T instance;
    public static T Instance 
    {
        get
        {
            if (instance==null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}
