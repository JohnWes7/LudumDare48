using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameBasePanelController : MonoBehaviour
{
    [Header("更新血量，能量相关")]
    public Transform Heath;

    public Text HpText;
    public Image HpBar;
    public Text EnergyText;
    public Image EnergyBar;

    private Coroutine AddHpCoroutine;
    private Coroutine AddEnergyCoroutine;

    [Tooltip("lerp等待fixed帧数")]
    public int LerpTimes = 15;
    public int shakeStrength = 30;
    public int shakeVibrato = 30;


    [Header("更新round显示")]
    public Text RoundAmoungText;

    #region TestUpdateHp
    //public int testhp;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        UpdateEnergy(testhp);
    //    }
    //}


    //private void Start()
    //{
    //    Type t = this.GetType();
    //    if (t == null)
    //    {
    //        Debug.Log("t = null");
    //    }

    //    System.Reflection.MethodInfo m = t.GetMethod("Debug1");
    //    if (m == null)
    //    {
    //        Debug.Log("m = null");
    //    }

    //    m.Invoke(this, null);
    //    //        Invoke函数的原型如下：
    //    //publicObject Invoke(Objectobj, Object[] parameters)
    //    //第一个参数为对其调用方法或构造函数的对象。如果方法是静态的，则应为null，否则必需给出一个实例，若在同一类中调用，则可指定为this。
    //    //第二个参数为调用的方法的参数列表。这是一个对象数组，这些对象与要调用的方法或构造函数的参数具有相同的数量、顺序和类型。
    //    //如果没有任何参数，则 parameters应为nullNothingnullptrnull引用。
    //    //函数的返回值便为所调用的函数的返回值，若无返回值，则为null。在获取返回值前应进行强制类型转换。
    //}

    //public void Debug1()
    //{
    //    Debug.Log("1111");
    //}
    #endregion

    public static GameBasePanelController Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHp(int hp, bool directly = false)
    {
        if (directly)
        {
            ChangeHp(hp);//直接更改数值和条
            return;
        }

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
            Heath.DOShakePosition(0.2f, shakeStrength, shakeVibrato);

            ChangeHp(hp);//直接更改数值和条
        }
        //等于直接什么都不干
    }

    public void ChangeHp(int hp)
    {
        HpBar.fillAmount = (float)hp / 100f;    //更改滑动条
        HpText.text = hp.ToString();            //更改文字
    }

    public void UpdateEnergy(int energy, bool directly = false)
    {
        if (directly)
        {
            ChangeEnergy(energy);
            return;
        }

        //取得差值
        int deltaEnergy = energy - int.Parse(EnergyText.text);

        if (deltaEnergy > 0)
        {
            //如果有动画直接取消掉
            if (AddEnergyCoroutine != null)
            {
                StopCoroutine(AddEnergyCoroutine);
            }

            AddEnergyCoroutine = StartCoroutine(AddEnergy(energy));
        }
        else if (deltaEnergy < 0)
        {
            //抖动
            Heath.DOShakePosition(0.2f, shakeStrength, shakeVibrato);

            ChangeEnergy(energy);
        }


    }

    public void ChangeEnergy(int energy)
    {
        EnergyBar.fillAmount = (float)energy / 100f;    //更改能量条
        EnergyText.text = energy.ToString();    //更改文字
    }

    public void UpdateLevel(int level)
    {
        RoundAmoungText.text = level.ToString();
    }

    IEnumerator AddHp(int hp)
    {
        for (int i = 0; i < LerpTimes; i++)
        {
            //取得当前的int值
            int nowhp = int.Parse(HpText.text);
            //与目标值做插值
            nowhp = ((int)Mathf.Lerp(nowhp, hp, 0.1f));
            ChangeHp(nowhp);

            //停顿一帧
            yield return new WaitForFixedUpdate();
        }

        //最后直接赋值
        ChangeHp(hp);
    }

    IEnumerator AddEnergy(int energy)
    {
        for (int i = 0; i < LerpTimes; i++)
        {
            //取得当前的int值
            int nowEnergy = int.Parse(EnergyText.text);
            //与目标值做插值
            nowEnergy = ((int)Mathf.Lerp(nowEnergy, energy, 0.1f));
            ChangeEnergy(nowEnergy);

            //停顿一帧
            yield return new WaitForFixedUpdate();
        }

        //最后直接赋值
        ChangeEnergy(energy);
    }
}
