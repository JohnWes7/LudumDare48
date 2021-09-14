using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

namespace FE_EventInfo
{

    /// <summary>
    /// 抽象类注意不能实例化 规范所有逻辑运算判定
    /// </summary>
    public abstract class LogicalOperations
    {
        [JsonProperty]
        protected List<string> has_event;
        [JsonProperty]
        protected And AND;
        [JsonProperty]
        protected Or OR;
        [JsonProperty]
        protected Nor NOR;

        public List<string> Has_event { get => has_event; set => has_event = value; }
        public And and { get => AND; set => AND = value; }
        public Or or { get => OR; set => OR = value; }
        public Nor nor { get => NOR; set => NOR = value; }

        public abstract bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain);
        public abstract bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain, out List<string> log);
    }

    public class Precondition : LogicalOperations
    {
        public static void AutoLogTool(ref List<string> log, List<bool> jugde_list, List<string> have_events)
        {
            for (int i = 0; i < have_events.Count; i++)
            {
                log.Add(jugde_list[i] + " : " + have_events[i] + "\n");
            }
        }
        public static void AutoLogTool(ref List<string> log, List<string> next_log)
        {
            for (int i = 0; i < next_log.Count; i++)
            {
                log.Add("\t" + next_log[i]);
            }
        }

        public override bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain, out List<string> log)
        {
            log = new List<string>();

            bool judge = true;
            // 先判断自己的have event (Precondition自己直接为 AND 全部为真)
            if (this.has_event != null)
            {
                List<bool> jugde_list = new List<bool>();

                for (int i = 0; i < this.has_event.Count; i++)
                {
                    string str = Has_event[i];

                    bool per_judge = playerEventsStatistics.Contains(chain == null ? "null" : chain, str);
                    jugde_list.Add(per_judge);

                    judge = judge && per_judge;
                }

                log.Add(judge + " : Per全部为真\n");
                Precondition.AutoLogTool(ref log, jugde_list, Has_event);
            }

            //判断AND
            if (this.AND != null)
            {
                List<string> and_log;
                bool and_judge = this.AND.Perform(playerEventsStatistics, chain, out and_log);

                log.Add(and_judge + " : AND全部为真\n");
                Precondition.AutoLogTool(ref log, and_log);

                judge = judge && and_judge;
            }

            //判断OR
            if (this.OR != null)
            {
                List<string> or_log;
                bool or_judge = this.OR.Perform(playerEventsStatistics, chain, out or_log);

                log.Add(or_judge + " : OR下一项为真\n");
                Precondition.AutoLogTool(ref log, or_log);

                judge = judge && or_judge;
            }

            //判断NOR
            if (this.NOR != null)
            {
                List<string> nor_log;
                bool nor_judge = this.NOR.Perform(playerEventsStatistics, chain, out nor_log);

                log.Add(nor_judge + " : NOR全不为真\n");
                Precondition.AutoLogTool(ref log, nor_log);

                judge = judge && nor_judge;
            }

            return judge;
        }
        public override bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain)
        {
            List<string> log;
            return Perform(playerEventsStatistics, chain, out log);
        }
    }

    public class And : LogicalOperations
    {
        public override bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain, out List<string> log)
        {
            log = new List<string>();

            bool judge = true;
            // 先判断自己的have event (AND 全部为真)
            if (this.has_event != null)
            {
                List<bool> jugde_list = new List<bool>();

                for (int i = 0; i < this.has_event.Count; i++)
                {
                    string str = Has_event[i];

                    bool per_judge = playerEventsStatistics.Contains(chain == null ? "null" : chain, str);
                    jugde_list.Add(per_judge);

                    judge = judge && per_judge;
                }

                Precondition.AutoLogTool(ref log, jugde_list, Has_event);
            }

            //判断AND
            if (this.AND != null)
            {
                List<string> and_log;
                bool and_judge = this.AND.Perform(playerEventsStatistics, chain, out and_log);

                log.Add(and_judge + " : AND全部为真\n");
                Precondition.AutoLogTool(ref log, and_log);

                judge = judge && and_judge;
            }

            //判断OR
            if (this.OR != null)
            {
                List<string> or_log;
                bool or_judge = this.OR.Perform(playerEventsStatistics, chain, out or_log);

                log.Add(or_judge + " : OR下一项为真\n");
                Precondition.AutoLogTool(ref log, or_log);

                judge = judge && or_judge;
            }

            //判断NOR
            if (this.NOR != null)
            {
                List<string> nor_log;
                bool nor_judge = this.NOR.Perform(playerEventsStatistics, chain, out nor_log);

                log.Add(nor_judge + " : NOR全不为真\n");
                Precondition.AutoLogTool(ref log, nor_log);

                judge = judge && nor_judge;
            }

            return judge;
        }

        public override bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain)
        {
            List<string> log;
            return Perform(playerEventsStatistics, chain, out log);
        }
    }

    public class Or : LogicalOperations
    {
        public override bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain)
        {
            List<string> log;
            return Perform(playerEventsStatistics, chain, out log);
        }

        public override bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain, out List<string> log)
        {
            log = new List<string>();

            bool judge = false;
            // 先判断自己的have event (OR 一项为真)
            if (this.has_event != null)
            {
                List<bool> jugde_list = new List<bool>();

                for (int i = 0; i < this.has_event.Count; i++)
                {
                    string str = Has_event[i];

                    bool per_judge = playerEventsStatistics.Contains(chain == null ? "null" : chain, str);
                    jugde_list.Add(per_judge);

                    judge = judge || per_judge;
                }

                Precondition.AutoLogTool(ref log, jugde_list, Has_event);
            }

            //判断AND
            if (this.AND != null)
            {
                List<string> and_log;
                bool and_judge = this.AND.Perform(playerEventsStatistics, chain, out and_log);

                log.Add(and_judge + " : AND全部为真\n");
                Precondition.AutoLogTool(ref log, and_log);

                judge = judge || and_judge;
            }

            //判断OR
            if (this.OR != null)
            {
                List<string> or_log;
                bool or_judge = this.OR.Perform(playerEventsStatistics, chain, out or_log);

                log.Add(or_judge + " : OR下一项为真\n");
                Precondition.AutoLogTool(ref log, or_log);

                judge = judge || or_judge;
            }

            //判断NOR
            if (this.NOR != null)
            {
                List<string> nor_log;
                bool nor_judge = this.NOR.Perform(playerEventsStatistics, chain, out nor_log);

                log.Add(nor_judge + " : NOR全不为真\n");
                Precondition.AutoLogTool(ref log, nor_log);

                judge = judge || nor_judge;
            }

            return judge;
        }
    }

    public class Nor : LogicalOperations
    {
        public override bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain)
        {
            List<string> log;
            return Perform(playerEventsStatistics, chain, out log);
        }

        public override bool Perform(PlayerEventsStatistics playerEventsStatistics, string chain, out List<string> log)
        {
            log = new List<string>();

            bool judge = true;
            // 先判断自己的have event (NOR 全为否)
            if (this.has_event != null)
            {
                List<bool> jugde_list = new List<bool>();

                for (int i = 0; i < this.has_event.Count; i++)
                {
                    string str = Has_event[i];

                    bool per_judge = playerEventsStatistics.Contains(chain == null ? "null" : chain, str);
                    jugde_list.Add(per_judge);

                    judge = judge && !per_judge;
                }

                Precondition.AutoLogTool(ref log, jugde_list, Has_event);
            }

            //判断AND
            if (this.AND != null)
            {
                List<string> and_log;
                bool and_judge = this.AND.Perform(playerEventsStatistics, chain, out and_log);

                log.Add(and_judge + " : AND全部为真\n");
                Precondition.AutoLogTool(ref log, and_log);

                judge = judge && !and_judge;
            }

            //判断OR
            if (this.OR != null)
            {
                List<string> or_log;
                bool or_judge = this.OR.Perform(playerEventsStatistics, chain, out or_log);

                log.Add(or_judge + " : OR下一项为真\n");
                Precondition.AutoLogTool(ref log, or_log);

                judge = judge && !or_judge;
            }

            //判断NOR
            if (this.NOR != null)
            {
                List<string> nor_log;
                bool nor_judge = this.NOR.Perform(playerEventsStatistics, chain, out nor_log);

                log.Add(nor_judge + " : NOR全不为真\n");
                Precondition.AutoLogTool(ref log, nor_log);

                judge = judge && !nor_judge;
            }

            return judge;
        }
    }
}


