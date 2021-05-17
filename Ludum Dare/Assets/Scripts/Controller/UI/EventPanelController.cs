using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EventPanelController : MonoBehaviour
{
    [Header("事件信息")]
    EventInfo evenInfo;
    [Header("UI元素")]
    public Text Title;
    public Text Desciption;

    [Header("Options选择项")]
    public Transform OptionsParent;
    public GameObject OptionPrefab;

    public List<OptionButtonController> OptionList;

    [Header("生成该panel的组件")]
    public EventController mEventController;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="Info"></param>
    /// <param name="mEventController"></param>
    public void InIt(EventInfo Info, EventController mEventController)
    {
        //改自身属性
        gameObject.name = "event_" + Info.ToString();
        this.evenInfo = Info;
        this.mEventController = mEventController;

        //获取语言
        int language = PlayerPrefs.GetInt("Language", (int)Config.Language.CH);

        //初始化显示


        //更改标题
        Title.text = evenInfo.Eventtitle.Split('#')[language];

        //更改描述
        Desciption.text = evenInfo.Desciption.Split('#')[language];

        //显示选项
        for (int i = 0; i < evenInfo.Options.Count; i++)
        {
            OptionButtonController newOption = Instantiate<GameObject>(OptionPrefab, OptionsParent).GetComponent<OptionButtonController>();
            newOption.InIt(evenInfo.Options[i]);
            OptionList.Add(newOption);
        }

    }

    /// <summary>
    /// 回退按键
    /// </summary>
    public void Back()
    {
        EndAni();
        PlayerController.Instance.OnEventPanelClose();
    }

    //old神必堡垒 解码器
    public void EventDecoder(string change)
    {
        string addDescip = "\n\n";

        if (change == "")
        {
            addDescip += "Nothing Happen....";
            goto end;
        }

        string[] events;
        int deltahp = 0;             //血量变化，最后一起加
        int deltafuel = 0;           //燃油变化
        double random;              //事件概率
        List<int> changeItemID = new List<int>();     //变化的物品


        events = change.Split('#');

        deltahp = int.Parse(events[0]);
        deltafuel = int.Parse(events[1]);

        for (int i = 2; i < events.Length; i = i + 4)
        {

            random = double.Parse(events[i]);// 判断事件是否发生
            if (random < Random.Range(0f, 1f))
            {
                continue;
            }

            deltahp += int.Parse(events[i + 1]);// 事件中的数值变化
            deltafuel += int.Parse(events[i + 2]);

            if (events[i + 3] != "0")
            {
                // 遗物的变化
                //yiwu.add(Integer.parseInt(events[i + 3]));
                int temp = int.Parse(events[i + 3]);

                //如果有变化再加入数组
                if (temp != 0)
                {
                    //GameManager.Instance.ChangeItemList(temp);
                    changeItemID.Add(temp);
                }
            }
        }

        //向数据加入
        //GameManager.Instance.ChangeEnery(deltafuel);
        //GameManager.Instance.ChangeHp(deltahp);


        //结束报告
        {
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
            if (deltafuel > 0)
            {
                addDescip += "Fuel: +" + deltafuel.ToString() + "\n";
            }
            else if (deltafuel < 0)
            {
                addDescip += "Fuel: " + deltafuel.ToString() + "\n";
            }
        }

        {
            //遗物
            bool judgelost = false;
            bool judgeget = false;
            string get = "Get item : ";
            string lost = "Lost item : ";

            //对get lost 进行操作
            for (int i = 0; i < changeItemID.Count; i++)
            {
                Debug.Log("changeItemID " + i + " : " + changeItemID[i]);
                string[] name = ItemInfoManager.Instance.Get(Mathf.Abs(changeItemID[i])).ItemName.Split('#');
                int language = PlayerPrefs.GetInt("Language", (int)Config.Language.CH);

                if (changeItemID[i] > 0)
                {
                    judgeget = true;
                    get += name[language] + " ";
                }
                else if (changeItemID[i] < 0)
                {
                    judgelost = true;
                    lost += name[language] + " ";
                }

            }
            get += "\n";
            lost += "\n";


            //如果有get
            if (judgeget)
            {
                addDescip += get;
            }
            //如果有lost
            if (judgelost)
            {
                addDescip += lost;
            }
            addDescip += "\n";

        }


        Debug.Log("deltafuel:" + deltafuel + "  deltahp:" + deltahp);

    //空语句直接在这边开始执行
    end:

        //更改Desciption
        ChangeDesciption(Desciption.text + addDescip);

        //事件完成标记
        mEventController.SetIsDone(true);
    }

    public void ChangeDesciption(string desciption)
    {
        this.Desciption.text = desciption;
    }

    //动画
    public void StartAni()
    {
        transform.DOKill(true);
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        transform.DOScale(Vector3.one, 0.2f);
    }
    
    public void EndAni()
    {
        transform.DOKill(true);
        transform.DOScale(new Vector3(0.5f, 0.5f, 1), 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }



}