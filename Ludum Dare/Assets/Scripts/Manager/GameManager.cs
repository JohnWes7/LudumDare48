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

    

    [Header("异步")]
    public AsyncOperation asyncOperation;


    #region debug for Items
    //void Update()
    //{
    //    //用来直接生成几个遗物 测试UI用 不用管
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        ChangeItemList(13);
    //    }
    //    //删除遗物测试
    //    if (Input.GetKeyDown(KeyCode.H))
    //    {
    //        ChangeItemList(-13);
    //    }
    //}
    #endregion

    private void Start()
    {
        //单例
        Instance = this;
        //不被销毁
        DontDestroyOnLoad(this);

        //初始数值
        PlayerModel.Instance.InIt();

        //开始跳转
        string laber = "Day : " + PlayerModel.Instance.GameDay.ToString();
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
        //拿初始数值更新UI
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        cbpc.UpdatePlayerInfo(PlayerModel.Instance);
    }

    //之后天数
    public void StartChangeToNextLevel()
    {
        //天数变化
        PlayerModel.Instance.IncreaseDay();

        //开始跳转
        string laber = "Day : " + PlayerModel.Instance.GameDay.ToString();
        BeginFadeAni(() => {

            //完成开始动画后进行场景跳转
            BeginAsyncLoadSence(Config.GameSceneName, () => {

                //完成跳转回调
                //结束动画
                EndFadeAni();

                //减少energy
                PlayerModel.Instance.ChangeEnery(-5);

                //更新UI显示
                GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
                cbpc.UpdateDay(PlayerModel.Instance.GameDay);
                cbpc.UpdateItems(PlayerModel.Instance.ItemIDList);
                cbpc.UpdateHp(PlayerModel.Instance.Hp, true);
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

    public bool TryEvent(int index)
    {
        
        EventInfo eventInfo = EventInfoManager.Instance.GetInfo(index);

        //如果是0代表可重复出现
        if (eventInfo.Precondition == 0)
        {
            return true;
        }
        else
        {
            //如果这个事件之前出现过，就false
            bool judge1 = !PlayerModel.Instance.ExpEventsList.Contains(eventInfo.Id);
            bool judge2 = true;

            //如果是 需要条件 的 非开头 事件链 则判断满不满足
            if (eventInfo.Precondition > 0)
            {
                //与遗物来进行判断
                //如果没有满足这个的条件，false
                judge2 = PlayerModel.Instance.ItemIDList.Contains(eventInfo.Precondition);
            }
            

            return judge1 && judge2;
        }
    }


    public void Dead()
    {

        Debug.Log("dead");


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

                //算分
                PlayerModel.Instance.CaculateScore();

                //刷新页面
                GameObject.Find("EndPanel").GetComponent<EndPanelController>().UpdateEndView(PlayerModel.Instance.Score, PlayerModel.Instance.GameDay);

            });

        }, "End...");
    }

    public void DestroySelf()
    {
        GameManager.Instance = null;
        Destroy(gameObject);
    }
}
