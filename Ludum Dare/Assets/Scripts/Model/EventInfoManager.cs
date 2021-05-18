using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class EventInfoManager : Single<EventInfoManager>
{
    /// <summary>
    /// 存有所有事件数据的列表
    /// </summary>
    //private List<EventInfo> EventInfoList { get; }
    private Dictionary<string, EventInfo> EventInfoTable { get; }

    /// <summary>
    /// 整个事件列表的长度
    /// </summary>
    public int EventInfoListCount { get { return EventInfoTable.Count; } }

    /// <summary>
    /// 构造函数
    /// </summary>
    public EventInfoManager()
    {
        //读取json
        string json = Resources.Load<TextAsset>(Config.EventInfoJsonPath).text;
        //转换成table
        EventInfoTable = JsonMapper.ToObject<Dictionary<string, EventInfo>>(json);

        #region Debug打印
        Debug.Log(json);
        #endregion

        foreach (var item in EventInfoTable)
        {
            item.Value.Id = item.Key;
        }
    }

    public EventInfo GetInfo(string Id)
    {
        EventInfo eventInfo;
        EventInfoTable.TryGetValue(Id, out eventInfo);

        return eventInfo;
    }

    public EventInfo GetInfo(int index)
    {
        if (index < EventInfoTable.Count)
        {
            int i = 0;
            foreach (KeyValuePair<string, EventInfo> item in EventInfoTable)
            {
                if (index == i++)
                {
                    return item.Value;
                }
            }
        }

        Debug.LogError("索引到空引用 index：" + index);
        return null;
    }
}

public class IDInfoPair
{
    public KeyValuePair<string, EventInfo> keyValuePair;

    public IDInfoPair(KeyValuePair<string, EventInfo> keyValuePair)
    {
        this.keyValuePair = keyValuePair;
    }
}

public class EventInfo
{
    public string Id { get; set; }
    public string Eventtitle { get; set; }
    public string Desciption { get; set; }
    public string Icon { get; set; }
    public string Precondition { get; set; }
    public string EventChain { get; set; }

    public List<Option> Options { get; set; }

