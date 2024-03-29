﻿using FE_EventInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EventController : MonoBehaviour
{
    [Header("该事件属性"),SerializeField]
    private EventInfo eventInfo;
    public bool isDone;

    [Header("显示相关")]
    public GameObject ESign;
    public GameObject EventPanelPrefabs;
    public SpriteRenderer mSpriteRenderer;
    public EventPanelController EventPanel;

    public void InIt(EventInfo info)
    {
        //获取事件ID
        this.eventInfo = info;

        //改名字 
        gameObject.name = "event_" + this.eventInfo.Id;

        ChangeIcon(info.Icon);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //判断是玩家碰到了
        if (collision.tag == Config.PlayerTag)
        {
            PlayerController.Instance.SetSelectEvent(this);
            //显示标记
            ESign.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //判断是玩家碰到了
        if (collision.tag == Config.PlayerTag)
        {
            PlayerController.Instance.SetSelectEvent(null);
            //关闭标记
            ESign.SetActive(false);
        }

    }

    //player来检测和触发事件
    public void TriggerEvent()
    {
        if (EventPanel)
        {
            EventPanel.gameObject.SetActive(true);
        }
        else
        {
            //生成
            EventPanel = Instantiate<GameObject>(EventPanelPrefabs, GameObject.Find("Canvas").transform).GetComponent<EventPanelController>();

            //初始化
            EventPanel.InIt(this.eventInfo, this);
        }

        //播放动画
        EventPanel.StartAni();
    }

    /// <summary>
    /// 将时间标记为isdone （ui选择后回调，表示完成）
    /// </summary>
    /// <param name="isdone">是否完成，true表示事件完成</param>
    public void Done(bool isdone = true)
    {
        this.isDone = isdone;
        SingleDayManager.ToDay.TryOpenRoad();
    }

    public void ChangeIcon(string PATH)
    {
        //根据列表替换显示的图(这一句之后不会是赋值会是直接读取)

        
        string[] strName = PATH.Split('#'); //进行分割

        Sprite eventImage = null;   //图片变量

        //分割成功进行图集操作
        if (strName.Length > 1)
        {
            string path = Config.EventIconPath + strName[0];
            //导入要切的图
            Sprite[] eventSpriteAtlas = Resources.LoadAll<Sprite>(path);

            eventImage = eventSpriteAtlas[int.Parse(strName[1])];
        }
        //失败进行图片操作
        else
        {
            string path = Config.EventIconPath + strName[0];
            eventImage = Resources.Load<Sprite>(path);
        }


        //最后赋值显示
        if (eventImage)
        {
            mSpriteRenderer.sprite = eventImage;
        }
    }

    public string GetID()
    {
        return this.eventInfo.Id;
    }
}
