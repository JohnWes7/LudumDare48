using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System;

public class GameBasePanelController : MonoBehaviour
{

    //血量有更新关
    public Transform Heath;

    public Text HpText;
    public Image HpBar;
    public Text EnergyText;
    public Image EnergyBar;

    private Coroutine AddHpCoroutine;
    private Coroutine AddEnergyCoroutine;

    [Tooltip("lerp等待fixed帧数")]
    public int LerpTimes = 15;

    #region TestUpdateHp
    //public int testhp;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        UpdateHp(testhp);
    //    }
    //} 
    #endregion

    private void Start()
    {
        Type t = this.GetType();
        if (t == null)
        {
            Debug.Log("t = null");
        }

        System.Reflection.MethodInfo m = t.GetMethod("Debug1");
        if (m == null)
        {
            Debug.Log("m = null");
        }

        m.Invoke(this, null);
        //        Invoke函数的原型如下：
        //publicObject Invoke(Objectobj, Object[] parameters)
        //第一个参数为对其调用方法或构造函数的对象。如果方法是静态的，则应为null，否则必需给出一个实例，若在同一类中调用，则可指定为this。
        //第二个参数为调用的方法的参数列表。这是一个对象数组，这些对象与要调用的方法或构造函数的参数具有相同的数量、顺序和类型。
        //如果没有任何参数，则 parameters应为nullNothingnullptrnull引用。
        //函数的返回值便为所调用的函数的返回值，若无返回值，则为null。在获取返回值前应进行强制类型转换。
    }

    public void Debug1()
    {
        Debug.Log("1111");
    }

    public void UpdateHp(int hp)
    {
        //取得差值
        int deltaHP = hp - int.Parse(HpText.text);

        //如果增加
        if (deltaHP > 0)
        {
            //如果有动画直接取消掉
            if (AddHpCoroutine != null)
            {
                StopCoroutine(AddHpCoroutine);
            }

            //添加新的
            AddHpCoroutine = StartCoroutine(AddHp(hp));
        }
        //如果减少
        else if (deltaHP < 0)
        {
            //抖动
            Heath.DOShakePosition(0.2f);

            HpBar.fillAmount = (float)hp / 100f;    //更改滑动条
            HpText.text = hp.ToString();            //更改文字
        }
        //等于直接什么都不干
    }

    public void UpdateEnergy(int energy)
    {
        if (energy > 0)
        {

        }

        EnergyBar.fillAmount = energy / 100;
        EnergyText.text = energy.ToString();
    }

    IEnumerator AddHp(int hp)
    {
        for (int i = 0; i < LerpTimes; i++)
        {
            //取得当前的int值
            int nowhp = int.Parse(HpText.text);
            //与目标值做插值
            nowhp = ((int)Mathf.Lerp(nowhp, hp, 0.1f));
            HpBar.fillAmount = (float)nowhp / 100f;
            HpText.text = nowhp.ToString();

            //停顿一帧
            yield return new WaitForFixedUpdate();
        }

        //最后直接赋值
        HpText.text = hp.ToString();
        HpBar.fillAmount = (float)hp / 100f;
    }
}
