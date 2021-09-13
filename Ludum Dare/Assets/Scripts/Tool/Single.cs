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

    /// <summary>
    /// 手动初始化会调用默认的构造函数
    /// </summary>
    public static void ManuallyInit()
    {
        if (instance == null)
        {
            instance = new T();
        }
    }
}
