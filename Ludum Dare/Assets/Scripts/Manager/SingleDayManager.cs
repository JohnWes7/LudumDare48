using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDayManager : MonoBehaviour
{
    //单例索引
    public static SingleDayManager ToDay { get; set; }


    public List<Rigidbody2D> BarrierList;

    private void Awake()
    {
        ToDay = this;
        
    }

    private void Start()
    {
        OpenRoad();
    }

    public void GenerateEvent()
    {

    }

    public void OpenRoad()
    {
        //全部解除静态
        for (int i = 0; i < BarrierList.Count; i++)
        {
            BarrierList[i].bodyType = RigidbodyType2D.Dynamic;
            BarrierList[i].AddForce(new Vector2(5, 0), ForceMode2D.Impulse);
        }

        //10秒后销毁
        Invoke("DestroyBarrier", 10);
    }
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
