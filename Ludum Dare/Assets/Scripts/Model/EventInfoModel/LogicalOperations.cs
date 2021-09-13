using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FE_EventInfo
{

    /// <summary>
    /// 抽象类注意不能实例化 规范所有逻辑运算判定
    /// </summary>
    public abstract class LogicalOperations
    {
        public List<string> have_events;
        public And AND;
        public Or OR;
        public Nor NOR;

        public abstract bool Perform();
    }

    public class Precondition : LogicalOperations
    {
        public override bool Perform()
        {
            return true;
        }
    }

    public class And : LogicalOperations
    {
        public override bool Perform()
        {
            return true;
        }
    }

    public class Or : LogicalOperations
    {
        public override bool Perform()
        {
            return true;
        }
    }

    public class Nor : LogicalOperations
    {
        public override bool Perform()
        {
            return true;
        }
    }
}


