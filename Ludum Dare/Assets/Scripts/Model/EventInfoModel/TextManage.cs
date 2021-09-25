using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace FE_EventInfo
{
    public class TextManage : Single<TextManage>
    {
        private EventText eventText;

        public TextManage()
        {
            string json = EventInfoManager.ReadTextFromResourcesJSON(Config.EventInfoTextPath); //获取json
            eventText = JsonConvert.DeserializeObject<EventText>(json);   //将json转成对应的类

            
        }

        public static string GetText(string key, Config.Language language)
        {
            string text = null;
            bool success = false;

            //用传进来的key 去对应的字典里索引值 中文查中文字典 英文查英文字典
            switch (language)
            {
                case Config.Language.l_simp_chinese:
                    success = Instance.eventText.l_simp_chinese.TryGetValue(key, out text);
                    break;
                case Config.Language.l_english:
                    success = Instance.eventText.l_english.TryGetValue(key, out text);
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
            string log = "";

            EventText eventText = Instance.eventText;
            foreach (var item in eventText.l_simp_chinese)
            {
                log = log + item.Key + " : " + item.Value + "\n";
            }
            log += "============================================\n";
            foreach (var item in eventText.l_english)
            {
                log = log + item.Key + " : " + item.Value + "\n";
            }

            Debug.Log(log);
        }

    }

    public class EventText
    {
        public Dictionary<string, string> l_simp_chinese;
        public Dictionary<string, string> l_english;
    }
}


