﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EventController : MonoBehaviour
{
    [Header("该事件属性")]
    public int EventId;
    public bool isDone;

    [Header("显示相关")]
    public GameObject ESign;
    public GameObject EventPanelPrefabs;
    public SpriteRenderer mSpriteRenderer;
    public EventPanelController EventPanel;

    public void InIt(int EventId)
    {
        //获取事件ID
        this.EventId = EventId;

        //初始化
        {
            //改名字 
            gameObject.name = "event_" + this.EventId;

            //根据列表替换显示的图(这一句之后不会是赋值会是直接读取)
            string propertyImageName = EventInfoManager.Instance.EventInfoList[this.EventId].Icon;
            
            //进行操作
            string[] strName = propertyImageName.Split('#');
            Sprite eventImage = null;

            //Debug.Log(strName.Length);

            if (strName.Length > 1)
            {
                string path = Config.EventIconPath + strName[0];
                Sprite[] eventSpriteAtlas = Resources.LoadAll<Sprite>(path);

                eventImage = eventSpriteAtlas[int.Parse(strName[1])];
            }
            else
            {
                string path = Config.EventIconPath + strName[0];
                //Debug.Log(path);
                eventImage = Resources.Load<Sprite>(path);
            }

            if (eventImage)
            {
                mSpriteRenderer.sprite = eventImage;
            }
        }
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
            EventPanel.InIt(this.EventId, this);
        }

        //播放动画
        EventPanel.StartAni();
    }

    public void SetIsDone(bool isdone)
    {
        this.isDone = isdone;
        SingleDayManager.ToDay.TryOpenRoad();
    }
}