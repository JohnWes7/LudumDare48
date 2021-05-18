using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EventController : MonoBehaviour
{
    [Header("该事件属性")]
    public EventInfo eventInfo;
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

        //初始化
        {
            //改名字 
            gameObject.name = "event_" + this.eventInfo.Id;

            //根据列表替换显示的图(这一句之后不会是赋值会是直接读取)
            string propertyImageName = eventInfo.Icon;
            
            //进行操作
            string[] strName = propertyImageName.Split('#');
            Sprite eventImage = null;

            //Debug.Log(strName.Length);

            if (strName.Length > 1)
            {
                string path = Config.EventIconPath + strName[0];
                //导入要切的图
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
}
