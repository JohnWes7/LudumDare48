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

    [Header("基本信息")]
    [SerializeField,Tooltip("统计天数（关卡）")]
    int GameDay;
    public int Hp;
    public int Energy;


    private void Start()
    {
        //单例
        Instance = this;
        //不被销毁
        DontDestroyOnLoad(this);

        //初始数值
        GameDay = 1;

        //开始跳转
        BeginFadeAni(() => {
            //完成开始动画后进行场景跳转
            BeginAsyncLoadSence(Config.GameSceneName, () => {
                //完成跳转后进行第一天初始化
                EndFadeAni();
                Level1InIt();
            });
        });
        
    }

    private void Level1InIt()
    {
        //初始数值
        Hp = 100;
        Energy = 100;

        //拿初始数值更新UI
        GameBasePanelController cbpc = GameObject.Find("Canvas/GameBasePanel").GetComponent<GameBasePanelController>();
        cbpc.UpdateHp(Hp, true);
        cbpc.UpdateEnergy(Energy, true);
        cbpc.UpdateLevel(GameDay);
    }

    public void StartNextLevel()
    {
        //天数变化
        GameDay++;

        //开始跳转
        BeginFadeAni(() => {

            //完成开始动画后进行场景跳转
            BeginAsyncLoadSence(Config.GameSceneName, () => {

                //完成跳转回调
                //结束动画
                EndFadeAni();
                //生成第二天的物品
                GenerateEvent();

            });

        });
    }

    /// <summary>
    /// 固定位置生成事件
    /// </summary>
    public void GenerateEvent()
    {

    }

    /// <summary>
    /// 开始fade动画
    /// </summary>
    /// <param name="callback"></param>
    private void BeginFadeAni(TweenCallback callback)
    {
        //跳转场景前
        ChangePanel.DOFade(1, StartDuring).OnComplete(callback);
        Day.DOFade(1, StartDuring);
        Day.text = "";
        Day.DOText("DAY : " + GameDay, StartDuring);
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
    /// 结束fade动画
    /// </summary>
    private void EndFadeAni()
    {
        //跳转场景后
        ChangePanel.DOFade(0, EndDuring);
        Day.DOFade(0, EndDuring);
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="unityAction"></param>
    /// <returns></returns>
    public IEnumerator AsyncLoadSence(string sceneName, UnityAction unityAction)
    {//携程换场景
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        yield return asyncOperation;

        //执行回调函数
        unityAction.Invoke();
    }


}
