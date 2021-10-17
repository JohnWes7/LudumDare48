using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace FE_EventInfo
{
    public class TextManage : Single<TextManage>, IAddInfo
    {
        private EventTextData eventTextData;

        public TextManage()
        {
            Debug.Log("TextManage 初始化");
            AddInfo(Config.FE_common_directory_PATH);
        }

        public string GetText(string key, Config.Language language)
        {
            string text = null;
            bool success = false;

            //用传进来的key 去对应的字典里索引值 中文查中文字典 英文查英文字典
            switch (language)
            {
                case Config.Language.l_simp_chinese:
                    if (this.eventTextData.l_simp_chinese != null)
                    {
                        success = this.eventTextData.l_simp_chinese.TryGetValue(key, out text);
                    }
                    break;
                case Config.Language.l_english:
                    if (this.eventTextData.l_english != null)
                    {
                        success = this.eventTextData.l_english.TryGetValue(key, out text);
                    }
                    break;
                default:
                    Debug.Log("此language 字典未找到");
                    break;
            }


            //如果索引成功就用索引出来的值 如果索引失败直接显示key
            if (success)
            {
                return text;
            }

            return key;
        }

        public static void DEBUG()
        {
            Debug.Log(Instance.eventTextData.ToString());
        }

        public void AddInfo(string mod_common_path)
        {
            DirectoryInfo directoryInfo;
            if (!Tool.OpenDirectory(mod_common_path, "/localisation", out directoryInfo))
            {
                return;
            }

            List<FileInfo> fileInfos = Tool.GetFiles(directoryInfo, "*.json");
            Debug.Log(directoryInfo.FullName + "内含有" + fileInfos.Count + "个file");

            for (int i = 0; i < fileInfos.Count; i++)
            {
                string json;
                if (Tool.ReadText(fileInfos[i], out json, Config.Encoding))
                {
                    EventTextData eventTextData = Tool.JSONString2Object<EventTextData>(json);
                    if (eventTextData != null)
                    {
                        AddInfo(eventTextData);
                    }
                }
            }
        }
        public void AddInfo(EventTextData eventTextData)
        {
            if (this.eventTextData != null)
            {
                this.eventTextData.AddInfo(eventTextData);
                return;
            }

            this.eventTextData = eventTextData;
        }
        public void AddInfo<T>(T info)
        {
            
        }
    }

    public class EventTextData
    {
        public Dictionary<string, string> l_simp_chinese;
        public Dictionary<string, string> l_english;

        public void AddInfo(EventTextData eventTextData)
        {
            this.l_simp_chinese = Tool.MergeDictionary(this.l_simp_chinese, eventTextData.l_simp_chinese);
            this.l_english = Tool.MergeDictionary(this.l_english, eventTextData.l_english);
        }
        public override string ToString()
        {
            StringBuilder log = new StringBuilder();

            foreach (var item in this.l_simp_chinese)
            {
                log.Append(item.Key + " : " + item.Value + "\n");
            }
            log.Append("============================================\n");
            foreach (var item in this.l_english)
            {
                log.Append(item.Key + " : " + item.Value + "\n");
            }

            return log.ToString();
        }
    }
}


