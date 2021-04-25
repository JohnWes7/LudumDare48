using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class ButtomScaleAni : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    public bool IsOnButtom = false;
    public bool IsDown = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsOnButtom = true;
        //判断有没有按下
        if (IsDown)
        {
            ToSmallState();
        }
        else
        {
            ToBigState();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsOnButtom = false;

        ToNormalState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDown = true;

        ToSmallState();
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        IsDown = false;

        if (IsOnButtom)
        {
            ToBigState();
        }
    }

    public void ToBigState()
    {
        transform.DOScale(1.1f, 0.1f);
    }
    public void ToNormalState()
    {
        transform.DOScale(1f, 0.1f);
    }
    public void ToSmallState()
    {
        transform.DOScale(0.9f, 0.1f);
    }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    Debug.Log("开拖");
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    gameObject.transform.position = eventData.position;
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    Debug.Log("拖完了");
    //}
}

//public class father
//{
//    public int a = 5;
//    public float b = 0.8f;

//    public void test()
//    {

//    } 
//}

//public interface IAttack
//{
//    void Attack(int a);
//}

//public class child2 : father, IAttack
//{
//    public int c = 6;

//    public void Attack(int a)
//    {
//        Debug.Log("扣血");
//    }
//}

//public class child3 : IAttack
//{
//    public void Attack(int a)
//    {
//        Debug.Log("流血");
//    }

//    public void judge(Object oj)
//    {
//        if (oj is IAttack)
//        {
//            Debug.Log("他能进行攻击");
//        }
//    }
//}