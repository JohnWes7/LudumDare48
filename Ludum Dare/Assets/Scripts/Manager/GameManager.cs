using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    [Header("自我组件")]
    //public Animator mAnimator;
    public Image ChangePanel;
    public Text Day;

    [Header("动画参数")]
    public float StartDuring = 1f;
    public float EndDuring = 1f;
    public GameObject mCanvas;

    [Header("基本信息")]
    [SerializeField,Tooltip("统计天数（关卡）")]
    int GameDay;
    public int Hp;
    public int Energy;

    [Header("ESC")]
    public GameObject ESCPanel;


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

    private void Update()
    {
        ESCPart();
    }

    #region 动画相关

    private void BeginFadeAni(TweenCallback callback, string laber)
    {
        //打开
        mCanvas.SetActive(true);

        //跳转场景前
        ChangePanel.DOFade(1, StartDuring).OnComplete(callback);
        Day.DOFade(1, StartDuring);
        Day.text = "";
        Day.DOText(laber, StartDuring).SetEase(Ease.Linear);
    }

    /// <summary>
    /// 结束fade动画
    /// </summary>
    private void EndFadeAni(TweenCallback callback = null)
    {
        //跳转场景后
        if (callback == null)
        {
            ChangePanel.DOFade(0, EndDuring);
        }
        else
        {
            ChangePanel.DOFade(0, EndDuring).OnComplete(callback);
        }

        Day.DOFade(0, EndDuring).OnComplete(() => { mCanvas.SetActive(false); });
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

        //拿初始数值更新UI
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        cbpc.UpdateHp(Hp, true);
        cbpc.UpdateEnergy(Energy, true);
        cbpc.UpdateDay(GameDay);
    }

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
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        yield return asyncOperation;

        //执行回调函数
        unityAction.Invoke();
    }


    #region ESC弹窗系统

    private void ESCPart()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ESCPanel == null)
            {
                //生成
                GameObject esc = Resources.Load<GameObject>(Config.ESCPanelPath);
                ESCPanel = Instantiate<GameObject>(esc, GameObject.Find("Canvas").transform);
            }
            else
            {
                Destroy(ESCPanel);
                ESCPanel = null;
            }
        }
    }

    public void ESCYesCallBack()
    {
        string laber = "Going Back...";
        BeginFadeAni(() => {

            //完成开始动画后进行场景跳转
            BeginAsyncLoadSence(Config.StartSceneName, () => {

                //完成跳转回调
                //结束动画
                EndFadeAni(() => { Destroy(gameObject); });
            });

        }, laber);
    }

    #endregion


}
