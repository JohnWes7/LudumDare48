using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class EventInfoManager : Single<EventInfoManager>
{
    /// <summary>
    /// 存有所有事件数据的列表
    /// </summary>
    private List<EventInfo> EventInfoList { get; }

    /// <summary>
    /// 整个事件列表的长度
    /// </summary>
    public int EventInfoListCount { get { return EventInfoList.Count; } }

    /// <summary>
    /// 构造函数
    /// </summary>
    public EventInfoManager()
    {
        //读取json
        string json = Resources.Load<TextAsset>(Config.EventInfoJsonPath).text;
        //转换成list
        EventInfoList = JsonMapper.ToObject<List<EventInfo>>(json);

        #region Debug打印
        Debug.Log(json);

        for (int i = 0; i < EventInfoList.Count; i++)
        {
            EventInfoList[i].PrintSelf();
        }
        #endregion

    }

    public EventInfo GetInfo(string Id)
    {
        for (int i = 0; i < EventInfoList.Count; i++)
        {
            if (Id == EventInfoList[i].Id)
            {
                return EventInfoList[i];
            }
        }

        return null;
    }
    public EventInfo GetInfo(int index)
    {
        if (index < EventInfoList.Count)
        {
            return EventInfoList[index];
        }

        Debug.LogError("索引到空引用 index：" + index);
        return null;
    }
}

public class EventInfo
{
    public string Id { get; set; }
    public string Eventtitle { get; set; }
    public string Desciption { get; set; }
    public string Icon { get; set; }
    public int Precondition { get; set; }
    public string EventChain { get; set; }

    public List<Option> Options { get; set; }

    public void PrintSelf()
    {
        Debug.Log(Id + "\n" + this.Eventtitle + "\n" + Desciption + "\n" + Icon + "\n" + Precondition + "\n" + ( EventChain == null ? "null" : EventChain ) );
        
    }
}

/// <summary>
/// 单项操作
/// </summary>
public class Option
{
    public string Label;

    public int Add_Health;
    public int Add_Energy;
    public string Add_Item;

    /// <summary>
    /// 多次判定
    /// </summary>
    public List<RandomPart> RandomParts;

    public void Run(GameBasePanelController gameBasePanelController, PlayerModel playerModel, EventPanelController eventPanelController)
    {
        string addDescription = "\n\n";

        int addHealth = 0;
        int addEnergy = 0;

    }
}

/// <summary>
/// 单次判定用的类
/// </summary>
public class RandomPart
{
    /// <summary>
    /// 多个百分比
    /// </summary>
    public List<RandomModify> randomModifies;
}

/// <summary>
/// 一次判定中某一项所占的百分比
/// </summary>
public class RandomModify
{
    public float percentage;

    public int Add_Health;
    public int Add_Energy;
    public string Add_Item;
}
