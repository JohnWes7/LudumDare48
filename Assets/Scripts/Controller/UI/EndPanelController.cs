using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPanelController : MonoBehaviour
{
    public Text ScoreNumber;
    public Text DayNumber;

    public void UpdateEndView(int score, int day)
    {
        ScoreNumber.text = score.ToString();
        DayNumber.text = day.ToString();
    }

    public void Back()
    {
        string laber = "Going Back...";
        GameManager.Instance.BeginFadeAni(() => 
        {

            //完成开始动画后进行场景跳转
            GameManager.Instance.BeginAsyncLoadSence(Config.StartSceneName, () => 
            {

                //完成跳转回调
                //结束动画
                GameManager.Instance.EndFadeAni(() => 
                { 
                    //结束manager
                    GameManager.Instance.DestroySelf(); 
                });
            });

        }, laber);
    }
}
