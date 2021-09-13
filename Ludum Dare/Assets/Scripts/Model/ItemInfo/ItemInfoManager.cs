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

    public ItemInfo Get(int index)
    {
        return ItemInfoList[index];
    }

    public ItemInfo Get(string Id)
    {
        for (int i = 0; i < ItemInfoList.Count; i++)
        {
            if (ItemInfoList[i].Id == Id)
            {
                return ItemInfoList[i];
            }
        }

        return null;
    }

    public string GetIcon(string Id)
    {
        return Get(Id).Icon;
    }
    public string GetDescription(string Id)
    {
        return Get(Id).Desciption;
    }
    public string GetName(string Id)
    {
        return Get(Id).ItemName;
    }
}

public class ItemInfo
{
    public string Id { get; set; }
    public string ItemName { get; set; }
    public string Desciption { get; set; }
    public string Icon { get; set; }
    public int Score { get; set; }
}
