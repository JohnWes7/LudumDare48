using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class Tool
{
    public static T JSONString2Object<T>(string json)
    {
        //新的转json工具
        T t = JsonConvert.DeserializeObject<T>(json);

        //如果转换失败 生成报告
        if (t == null)
        {
            Debug.LogWarning("json转换类失败\n失败json ：" + json + "\n" + t.GetType().ToString());
        }

        return t;
    }
}
