using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FE_EventInfo
{
    /// <summary>
    /// 单项操作
    /// </summary>
    public class Option
    {
        /// <summary>
        /// 单项选泽的文字
        /// </summary>
        [JsonProperty]
        private string label = null;
        /// <summary>
        /// 固定体力增加量（负数为扣除）
        /// </summary>
        [JsonProperty]
        private int add_health = 0;
        /// <summary>
        /// 固定能量增减量（负数为扣除）
        /// </summary>
        [JsonProperty]
        private int add_energy = 0;
        /// <summary>
        /// 固定物品增减用(字符串表示 格式为 物品代码.lost/add)
        /// </summary>
        [JsonProperty]
        private List<string> add_item = null;
        /// <summary>
        /// 增加固定事件判断代码
        /// </summary>
        [JsonProperty]
        private List<string> add_event = null;
        /// <summary>
        /// 多次判定
        /// </summary>
        [JsonProperty]
        private List<RandomPart> random_parts = null;

        public string Label { get => label; }
        public int Add_health { get => add_health; }
        public int Add_energy { get => add_energy; }
        public List<string> Add_item { get => add_item; }
        public List<string> Add_events { get => add_event; }

        /// <summary>
        /// 执行这项选择
        /// </summary>
        /// <param name="playerModel">玩家数据模型</param>
        /// <returns></returns>
        public string ExecuteOption(PlayerModel playerModel, string chain)
        {
            //怎加固定基础属性改变
            int deltahp = Add_health;
            int deltaenerfy = Add_energy;

            List<string> add_Item = new List<string>();
            List<string> add_Events = new List<string>();
            //增加固定物品
            if (this.Add_item != null)
                add_Item.AddRange(Add_item);
            //增加固定事件代码
            if (this.Add_events != null)
            {
                add_Events.AddRange(this.Add_events);
            }

            //增加判断事件
            if (this.random_parts != null)
            {
                for (int i = 0; i < random_parts.Count; i++)
                {
                    //多次随机判定中的单次
                    if (random_parts[i].Random_modifies == null)
                        continue;

                    int hp_temp;
                    int energy_temp;
                    List<string> item_temp;
                    List<string> event_temp;

                    random_parts[i].ExecuteRandomPart(out hp_temp, out energy_temp, out item_temp, out event_temp);

                    deltahp += hp_temp;
                    deltaenerfy += energy_temp;
                    if (item_temp != null)
                    {
                        add_Item.AddRange(item_temp);
                    }
                    if (event_temp!=null)
                    {
                        add_Events.AddRange(event_temp);
                    }
                }
            }

            //所有的执行完 更改modle
            if (playerModel != null)
            {
                playerModel.ChangeHp(deltahp);
                playerModel.ChangeEnery(deltaenerfy);
                playerModel.ChangeItemList(add_Item);
                playerModel.AddExpEvents(chain, add_Events);
            }

            Debug.Log("已执行选项deltahp: " + deltahp + " " + " deltaenerfy: " + deltaenerfy);
            //生成事件报告给玩家
            return ProduceReport(deltahp, deltaenerfy, add_Item);
        }
        //事件报告
        public static string ProduceReport(int deltahp, int deltaenerfy, List<string> add_Item)
        {
            //TODO:事件报告可能需要更改 滚动text debug
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
            string get = "Get item : \n";
            string lost = "Lost item : \n";

            for (int i = 0; i < add_Item.Count; i++)
            {
                //[0]物品的id [1]物品的得到与失去
                string[] modify = add_Item[i].Split('.');
                string id = modify[0];

                //获得物品信息
                ItemInfo itemInfo = ItemInfoManager.Instance.Get(id);
                string[] ItemName = null; //物品的名字

                //如果信息出错直接下一个
                if (itemInfo == null)
                {
                    Debug.LogWarning("没有找到该物品 已跳过：" + add_Item[i]);
                    continue;
                }
                else
                {
                    ItemName = itemInfo.Item_Name.Split('#');
                }

                //al 应该是 lost 获得 add
                string gl = null;
                //如果并没有分割出信息 （数据结构有问题）
                if (modify.Length > 0)
                {
                    gl = modify[1];
                }

                //获得语言
                Config.Language language = (Config.Language)PlayerPrefs.GetInt("Language", (int)Config.Language.l_simp_chinese);

                if (gl != null)
                {
                    if (gl == "add")
                    {
                        judgeget = true;
                        get += "\t" + TextManage.Instance.GetText(itemInfo.Item_Name, language) + "\n";
                    }

                    if (gl == "lost")
                    {
                        judgelost = true;
                        lost += "\t" + TextManage.Instance.GetText(itemInfo.Item_Name, language) + "\n";
                    }
                }
                //如果没有标记当作得到处理
                else
                {
                    judgeget = true;
                    get += "\t" + TextManage.Instance.GetText(itemInfo.Item_Name, language) + "\n";
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

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("label: ");
            stringBuilder.Append(Label);
            stringBuilder.Append("\n");
            stringBuilder.Append("add_health: ");
            stringBuilder.Append(Add_health);
            stringBuilder.Append("\n");
            stringBuilder.Append("add_energy: ");
            stringBuilder.Append(Add_energy);
            stringBuilder.Append("\n");
            if (Add_item != null)
            {
                for (int i = 0; i < Add_item.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" add_item: ");
                    stringBuilder.Append(Add_item[i]);
                    stringBuilder.Append("\n");
                }
            }
            if (this.Add_events != null)
            {
                for (int i = 0; i < Add_events.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" add_events: ");
                    stringBuilder.Append(Add_events[i]);
                    stringBuilder.Append("\n");
                }
            }

            if (random_parts != null)
            {
                for (int i = 0; i < random_parts.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" random_parts: ");
                    stringBuilder.Append(random_parts[i].ToString());
                    stringBuilder.Append("\n");
                }
            }
            

            return stringBuilder.ToString();
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
        [JsonProperty]
        private List<RandomModify> random_modifies = null;

        public List<RandomModify> Random_modifies { get => random_modifies; }

        public void ExecuteRandomPart(out int addHealth, out int addEnergy, out List<string> add_Item, out List<string> add_Events)
        {
            float judge = Random.value;

            float IntervalMax = 0;
            float IntervalMin = 0;

            for (int j = 0; j < Random_modifies.Count; j++)
            {
                RandomModify randomModify = Random_modifies[j];

                IntervalMax += randomModify.Percentage;

                //如果随机到了该modify则执行
                if (judge >= IntervalMin && judge <= IntervalMax)
                {
                    randomModify.ExecuteModify(out addHealth, out addEnergy, out add_Item, out add_Events);
                    Debug.Log(judge + "  " + addHealth + "  " + addEnergy);
                    return;
                }

                IntervalMin = IntervalMax;
            }

            add_Item = null;
            add_Events = null;
            addHealth = 0;
            addEnergy = 0;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Random_modifies != null)
            {
                for (int i = 0; i < Random_modifies.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append("random_modifies: ");
                    stringBuilder.Append(Random_modifies[i].ToString());
                    stringBuilder.Append("\n");
                }
            }

            return stringBuilder.ToString();
        }
    }

    /// <summary>
    /// 一次判定中某一项所占的百分比
    /// </summary>
    public class RandomModify
    {
        [JsonProperty]
        private float percentage = 0;
        [JsonProperty]
        private int add_health = 0;
        [JsonProperty]
        private int add_energy = 0;
        [JsonProperty]
        private List<string> add_item = null;
        [JsonProperty]
        private List<string> add_events = null;

        public int Add_health { get => add_health;  }
        public int Add_energy { get => add_energy;  }
        public List<string> Add_item { get => add_item; }
        public List<string> Add_events { get => add_events; }
        public float Percentage { get => percentage; }

        public void ExecuteModify(out int addHealth, out int addEnergy, out List<string> add_Item, out List<string> add_Event)
        {
            add_Item = null;
            add_Event = null;

            //加入物品改变
            add_Item = this.Add_item;

            //加入事件符号改变
            add_Event = this.Add_events;

            addHealth = Add_health;
            addEnergy = Add_energy;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("percentage: ");
            stringBuilder.Append(percentage);
            stringBuilder.Append("\n");
            stringBuilder.Append("add_health: ");
            stringBuilder.Append(Add_health);
            stringBuilder.Append("\n");
            stringBuilder.Append("add_energy: ");
            stringBuilder.Append(Add_energy);
            stringBuilder.Append("\n");

            if (Add_item != null)
            {
                for (int i = 0; i < Add_item.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" add_item: ");
                    stringBuilder.Append(Add_item[i]);
                    stringBuilder.Append("\n");
                }
            }
            if (this.Add_events != null)
            {
                for (int i = 0; i < Add_events.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" add_events: ");
                    stringBuilder.Append(Add_events[i]);
                    stringBuilder.Append("\n");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
