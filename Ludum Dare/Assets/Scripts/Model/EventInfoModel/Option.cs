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
        private string label;
        public string Label { get => label; }

        /// <summary>
        /// 固定体力增加量（负数为扣除）
        /// </summary>
        [JsonProperty]
        private int add_health;
        /// <summary>
        /// 固定能量增减量（负数为扣除）
        /// </summary>
        [JsonProperty]
        private int add_energy;
        /// <summary>
        /// 固定物品增减用(字符串表示 格式为 物品代码.lost/add)
        /// </summary>
        [JsonProperty]
        private List<string> add_item;
        /// <summary>
        /// 增加固定事件判断代码
        /// </summary>
        [JsonProperty]
        private List<string> add_events;

        /// <summary>
        /// 多次判定
        /// </summary>
        [JsonProperty]
        private List<RandomPart> random_parts;

        

        /// <summary>
        /// 执行这项选择
        /// </summary>
        /// <param name="playerModel">玩家数据模型</param>
        /// <returns></returns>
        public string ExecuteOption(PlayerModel playerModel)
        {
            //TODO:判断要加addevent
            int deltahp = add_health;
            int deltaenerfy = add_energy;
            List<string> add_Item = new List<string>();

            //如果没有物品变化就不管
            if (add_item != null)
                add_Item.AddRange(add_item);

            if (random_parts != null)
            {
                for (int i = 0; i < random_parts.Count; i++)
                {
                    //多次随机判定中的单次
                    if (random_parts[i].random_modifies == null)
                        continue;

                    int hptemp;
                    int energytemp;
                    List<string> itemtemp;

                    random_parts[i].ExecuteRandomPart(out hptemp, out energytemp, out itemtemp);

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

            Debug.Log("已执行选项deltahp: " + deltahp + " " + " deltaenerfy: " + deltaenerfy);
            //生成事件报告给玩家
            return ProduceReport(deltahp, deltaenerfy, add_Item);
        }

        //事件报告
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

                //获得物品信息
                ItemInfo itemInfo = ItemInfoManager.Instance.Get(id);
                string[] ItemName = null; //物品的名字

                //如果信息出错直接下一个
                if (itemInfo == null)
                {
                    Debug.LogError("没有找到该物品 已跳过：" + add_Item[i]);
                    continue;
                }
                else
                {
                    ItemName = itemInfo.ItemName.Split('#');
                }

                //gl 应该是 lost 获得 get
                string gl = null;
                //如果并没有分割出信息 （数据结构有问题）
                if (modify.Length > 0)
                {
                    gl = modify[1];
                }

                //获得语言
                int language = PlayerPrefs.GetInt("Language", (int)Config.Language.l_simp_chinese);

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
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("label: ");
            stringBuilder.Append(Label);
            stringBuilder.Append("\n");
            stringBuilder.Append("add_health: ");
            stringBuilder.Append(add_health);
            stringBuilder.Append("\n");
            stringBuilder.Append("add_energy: ");
            stringBuilder.Append(add_energy);
            stringBuilder.Append("\n");
            if (add_item != null)
            {
                for (int i = 0; i < add_item.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" add_item: ");
                    stringBuilder.Append(add_item[i]);
                    stringBuilder.Append("\n");
                }
            }
            if (this.add_events != null)
            {
                for (int i = 0; i < add_events.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" add_events: ");
                    stringBuilder.Append(add_events[i]);
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
        public List<RandomModify> random_modifies;

        public void ExecuteRandomPart(out int addHealth, out int addEnergy, out List<string> add_Item)
        {
            float judge = Random.value;

            float IntervalMax = 0;
            float IntervalMin = 0;

            for (int j = 0; j < random_modifies.Count; j++)
            {
                RandomModify randomModify = random_modifies[j];

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

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (random_modifies != null)
            {
                for (int i = 0; i < random_modifies.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append("random_modifies: ");
                    stringBuilder.Append(random_modifies[i].ToString());
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
        public float percentage;

        public int add_health;
        public int add_energy;
        public List<string> add_item;
        public List<string> add_events;

        public void ExecuteModify(out int addHealth, out int addEnergy, out List<string> add_Item)
        {
            add_Item = new List<string>();
            if (add_item != null)
                add_Item.AddRange(add_item);
            addHealth = add_health;
            addEnergy = add_energy;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("add_health: ");
            stringBuilder.Append(add_health);
            stringBuilder.Append("\n");
            stringBuilder.Append("add_energy: ");
            stringBuilder.Append(add_energy);
            stringBuilder.Append("\n");

            if (add_item != null)
            {
                for (int i = 0; i < add_item.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" add_item: ");
                    stringBuilder.Append(add_item[i]);
                    stringBuilder.Append("\n");
                }
            }
            if (this.add_events != null)
            {
                for (int i = 0; i < add_events.Count; i++)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append(" add_events: ");
                    stringBuilder.Append(add_events[i]);
                    stringBuilder.Append("\n");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
