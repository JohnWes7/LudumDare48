using FE_EventInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责生成事件，打开道路，每一关（一天）唯一，
/// </summary>
public class SingleDayManager : MonoBehaviour
{
    //单例索引
    public static SingleDayManager ToDay { get; set; }
    [Header("障碍物")]
    public List<Rigidbody2D> BarrierList;

    [Header("生成事件地点")]
    public List<Transform> EventPos;

    [Header("生成事件列表相关")]
    public List<EventController> Events = new List<EventController>();
    public GameObject EventPrefabs;

    private void Awake()
    {
        ToDay = this;
    }

    private void Start()
    {
        //一开始直接生成？或者要manager来调用
        GenerateEvent();

    }

    //负责生成事件
    public void GenerateEvent()
    {
        //事件个数
        int eventAmount = Random.Range(1, 3);

        //生成事件
        for (int i = 0; i < eventAmount; i++)
        {
            GenerateEvent(EventPos[i]);
        }

        DEBUG(this.Events);
    }

    public void GenerateEvent(Transform parent)
    {
        int index = 0;

        while (true)
        {
            //取得事件序号
            index = Random.Range(0, EventInfoManager.DayEventsCount);

            //判断能不能用
            if (PlayerModel.TryEvent(index) && IsNOTRepeat(index))
            {
                break;
            }
        }

        GameObject go = Instantiate<GameObject>(EventPrefabs, parent);   //生成事件并加入数组(用数组来判断能不能进下一关)
        go.transform.localPosition = Vector3.zero;                       //改位置

        EventController NewEvent1 = go.GetComponent<EventController>();
        NewEvent1.InIt(EventInfoManager.GetInfo(index));        //初始化生成的event

        Events.Add(NewEvent1);                                  //加入数组

    }


    /// <summary>
    /// 判断生成的事件是否重复
    /// </summary>
    /// <param name="index">需要判断的事件索引</param>
    /// <returns>判断结果</returns>
    public bool IsNOTRepeat(int index)
    {
        bool jugde = true;

        for (int i = 0; i < Events.Count; i++)
        {
            jugde = jugde && !(this.Events[i].GetID().Equals(EventInfoManager.GetInfo(index).Id));
        }

        return jugde;
    }

    private static void DEBUG(List<EventController> events)
    {
        string log = "";
        for (int i = 0; i < events.Count; i++)
        {
            log = log + "event" + i + ": " + events[i].GetID() + " ";
        }
        Debug.Log(log);
    }

    //开路
    public void TryOpenRoad()
    {
        bool isAllDone = true;

        for (int i = 0; i < Events.Count; i++)
        {
            isAllDone = isAllDone && Events[i].isDone;
        }

        //如果没有全部做完就直接返回
        if (!isAllDone)
        {
            return;
        }

        //全部解除静态
        for (int i = 0; i < BarrierList.Count; i++)
        {
            BarrierList[i].bodyType = RigidbodyType2D.Dynamic;
            BarrierList[i].AddForce(new Vector2(5, 0), ForceMode2D.Impulse);
        }

        //10秒后销毁
        Invoke("DestroyBarrier", 10);
    }
    //销毁障碍物
    public void DestroyBarrier()
    {
        //全部销毁
        for (int i = 0; i < BarrierList.Count; i++)
        {
            Destroy(BarrierList[i].gameObject);
        }

        //list清除
        BarrierList.Clear();
    }

}
