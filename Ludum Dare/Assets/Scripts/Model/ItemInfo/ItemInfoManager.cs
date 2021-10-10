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

        FileInfo[] fileInfos = common_info.GetFiles("*.json");
        for (int i = 0; i < fileInfos.Length; i++)
        {
            FileStream file_stream = fileInfos[i].OpenRead();

            byte[] temp = new byte[file_stream.Length];
            file_stream.Read(temp, 0, (int)file_stream.Length);
            string json = System.Text.Encoding.UTF8.GetString(temp);
            //TODO:物品读取转json

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
