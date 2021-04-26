using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoManager : Single<ItemInfoManager>
{
    public List<ItemInfo> ItemInfoList { get; }

    public ItemInfoManager()
    {
        //读取json
        string json = Resources.Load<TextAsset>(Config.ItemInfoJsonPath).text;
        //转换成list
        ItemInfoList = JsonMapper.ToObject<List<ItemInfo>>(json);

        #region debug
        //Debug.Log(json);

        #endregion
    }

    public ItemInfo Get(int item_id)
    {
        return ItemInfoList[item_id - 1];
    }
}

public class ItemInfo
{
    public int Id;
    public string ItemName;
    public string Desciption;
    public string Icon;
    public int Score;
}
