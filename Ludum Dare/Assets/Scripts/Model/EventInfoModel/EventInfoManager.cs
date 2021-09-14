using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text;
using Newtonsoft.Json;

namespace FE_EventInfo
{
    public class EventInfoManager : Single<EventInfoManager>
    {
        /// <summary>
        /// 存有所有事件数据的工具类
        /// </summary>
        private Events events;

        /// <summary>
        /// 整个事件列表的长度
        /// </summary>
        public static int DayEventsCount { get { return Instance.events.DayEvents.Count; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EventInfoManager()
        {
            //读取json
            string json = ReadTextFromResourcesJSON(Config.EventInfoJsonPath);
            //将自己的json转入数据
            Events default_event = null;
            //default_event = JsonMapper.ToObject<Events>(json);
            default_event = JsonConvert.DeserializeObject<Events>(json); //新版的插件读取可以读取到private

            #region Debug打印
            Debug.Log(json);
            #endregion

            this.events = default_event;
        }

        public static void DEBUG()
        {
            StringBuilder log = new StringBuilder();

            for (int i = 0; i < DayEventsCount; i++)
            {
                log.Append(Instance.events.DayEvents[i].ToString());
                log.Append("==========================================\n");
            }

            Debug.Log(log.ToString());
        }

        public static string ReadTextFromResourcesJSON(string PATH)
        {
            //读取json
            string json = Resources.Load<TextAsset>(PATH).text;
            return json;
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
                if (Instance.events.DayEvents[i].Id == id)
                {
                    return Instance.events.DayEvents[i];
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
                return Instance.events.DayEvents[index];
            }

            return null;
        }
    }

    /// <summary>
    /// 用来导入数据的工具类（json按照该类导入）
    /// </summary>
    public class Events
    {
        public List<EventInfo> DayEvents;
    }

    public class EventInfo
    {
        [JsonProperty]
        private string id;
        public string Id { get => id; }
        [JsonProperty]
        private string event_title;
        public string Event_title { get => event_title; }
        [JsonProperty]
        private string description;
        public string Description { get => description; }
        [JsonProperty]
        private string icon;
        public string Icon { get => icon; }
        [JsonProperty]
        private Precondition precondition;
        public Precondition Precondition { get => precondition; }
        [JsonProperty]
        private string event_chain;
        public string Event_chain { get => event_chain; }
        [JsonProperty]
        private List<Option> options;
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
