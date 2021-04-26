using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 主要负责跳转场景，以及动画
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    [Header("自我组件")]
    //public Animator mAnimator;
    public Image ChangePanel;
    public Text DayText;

    [Header("动画参数")]
    public float StartDuring = 1f;
    public float EndDuring = 1f;
    public GameObject mCanvas;

    [Header("玩家基本信息")]
    [SerializeField,Tooltip("统计天数（关卡）")]
    int GameDay;
    public int Hp;
    public int Energy;
    public int Score;
    public List<int> ExpEventsList;
    public List<int> ItemIDList;
    [SerializeField]
    private bool isDead = false;

    [Header("异步")]
    public AsyncOperation asyncOperation;




    private void Start()
    {
        //单例
        Instance = this;
        //不被销毁
        DontDestroyOnLoad(this);

        //初始数值
        GameDay = 1;

        //开始跳转
        string laber = "Day : " + GameDay.ToString();
        BeginFadeAni(() => {

            //完成开始动画后进行场景跳转
            BeginAsyncLoadSence(Config.GameSceneName, () => {

                //完成跳转后进行第一天初始化
                EndFadeAni();
                Level1InIt();

            });
        }, laber);
        
    }


    #region 动画相关

    public void BeginFadeAni(TweenCallback callback, string laber)
    {
        //如果已经有人在转场景了就取消这一次的
        if (asyncOperation != null)
        {
            return;
        }

        //打开
        mCanvas.SetActive(true);

        ChangePanel.DOKill(true);

        //跳转场景前
        ChangePanel.DOFade(1, StartDuring).OnComplete(callback);
        DayText.DOFade(1, StartDuring);
        DayText.text = "";
        DayText.DOText(laber, StartDuring).SetEase(Ease.Linear);
    }

    /// <summary>
    /// 结束fade动画
    /// </summary>
    public void EndFadeAni(TweenCallback callback = null)
    {

        ChangePanel.DOKill(true);

        //跳转场景后
        if (callback == null)
        {
            ChangePanel.DOFade(0, EndDuring);
        }
        else
        {
            ChangePanel.DOFade(0, EndDuring).OnComplete(callback);
        }

        DayText.DOFade(0, EndDuring).OnComplete(() => { mCanvas.SetActive(false);  });
    }

    #endregion

    /// <summary>
    /// 第一天初始化函数
    /// </summary>
    private void Level1InIt()
    {
        //初始数值
        Hp = 100;
        Energy = 100;
        Score = 0;
        ExpEventsList = new List<int>();
        ItemIDList = new List<int>();

        //拿初始数值更新UI
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        cbpc.UpdateHp(Hp, true);
        cbpc.UpdateEnergy(Energy, true);
        cbpc.UpdateDay(GameDay);
        cbpc.UpdateItems(ItemIDList);
    }

    //之后天数
    public void StartChangeToNextLevel()
    {
        //天数变化
        GameDay++;

        //开始跳转
        string laber = "Day : " + GameDay.ToString();
        BeginFadeAni(() => {

            //完成开始动画后进行场景跳转
            BeginAsyncLoadSence(Config.GameSceneName, () => {

                //完成跳转回调
                //结束动画
                EndFadeAni();

                //减少energy
                ChangeEnery(-5);

                //更新UI显示
                GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
                cbpc.UpdateDay(GameDay);
                cbpc.UpdateItems(ItemIDList);
                cbpc.UpdateHp(Hp, true);
            });

        }, laber);
    }

    /// <summary>
    /// 开始异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="unityAction"></param>
    public void BeginAsyncLoadSence(string sceneName, UnityAction unityAction)
    {
        StartCoroutine(AsyncLoadSence(sceneName, unityAction));
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="unityAction"></param>
    /// <returns></returns>
    public IEnumerator AsyncLoadSence(string sceneName, UnityAction unityAction)
    {
        //携程换场景
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        yield return asyncOperation;

        asyncOperation = null;

        //执行回调函数
        unityAction.Invoke();
    }




    public void ChangeEnery(int deltaEnergy)
    {
        //更改数值
        this.Energy = Mathf.Clamp(this.Energy + deltaEnergy, 0, 100);

        //更新ui显示
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        cbpc.UpdateEnergy(this.Energy);

        if (this.Energy <= 0)
        {
            Dead();
        }
    }

    public void ChangeHp(int deltaHp)
    {
        //更改数值
        this.Hp = Mathf.Clamp(this.Hp + deltaHp, 0, 100);

        //更新ui显示
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        cbpc.UpdateHp(this.Hp);

        if (this.Hp <= 0)
        {
            Dead();
        }
    }

    public void AddExpEvents(int event_Id)
    {
        ExpEventsList.Add(event_Id);
    }

    public void ChangeItemList(int item_change)
    {
        if (item_change == 0)
        {
            return;
        }
        else if (item_change < 0 )
        {
            ItemIDList.Remove(Mathf.Abs(item_change));
        }
        else if (item_change > 0)
        {
            ItemIDList.Add(item_change);
        }

        //更新显示
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        cbpc.UpdateItems(ItemIDList);

    }

    public bool TryEvent(int event_id)
    {
        
        EventInfo eventInfo = EventInfoManager.Instance.EventInfoList[event_id];

        //如果是0代表可重复出现
        if (eventInfo.Precondition == 0)
        {
            return true;
        }
        else
        {
            //如果这个事件之前出现过，就false
            bool judge1 = !ExpEventsList.Contains(event_id);
            bool judge2 = true;

            //如果是 需要条件 的 非开头 事件链 则判断满不满足
            if (eventInfo.Precondition > 0)
            {
                //如果没有满足这个的条件，false
                judge2 = ItemIDList.Contains(eventInfo.Precondition);
            }
            

            return judge1 && judge2;
        }

        return false;
    }


    private void Dead()
    {
        if (isDead)
        {
            return;
        }
        else
        {
            Debug.Log("dead");
            isDead = true;

            //人物不能移动
            if (PlayerController.Instance)
            {
                PlayerController.Instance.enabled = false;
            }
            
            //跳转场景到end
            BeginFadeAni(() =>
            {
                BeginAsyncLoadSence("EndScene", () =>
                {
                    //结束动画
                    EndFadeAni();

                    //计算分数
                    for (int i = 0; i < ItemIDList.Count; i++)
                    {
                        ItemInfo info = ItemInfoManager.Instance.Get(ItemIDList[i]);
                        this.Score += info.Score * 50;
                    }
                    this.Score += this.GameDay * 10;

                    //刷新页面
                    GameObject.Find("EndPanel").GetComponent<EndPanelController>().UpdateEndView(this.Score, this.GameDay);

                });

            }, "End...");
        }
    }

    public void DestroySelf()
    {
        GameManager.Instance = null;
        Destroy(gameObject);
    }
}
