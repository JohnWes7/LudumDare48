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
        int EventAmount = Random.Range(1, 2);
        int rEventID1 = 0;
        int rEventID2 = 0;
        string id1 = "";
        string id2 = "";

        //生成第一个事件
        if (EventAmount > 0)
        {
            //Debug.Log("生成第一个事件");
            while (true)
            {
                //取得事件
                rEventID1 = Random.Range(0, EventInfoManager.Instance.EventInfoListCount);

                //判断能不能用
                if (GameManager.Instance.TryEvent(rEventID1,out id1))
                {
                    break;
                }
            }

            //生成事件并加入数组(用数组来判断能不能进下一关)
            GameObject go = Instantiate<GameObject>(EventPrefabs, EventPos[0]);
            go.transform.localPosition = Vector3.zero;                  //改位置

            EventController NewEvent1 = go.GetComponent<EventController>();
            Events.Add(NewEvent1);                                      //加入数组

            //初始化生成的event
            NewEvent1.InIt(EventInfoManager.Instance.GetInfo(id1));
            //计入经历过的事件
            PlayerModel.Instance.AddExpEvents(id1);
        }

        //生成第2个事件
        if (EventAmount > 1)
        {
            //Debug.Log("生成第2个事件");
            while (true)
            {
                rEventID2 = Random.Range(0, EventInfoManager.Instance.EventInfoListCount);
                if (rEventID2 != rEventID1)
                {
                    //判断能不能用
                    if (GameManager.Instance.TryEvent(rEventID2,out id2))
                    {
                        break;
                    }
                }
            }

            //生成事件并加入数组(用数组来判断能不能进下一关)
            GameObject go = Instantiate<GameObject>(EventPrefabs, EventPos[1]); //生成在了第二个位置
            go.transform.localPosition = Vector3.zero;                          //改位置
            EventController NewEvent2 = go.GetComponent<EventController>();
            Events.Add(NewEvent2);                                                     //加入数组

            //初始化生成的event
            NewEvent2.InIt(EventInfoManager.Instance.GetInfo(rEventID2));
            PlayerModel.Instance.AddExpEvents(id2);
        }

        
        Debug.Log("EventAmount:" + EventAmount + "  rEventID1:" + id1 + " rEventID2:" + id2);
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
