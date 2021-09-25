using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class ItemInfoManager : Single<ItemInfoManager>
{
    private ItemData item_data;

    public ItemData Item_Data { get => item_data; }

    public ItemInfoManager()
    {
        //TODO:新版的物品读取

        ////读取json
        //string json = Resources.Load<TextAsset>(Config.ItemInfoJsonPath).text;
        ////转换成list
        //item_data = Tool.JSONString2Object<ItemData>(json);

        AddInfo(Config.FE_common_directory_PATH);

        #region debug
        //Debug.Log(json);

        #endregion
    }

    public void AddInfo(string mod_path)
    {
        DirectoryInfo common_info = null;

        try
        {
            common_info = new DirectoryInfo(mod_path + "/items");
        }
        catch (ArgumentNullException)
        {
            Debug.LogError("传入的字符串为空");
        }
        catch (System.Security.SecurityException)
        {
            Debug.LogError("调用方没有所要求的权限。");
        }
        catch (ArgumentException)
        {
            Debug.LogError("path 包含无效字符，例如 \"、<、> 或 |。");
        }
        catch (PathTooLongException)
        {
            Debug.LogError("指定的路径和/或文件名超过了系统定义的最大长度。");
        }

        FileInfo[] fileInfos = common_info.GetFiles("*.json");
        for (int i = 0; i < fileInfos.Length; i++)
        {
            FileStream file_stream = fileInfos[i].OpenRead();

            byte[] temp = new byte[file_stream.Length];
            file_stream.Read(temp, 0, (int)file_stream.Length);
            string json = System.Text.Encoding.UTF8.GetString(temp);

            Debug.Log(json);
        }
    }

    public ItemInfo Get(int index)
    {
        return Item_Data.Item_info[index];
    }

    public ItemInfo Get(string Id)
    {
        for (int i = 0; i < Item_Data.Item_info.Count; i++)
        {
            if (Item_Data.Item_info[i].Id == Id)
            {
                return Item_Data.Item_info[i];
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
        return Get(Id).Item_Name;
    }
}

public class ItemData
{
    [JsonProperty]
    private List<ItemInfo> item_info = null;

    public List<ItemInfo> Item_info { get => item_info; }
}

public class ItemInfo
{
    [JsonProperty]
    private string id = null;
    [JsonProperty]
    private string item_name = null;
    [JsonProperty]
    private string desciption = null;
    [JsonProperty]
    private string icon = null;
    [JsonProperty]
    private int score = 0;

    public string Id { get => id; }
    public string Item_Name { get => item_name; }
    public string Desciption { get => desciption; }
    public string Icon { get => icon; }
    public int Score { get => score; }
}