    public void PrintSelf()
    {
        Debug.Log(Id + "\n" + this.Eventtitle + "\n" + Desciption + "\n" + Icon + "\n" + (Precondition == null ? "null" : EventChain) + "\n" + (EventChain == null ? "null" : EventChain));

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
    public List<string> Add_Item;

    /// <summary>
    /// 多次判定
    /// </summary>
    public List<RandomPart> RandomParts;

    public string ExecuteOption(PlayerModel playerModel)
    {

        int deltahp = Add_Health;
        int deltaenerfy = Add_Energy;
        List<string> add_Item = new List<string>();

        if (Add_Item != null)
            add_Item.AddRange(Add_Item);

        if (RandomParts != null)
        {
            for (int i = 0; i < RandomParts.Count; i++)
            {
                //多次随机判定中的单次
                if (RandomParts[i].randomModifies == null)
                    continue;

                int hptemp;
                int energytemp;
                List<string> itemtemp;

                RandomParts[i].ExecuteRandomPart(out hptemp, out energytemp, out itemtemp);

                deltahp += hptemp;
                deltaenerfy += energytemp;
                add_Item.AddRange(itemtemp);
            }
        }

        //所有的执行完 更改modle
        if (playerModel != null)
        {
            playerModel.ChangeHp(deltahp);
            playerModel.ChangeEnery(deltaenerfy);
            playerModel.ChangeItemList(add_Item);
        }

        //生成事件报告给玩家
        return ProduceReport(deltahp, deltaenerfy, add_Item);
    }

    public static string ProduceReport(int deltahp, int deltaenerfy, List<string> add_Item)
    {
        string addDescip = "\n\n";

        //血量
        if (deltahp > 0)
        {
            addDescip += "Durability: +" + deltahp.ToString() + "\n";
        }
        else if (deltahp < 0)
        {
            addDescip += "Durability: " + deltahp.ToString() + "\n";
        }

        //能量
        if (deltaenerfy > 0)
        {
            addDescip += "Fuel: +" + deltaenerfy.ToString() + "\n";
        }
        else if (deltaenerfy < 0)
        {
            addDescip += "Fuel: " + deltaenerfy.ToString() + "\n";
        }

        //遗物
        bool judgelost = false;
        bool judgeget = false;
        string get = "Get item : ";
        string lost = "Lost item : ";

        for (int i = 0; i < add_Item.Count; i++)
        {
            //[0]物品的id [1]物品的得到与失去
            string[] modify = add_Item[i].Split('.');
            string id = modify[0];

            string gl = null;
            if (modify.Length > 0)
            {
                gl = modify[1];
            }

            //获得名字
            string[] ItemName = ItemInfoManager.Instance.Get(id).ItemName.Split('#');
            //获得语言
            int language = PlayerPrefs.GetInt("Language", (int)Config.Language.CH);

            if (gl != null)
            {
                if (gl == "get")
                {
                    judgeget = true;
                    get += ItemName[language];
                }

                if (gl == "lost")
                {
                    judgelost = true;
                    lost += ItemName[language];
                }
            }
            //如果没有标记当作得到处理
            else
            {
                judgeget = true;
                get += ItemName[language];
            }
        }

        if (judgeget)
        {
            addDescip += get + "\n";
        }

        if (judgelost)
        {
            addDescip += lost + "\n";
        }

        return addDescip;
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

    public void ExecuteRandomPart(out int addHealth, out int addEnergy, out List<string> add_Item)
    {
        float judge = Random.value;

        float IntervalMax = 0;
        float IntervalMin = 0;

        for (int j = 0; j < randomModifies.Count; j++)
        {
            RandomModify randomModify = randomModifies[j];

            IntervalMax += randomModify.percentage;

            //如果随机到了该modify则执行
            if (judge >= IntervalMin && judge <= IntervalMax)
            {
                randomModify.ExecuteModify(out addHealth, out addEnergy, out add_Item);
                Debug.Log(judge + "  " + addHealth + "  " + addEnergy);
                return;
            }

            IntervalMin = IntervalMax;
        }

        add_Item = null;
        addHealth = 0;
        addEnergy = 0;
    }
}

/// <summary>
/// 一次判定中某一项所占的百分比
/// </summary>
public class RandomModify
{
    public float percentage;

    public int Add_Health;
    public int Add_Energy;
    public List<string> Add_Item;

    public void ExecuteModify(out int addHealth, out int addEnergy, out List<string> add_Item)
    {
        add_Item = new List<string>();
        if (Add_Item != null)
            add_Item.AddRange(Add_Item);
        addHealth = Add_Health;
        addEnergy = Add_Energy;
    }
}


////old神必堡垒 解码器
//public void EventDecoder(string change)
//{
//    string addDescip = "\n\n";

//    if (change == "")
//    {
//        addDescip += "Nothing Happen....";
//        goto end;
//    }

//    string[] events;
//    int deltahp = 0;             //血量变化，最后一起加
//    int deltafuel = 0;           //燃油变化
//    double random;              //事件概率
//    List<int> changeItemID = new List<int>();     //变化的物品


//    events = change.Split('#');

//    deltahp = int.Parse(events[0]);
//    deltafuel = int.Parse(events[1]);

//    for (int i = 2; i < events.Length; i = i + 4)
//    {

//        random = double.Parse(events[i]);// 判断事件是否发生
//        if (random < Random.Range(0f, 1f))
//        {
//            continue;
//        }

//        deltahp += int.Parse(events[i + 1]);// 事件中的数值变化
//        deltafuel += int.Parse(events[i + 2]);

//        if (events[i + 3] != "0")
//        {
//            // 遗物的变化
//            //yiwu.add(Integer.parseInt(events[i + 3]));
//            int temp = int.Parse(events[i + 3]);

//            //如果有变化再加入数组
//            if (temp != 0)
//            {
//                //GameManager.Instance.ChangeItemList(temp);
//                changeItemID.Add(temp);
//            }
//        }
//    }

//    //向数据加入
//    //GameManager.Instance.ChangeEnery(deltafuel);
//    //GameManager.Instance.ChangeHp(deltahp);


//    //结束报告
//    {
//        //血量
//        if (deltahp > 0)
//        {
//            addDescip += "Durability: +" + deltahp.ToString() + "\n";
//        }
//        else if (deltahp < 0)
//        {
//            addDescip += "Durability: " + deltahp.ToString() + "\n";
//        }

//        //能量
//        if (deltafuel > 0)
//        {
//            addDescip += "Fuel: +" + deltafuel.ToString() + "\n";
//        }
//        else if (deltafuel < 0)
//        {
//            addDescip += "Fuel: " + deltafuel.ToString() + "\n";
//        }
//    }

//    {
//        //遗物
//        bool judgelost = false;
//        bool judgeget = false;
//        string get = "Get item : ";
//        string lost = "Lost item : ";

//        //对get lost 进行操作
//        for (int i = 0; i < changeItemID.Count; i++)
//        {
//            Debug.Log("changeItemID " + i + " : " + changeItemID[i]);
//            string[] name = ItemInfoManager.Instance.Get(Mathf.Abs(changeItemID[i])).ItemName.Split('#');
//            int language = PlayerPrefs.GetInt("Language", (int)Config.Language.CH);

//            if (changeItemID[i] > 0)
//            {
//                judgeget = true;
//                get += name[language] + " ";
//            }
//            else if (changeItemID[i] < 0)
//            {
//                judgelost = true;
//                lost += name[language] + " ";
//            }

//        }
//        get += "\n";
//        lost += "\n";


//        //如果有get
//        if (judgeget)
//        {
//            addDescip += get;
//        }
//        //如果有lost
//        if (judgelost)
//        {
//            addDescip += lost;
//        }
//        addDescip += "\n";

//    }


//    Debug.Log("deltafuel:" + deltafuel + "  deltahp:" + deltahp);

////空语句直接在这边开始执行
//end:

//    //更改Desciption
//    ChangeDesciption(Desciption.text + addDescip);

//    //事件完成标记
//    mEventController.SetIsDone(true);
//}