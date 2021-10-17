using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace FE_EventInfo
{
    public class EventInfoManager : Single<EventInfoManager>, IAddInfo
    {
        /// <summary>
        /// 存有所有事件数据的工具类
        /// </summary>
        private EventsData events_data;
        /// <summary>
        /// 整个事件列表的长度
        /// </summary>
        public static int DayEventsCount { get { return Instance.events_data.DayEvents.Count; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EventInfoManager()
        {
            Debug.Log("event info 初始化");
            //从common导入本体 （千万注意只能读取一次）
            AddInfo(Config.FE_common_directory_PATH);
        }

        /// <summary>
        /// 通过mod路径导入
        /// </summary>
        /// <param name="mod_directory_path"></param>
        public void AddInfo(string mod_directory_path)
        {
            DirectoryInfo day_directoryInfo = null;

            //打开文件夹
            if (!Tool.OpenDirectory(mod_directory_path, "/events", out day_directoryInfo))
            {
                return;
            } 

            //递归找events下的所有json （因为就算分了类但是所有全部导入的类还是一个类 EventsData所以找全部）
            List<FileInfo> fileInfos = Tool.GetAllFiles(day_directoryInfo, "*.json");
            Debug.Log(day_directoryInfo.FullName + "中递归找到了" + fileInfos.Count + "个file");

            for (int i = 0; i < fileInfos.Count; i++)
            {
                string json;
                if (Tool.ReadText(fileInfos[i], out json, Config.Encoding))
                {
                    EventsData events = Tool.JSONString2Object<EventsData>(json);
                    if (events != null)
                    {
                        //如果读取成功 全部导入
                        this.AddInfo(events);
                    }
                }
            }
        }
        /// <summary>
        /// 将传入的event和静态类中的合体一起存放
        /// </summary>
        /// <param name="events"></param>
        public void AddInfo(EventsData events)
        {
            Debug.Log("正在添加\n" + events.ToString());

            if (this.events_data == null)
            {
                this.events_data = events;
                return;
            }

            this.events_data.AddInfo(events);
        }
        /// <summary>
        /// 可以泛型装 也许之后装载events的子类用来提取部分信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        public void AddInfo<T>(T info)
        {
            if (info is EventsData)
            {
                AddInfo(info as EventsData);
                return;
            }

            Debug.LogError("错误将 info 装箱进 Events");
        }
        /// <summary>
        /// DEBUG
        /// </summary>
        public static void DEBUG()
        {
            Debug.Log(Instance.events_data.ToString());
        }
        /// <summary>
        /// 获取事件信息 默认获取
        /// </summary>
        /// <param name="id">事件代码</param>
        /// <returns></returns>
        public static EventInfo GetInfo(string id)
        {
            for (int i = 0; i < DayEventsCount; i++)
            {
                if (Instance.events_data.DayEvents[i].Id == id)
                {
                    return Instance.events_data.DayEvents[i];
                }
            }

            return null;
        }
        /// <summary>
        /// 获取事件信息
        /// </summary>
        /// <param name="index">事件索引</param>
        /// <returns></returns>
        public static EventInfo GetInfo(int index)
        {
            if (index >= 0 && index < DayEventsCount)
            {
                return Instance.events_data.DayEvents[index];
            }

            return null;
        }
    }

    /// <summary>
    /// 用来导入数据的工具类（json按照该类导入）
    /// </summary>
    public class EventsData
    {
        public List<EventInfo> DayEvents;

        public void AddInfo(EventsData events)
        {
            if (events.DayEvents != null)
            {
                this.DayEvents.AddRange(events.DayEvents);
            }
            else
            {
                this.DayEvents = events.DayEvents;
            }
        }
        
        public override string ToString()
        {
            StringBuilder log = new StringBuilder();

            for (int i = 0; i < this.DayEvents.Count; i++)
            {
                log.Append(this.DayEvents[i].ToString());
                log.Append("==========================================\n");
            }

            return log.ToString();
        }
    }

    public class EventInfo
    {
        [JsonProperty]
        private string id = null;
        public string Id { get => id; }
        [JsonProperty]
        private string event_title = null;
        public string Event_title { get => event_title; }
        [JsonProperty]
        private string description = null;
        public string Description { get => description; }
        [JsonProperty]
        private string icon = null;
        public string Icon { get => icon; }
        [JsonProperty]
        private Precondition precondition = null;
        public Precondition Precondition { get => precondition; }
        [JsonProperty]
        private string event_chain = null;
        public string Event_chain { get => event_chain; }
        [JsonProperty]
        private List<Option> options = null;
        public List<Option> Options { get => options; }
        
        public bool TryPrecondition(PlayerEventsStatistics playerEventsStatistics)
        {
            //如果没有Precondition
            if (Precondition == null)
            {
                return true;
            }

            StringBuilder stringBuilder = new StringBuilder();
            List<string> log = new List<string>();

            bool judge = Precondition.Perform(playerEventsStatistics, this.Event_chain, out log);

            //DEBUG
            for (int i = 0; i < log.Count; i++)
            {
                stringBuilder.Append(log[i]);
            }
            Debug.Log(stringBuilder.ToString());

            return judge;
        }

        public void PrintSelf()
        {
            Debug.Log("id: " + Id + "\n" + "title: " + this.Event_title + "\ndescription: " + Description + "\nicon: " + Icon + "\nprecondition: " + (Precondition == null ? "null" : Precondition.ToString()) + "\nevent_chain: " + (Event_chain == null ? "null" : Event_chain));
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("id: " + Id + "\n" + "title: " + this.Event_title + "\ndescription: " + Description + "\nicon: " + Icon + "\nprecondition: " + (Precondition == null ? "null" : Precondition.ToString()) + "\nevent_chain: " + (Event_chain == null ? "null" : Event_chain) + "\n");

            if (options != null)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append("option");
                    stringBuilder.Append(options[i].ToString());
                    stringBuilder.Append("\n");
                }
            }

            return stringBuilder.ToString();
        }
    }


}



#region 废弃的代码
//public class IDInfoPair
//{
//    public KeyValuePair<string, EventInfo> keyValuePair;

//    public IDInfoPair(KeyValuePair<string, EventInfo> keyValuePair)
//    {
//        this.keyValuePair = keyValuePair;
//    }
//}

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
#endregion
