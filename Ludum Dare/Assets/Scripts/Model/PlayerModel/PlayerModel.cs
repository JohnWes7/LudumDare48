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
    public int Hp;
    public int Energy;
    public int GameDay;
    public int Score;
    public List<string> ItemIDList { get; set; }

    //TODO:玩家数据要更改 添加经过事件 逻辑判断联动
    private PlayerEventsStatistics eventStatistics;

    [SerializeField]
    private bool isDead = false;

    public void InIt()
    {
        GameDay = 1;
        Hp = 100;
        Energy = 100;
        Score = 0;
        ItemIDList = new List<string>();
        isDead = false;
        eventStatistics = new PlayerEventsStatistics();
    }

    public static bool TryEvent(int index, out string id)
    {
        EventInfo eventInfo = EventInfoManager.GetInfo(index);
        if (eventInfo == null)
        {
            Debug.Log("没有索引到！！index："+index);
            id = null;
            return false;
        }

        id = eventInfo.Id;

        //如果是null代表可重复出现
        if (eventInfo.Precondition== null)
        {
            return true;
        }
        else
        {
            //TODO:这里要写生成事件的判定

            return true;
        }
    }

    public void IncreaseDay()
    {
        GameDay++;
    }

    public void ChangeEnery(int deltaEnergy)
    {
        //更改数值
        this.Energy = Mathf.Clamp(this.Energy + deltaEnergy, 0, 100);

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
        this.Hp = Mathf.Clamp(this.Hp + deltaHp, 0, 100);

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
        eventStatistics.Put(chain, str);
    }

    public void ChangeItemList(string item_change)
    {
        //[0]物品的id [1]物品的得到与失去
        string[] modify = item_change.Split('.');
        string id = modify[0];

        //如果没有这一项物品直接跳过
        if (ItemInfoManager.Instance.Get(id) == null)
        {
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
        if(cbpc)
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
        Score = 0;
        //计算分数
        for (int i = 0; i < ItemIDList.Count; i++)
        {
            ItemInfo info = ItemInfoManager.Instance.Get(ItemIDList[i]);
            Score += info.Score * 50;
        }
        Score += GameDay * 10;
    }
}
