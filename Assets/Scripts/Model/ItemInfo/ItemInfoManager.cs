using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Text;

public class ItemInfoManager : Single<ItemInfoManager>, IAddInfo
{
    private ItemData item_data;

    public ItemData Item_Data { get => item_data; }

    public ItemInfoManager()
    {
        //只能add一次
        AddInfo(Config.FE_common_directory_PATH);

        #region debug
        //Debug.Log(json);

        #endregion
    }

    public void AddInfo(string mod_directory_path)
    {
        DirectoryInfo common_info = null;

        try
        {
            if (Directory.Exists(mod_directory_path + "/items"))
            {
                common_info = new DirectoryInfo(mod_directory_path + "/items");
            }
            else
            {
                Debug.Log("文件夹不存在： " + mod_directory_path + "/items");
                return;
            }
        }
        catch (ArgumentNullException)
        {
            Debug.LogError("传入的字符串为空");
            return;
        }
        catch (System.Security.SecurityException)
        {
            Debug.LogError("调用方没有所要求的权限。");
            return;
        }
        catch (ArgumentException)
        {
            Debug.LogError("path 包含无效字符，例如 \"、<、> 或 |。");
            return;
        }
        catch (PathTooLongException)
        {
            Debug.LogError("指定的路径和/或文件名超过了系统定义的最大长度。");
            return;
        }

        List<FileInfo> fileInfos = Tool.GetFiles(common_info, "*.json");
        Debug.Log(common_info.FullName + "内含有" + fileInfos.Count + "个file");

        for (int i = 0; i < fileInfos.Count; i++)
        {
            string json;
            if (Tool.ReadText(fileInfos[i], out json, Config.Encoding))
            {
                ItemData eventTextData = Tool.JSONString2Object<ItemData>(json);
                if (eventTextData != null)
                {
                    AddInfo(eventTextData);
                }
            }
        }
    }
    
    public void AddInfo<T>(T info)
    {
        if (info is ItemData)
        {
            if (item_data != null)
            {
                item_data.AddInfo(info as ItemData);
            }
            else
            {
                item_data = info as ItemData;
            }
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
    public static void DEBUG()
    {
        Debug.Log(Instance.item_data.ToString());
    }
}

public class ItemData
{
    [JsonProperty]
    private List<ItemInfo> item_info = null;
    public List<ItemInfo> Item_info { get => item_info; }
    public void AddInfo(ItemData itemData)
    {
        this.item_info.AddRange(itemData.Item_info);
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < item_info.Count; i++)
        {
            stringBuilder.Append(this.item_info[i].ToString() + "\n");
        }

        return stringBuilder.ToString();
    }
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

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(" id: " + id);
        stringBuilder.Append(" Item_Name: " + item_name);
        stringBuilder.Append(" Desciption: " + desciption);
        stringBuilder.Append(" Icon: " + icon);

        return stringBuilder.ToString();
    }
}
