using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventsStatistics
{
    private Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

    public Dictionary<string, List<string>> Data { get => data; }

    public void Put(string eventChain, string str)
    {
        List<string> chain = null;

        eventChain = eventChain == null ? "null" : eventChain;  

        //如果已经有对应的列表 直接加入
        if (data.TryGetValue(eventChain, out chain))
        {
            chain.Add(str);
        }
        //如果没有直接创建新的
        else
        {
            chain = new List<string>();
            chain.Add(str);

            data.Add(eventChain, chain);
        } 
    }

    public bool Contains(string str)
    {
        foreach (var list in data.Values)
        {
            if (list.Contains(str))
            {
                return true;
            } 
        }

        return false;
    }

    /// <summary>
    /// 是否含有该判定
    /// </summary>
    /// <param name="chain">keyi</param>
    /// <param name="str"></param>
    /// <returns></returns>
    public bool Contains(string chain, string str)
    {
        List<string> list = null;

        chain = chain == null ? "null" : chain;

        if (data.TryGetValue(chain, out list))
        {
            return list.Contains(str);
        }

        return false;
    }
}
