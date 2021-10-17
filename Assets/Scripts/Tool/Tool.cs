using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Linq;

public static class Tool
{
    /// <summary>
    /// 转换json到类
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="json">json字符串</param>
    /// <returns>返回类 （可以是null null的话回有debug）</returns>
    public static T JSONString2Object<T>(string json)
    {
        //新的转json工具
        T t = JsonConvert.DeserializeObject<T>(json);

        //如果转换失败 生成报告
        if (t == null)
        {
            Debug.LogWarning("json转换类失败\n失败json ：" + json + "\n" + t.GetType().ToString());
        }

        return t;
    }
    /// <summary>
    /// 读取工具类
    /// </summary>
    /// <param name="stream">流</param>
    /// <param name="str">读取出的字符串</param>
    /// <param name="encoding">编码格式 UTF8</param>
    /// <returns>是否读取成功</returns>
    public static bool ReadText(Stream stream, out string str, Encoding encoding)
    {
        str = null;
        try
        {
            using (StreamReader streamReader = new StreamReader(stream, encoding))
            {
                str = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }

        //所有的文件IO读取都会做个打印
        if (str != null)
        {
            Debug.Log(str);
        }

        return true;
    }
    /// <summary>
    /// 读取工具类
    /// </summary>
    /// <param name="fileInfo">文件fileinfo</param>
    /// <param name="str">读取出的字符串</param>
    /// <param name="encoding">编码格式 UTF8</param>
    /// <returns>是否读取成功</returns>
    public static bool ReadText(FileInfo fileInfo, out string str, Encoding encoding)
    {
        FileStream fileStream;
        try
        {
            fileStream = fileInfo.Open(FileMode.Open);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            str = null;
            return false;
        }
        
        
        bool jugde = ReadText(fileStream, out str, encoding);

        fileStream.Close();

        return jugde;
    }
    /// <summary>
    /// 从指定Resource路径读取 TextAsset 文件
    /// </summary>
    /// <param name="PATH">路径</param>
    /// <returns>TextAsset 字符串</returns>
    public static string ReadTextFromResourcesJSON(string PATH)
    {
        //读取json
        string json = Resources.Load<TextAsset>(PATH).text;
        return json;
    }
    /// <summary>
    /// 递归得到文件夹下面所有的文件
    /// </summary>
    /// <param name="directoryInfo">初始文件夹</param>
    /// <param name="searchPattern">搜索searchPattern</param>
    /// <returns>所有的结果</returns>
    public static List<FileInfo> GetAllFiles(DirectoryInfo directoryInfo, string searchPattern) 
    {
        
        List<FileInfo> list = directoryInfo.GetFiles(searchPattern).ToList<FileInfo>();
        //Debug.Log("当前访问：" + directoryInfo.FullName + " 找到" + list.Count + "个" + searchPattern);

        DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
        for (int i = 0; i < directoryInfos.Length; i++)
        {
            list.AddRange(GetAllFiles(directoryInfos[i], searchPattern));
        }

        return list;
    }
    /// <summary>
    /// 不递归找到所有文件
    /// </summary>
    /// <param name="directoryInfo">文件夹</param>
    /// <param name="searchPattern">索引</param>
    /// <returns>所有的结果</returns>
    public static List<FileInfo> GetFiles(DirectoryInfo directoryInfo, string searchPattern)
    {
        List<FileInfo> list = directoryInfo.GetFiles(searchPattern).ToList<FileInfo>();
        Debug.Log("当前访问：" + directoryInfo.FullName + " 找到" + list.Count + "个" + searchPattern);

        return list;
    }
    /// <summary>
    /// 打开指定文件夹 失败directory为null 返回false
    /// </summary>
    /// <param name="mod_directory_path">mod common文件夹</param>
    /// <param name="part">所打开的部分</param>
    /// <param name="directoryInfo">打开后的info 失败为null</param>
    /// <returns>是否成功打开</returns>
    public static bool OpenDirectory(string mod_directory_path,string part, out DirectoryInfo directoryInfo)
    {
        directoryInfo = null;

        //获得文件夹
        try
        {
            //事件路径
            string path = mod_directory_path + part;

            if (Directory.Exists(path))
            {
                directoryInfo = new DirectoryInfo(path);
            }
            else
            {
                Debug.Log("文件夹不存在： " + path);
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }

        return true;
    }
    /// <summary>
    /// 合并字典
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static Dictionary<string, string> MergeDictionary(Dictionary<string, string> first, Dictionary<string, string> second)
    {
        if (first == null) first = new Dictionary<string, string>();
        if (second == null) return first;

        foreach (var item in second)
        {
            if (!first.ContainsKey(item.Key))
                first.Add(item.Key, item.Value);
        }

        return first;

        
    }

    
}


