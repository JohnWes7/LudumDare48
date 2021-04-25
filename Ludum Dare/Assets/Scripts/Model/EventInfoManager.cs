using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class EventInfoManager : Single<EventInfoManager>
{
    /// <summary>
    /// 存有所有事件数据的列表
    /// </summary>
    public List<EventInfo> EventInfoList;

    public EventInfoManager()
    {
        //读取json
        string json = Resources.Load<TextAsset>(Config.EventInfoJsonPath).text;
        //转换成list
        EventInfoList = JsonMapper.ToObject<List<EventInfo>>(json);

        #region Debug打印
        //Debug.Log(json);

        //for (int i = 0; i < EventInfoList.Count; i++)
        //{
        //    Debug.Log("Id:" + EventInfoList[i].Id + " Eventtitle:" + EventInfoList[i].Eventtitle + " Desciption:" + EventInfoList[i].Desciption + " Choice1:" + EventInfoList[i].Choice1 + " Choice1Change:" + EventInfoList[i].Choice1Change + " Choice2:"
        //        + EventInfoList[i].Choice2 + " Choice2Change:" + EventInfoList[i].Choice2Change + " Choice3:" + EventInfoList[i].Choice3 + " Choice3Change:" + EventInfoList[i].Choice3Change);
        //} 
        #endregion

    }


}

public class EventInfo
{
    public int Id;
    public string Eventtitle;
    public string Desciption;
    public string Choice1;
    public string Choice1Change;
    public string Choice2;
    public string Choice2Change;
    public string Choice3;
    public string Choice3Change;
}

