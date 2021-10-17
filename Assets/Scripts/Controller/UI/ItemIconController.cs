using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIconController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("基本参数")]
    public GameObject ItemDescriptionPrefab;    //预制体
    public GameObject ItemDescription;          //组件
    public string id;
    public bool isOn = false;
    public float ShowTime = 0;
    public float Timer = 0;

    public void InIt(string id)
    {
        this.id = id;

        //加载图片
        string path = Config.ItemIconPath + ItemInfoManager.Instance.GetIcon(id);
        //Debug.Log(path);
        Sprite sprite = Resources.Load<Sprite>(path);

        //初始化
        this.name = "item_" + id;
        if (sprite)
        {
            this.GetComponent<Image>().sprite = sprite;
        }
    }

    private void Update()
    {
        Timer += Time.deltaTime;

        if (isOn && Timer > ShowTime)
        {
            //第一次
            if (ItemDescription == null)
            {
                //实例化出来
                ItemDescription = Instantiate<GameObject>(ItemDescriptionPrefab, Input.mousePosition, Quaternion.identity, transform.parent.parent.parent.parent);
                
                Config.Language language = (Config.Language)PlayerPrefs.GetInt("Language", 0);       //获得语言

                string des = "";

                try
                {
                    des = FE_EventInfo.TextManage.Instance.GetText(ItemInfoManager.Instance.GetDescription(id), language);
                }
                catch
                {

                }

                string itemName = "";
                try
                {
                    itemName = FE_EventInfo.TextManage.Instance.GetText(ItemInfoManager.Instance.GetName(id), language);
                }
                catch
                {

                }

                ItemDescription.transform.GetChild(0).GetComponent<Text>().text = itemName + "\n\n" + des;
            }
            //非第一次
            else
            {
                //从关闭状态打开
                if (!ItemDescription.activeSelf)
                {
                    ItemDescription.SetActive(true);
                }

                //每一帧跟随鼠标
                ItemDescription.transform.position = Input.mousePosition;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOn = true;
        Timer = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOn = false;
        if (ItemDescription)
        {
            ItemDescription.SetActive(false);
        }
    }

}
