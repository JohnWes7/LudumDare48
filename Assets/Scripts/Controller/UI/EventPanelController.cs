using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using FE_EventInfo;

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
        gameObject.name = "event_" + Info.Id;
        this.evenInfo = Info;
        this.mEventController = mEventController;

        //获取语言
        Config.Language language = (Config.Language)PlayerPrefs.GetInt("Language", (int)Config.Language.l_simp_chinese);


        //更改标题
        Title.text = TextManage.Instance.GetText(Info.Event_title, language);

        //更改描述
        Desciption.text = TextManage.Instance.GetText(Info.Description, language);

        //显示选项
        for (int i = 0; i < evenInfo.Options.Count; i++)
        {
            Option option = evenInfo.Options[i];
            OptionButtonController newOption = Instantiate<GameObject>(OptionPrefab, OptionsParent).GetComponent<OptionButtonController>();
            newOption.InIt(option);
            newOption.button.onClick.AddListener(() => { Execute(option, evenInfo.Event_chain); });
            OptionList.Add(newOption);
        }

    }

    public void Execute(Option option, string chain)
    {
        string addDes = option.ExecuteOption(PlayerModel.Instance, chain);
        AddDescription(addDes);
        CloseOption();
        Done();
    }


    /// <summary>
    /// 回退按键
    /// </summary>
    public void Back()
    {
        EndAni();
        PlayerController.Instance.OnEventPanelClose();
    }


    /// <summary>
    /// 添加description
    /// </summary>
    /// <param name="addDescription">将添加的值</param>
    public void AddDescription(string addDescription)
    {
        Desciption.text += addDescription;
    }

    /// <summary>
    /// 直接改变description
    /// </summary>
    /// <param name="desciption"></param>
    public void ChangeDesciption(string desciption)
    {
        this.Desciption.text = desciption;
    }

    /// <summary>
    /// 关闭选项选择
    /// </summary>
    public void CloseOption()
    {
        OptionsParent.gameObject.SetActive(false);
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

    public void Done()
    {
        mEventController.Done();
    }

}