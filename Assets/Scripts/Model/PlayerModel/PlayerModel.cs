using FE_EventInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 该句游戏的玩家信息类
/// </summary>
public class PlayerModel : Single<PlayerModel>
{
    [Header("玩家基本信息")]
    [SerializeField, Tooltip("统计天数（关卡）")]
    private int hp;
    private int energy;
    private int gameDay;
    private int score;
    private List<string> itemIDList;
    

    private PlayerEventsStatistics eventStatistics;

    public List<string> ItemIDList { get => itemIDList; }
    public int GameDay { get => gameDay; }
    public int Hp { get => hp;  }
    public int Energy { get => energy;  }
    public int Score { get => score;  }
    public PlayerEventsStatistics EventStatistics { get => eventStatistics; }

    [SerializeField]
    private bool isDead = false;
    
    /// <summary>
    /// 手动初始化
    /// </summary>
    public static void InIt()
    {
        ManuallyInit();
        Instance.gameDay = 1;
        Instance.hp = 100;
        Instance.energy = 100;
        Instance.score = 0;
        Instance.itemIDList = new List<string>();
        Instance.isDead = false;
        Instance.eventStatistics = new PlayerEventsStatistics();
    }

    public static bool TryEvent(EventInfo eventInfo)
    {
        if (eventInfo != null)
        {
            return eventInfo.TryPrecondition(Instance.EventStatistics);
        }

        Debug.LogWarning("需要进行事件前置检测的eventInfo 为null （来自PlayerModel.TryEvent）");
        return false;
    }
    public static bool TryEvent(int index, out string id)
    {
        EventInfo eventInfo = EventInfoManager.GetInfo(index);
        if (eventInfo == null)
        {
            Debug.Log("没有索引到！！index：" + index);
            id = null;
            return false;
        }

        id = eventInfo.Id;
        return TryEvent(eventInfo); 
    }
    public static bool TryEvent(int index)
    {
        EventInfo eventInfo = EventInfoManager.GetInfo(index);
        if (eventInfo == null)
        {
            Debug.Log("没有索引到！！index：" + index);
            return false;
        }

        return TryEvent(eventInfo);
    }

    public void IncreaseDay()
    {
        gameDay++;
    }

    public void ChangeEnery(int deltaEnergy)
    {
        //更改数值
        this.energy = Mathf.Clamp(this.Energy + deltaEnergy, 0, 100);

        //更新ui显示
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        if (cbpc)
            cbpc.UpdateEnergy(this.Energy);

        if (this.Energy <= 0)
        {
            if (isDead == false)
            {
                isDead = true;
                GameManager.Instance.Dead();
            }

        }
    }

    public void ChangeHp(int deltaHp)
    {
        //更改数值
        this.hp = Mathf.Clamp(this.Hp + deltaHp, 0, 100);

        //更新ui显示
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        if (cbpc)
            cbpc.UpdateHp(this.Hp);

        if (this.Hp <= 0)
        {
            if (isDead == false)
            {
                isDead = true;
                GameManager.Instance.Dead();
            }

        }
    }

    public void AddExpEvents(string chain, string str)
    {
        EventStatistics.Put(chain, str);
    }

    public void AddExpEvents(string chain, List<string> str_list)
    {
        if (str_list != null)
        {
            for (int i = 0; i < str_list.Count; i++)
            {
                AddExpEvents(chain, str_list[i]);
            }
        }
    }

    public void ChangeItemList(string item_change)
    {
        //[0]物品的id [1]物品的得到与失去
        string[] modify = item_change.Split('.');
        string id = modify[0];

        //如果没有这一项物品直接跳过
        if (ItemInfoManager.Instance.Get(id) == null)
        {
            Debug.LogWarning("未找到该遗物 " + id + " ,没有加入到player items");
            return;
        }

        string gl = null;
        if (modify.Length > 0)
        {
            gl = modify[1];
        }

        if (gl != null)
        {
            if (gl == "add")
            {
                ItemIDList.Add(id);
            }

            if (gl == "lost")
            {
                ItemIDList.Remove(id);
            }
        }
        //如果没有标记当作得到处理
        else
        {
            ItemIDList.Add(id);
        }

        //更新显示
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        if (cbpc)
            cbpc.UpdateItems(ItemIDList);

    }

    public void ChangeItemList(List<string> item_change_list)
    {
        if (item_change_list == null)
            return;

        for (int i = 0; i < item_change_list.Count; i++)
        {
            ChangeItemList(item_change_list[i]);
        }
    }

    public void CaculateScore()
    {
        score = 0;
        //计算分数
        for (int i = 0; i < ItemIDList.Count; i++)
        {
            ItemInfo info = ItemInfoManager.Instance.Get(ItemIDList[i]);
            score += info.Score * 50;
        }
        score += GameDay * 10;
    }
}
