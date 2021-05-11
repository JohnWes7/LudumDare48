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
    public int ID = 1;
    public bool isOn = false;
    public float ShowTime = 0;
    public float Timer = 0;

    public void InIt(int id)
    {
        this.ID = id;

        //读表
        ItemInfo itemInfo = ItemInfoManager.Instance.Get(id);

        //加载图片
        string path = Config.ItemIconPath + itemInfo.Icon;
        //Debug.Log(path);
        Sprite sprite = Resources.Load<Sprite>(path);

        //初始化
        this.name = "item_" + id;
        if (sprite)
        {
            this.GetComponent<Image>().sprite = sprite;
        }
    }

    public void InIt(ItemInfo info)
    {
        
    }

    private void Update()
    {
        Timer += Time.deltaTime;

        if (isOn && Timer > ShowTime)
        {
            //第一次
            if (ItemDescription == null)
            {
                ItemDescription = Instantiate<GameObject>(ItemDescriptionPrefab, Input.mousePosition, Quaternion.identity, transform.parent.parent.parent.parent);
                ItemInfo info = ItemInfoManager.Instance.Get(this.ID);  //获得信息
                int language = PlayerPrefs.GetInt("Language", 0);       //获得语言

                string des = "";

                try
                {
                    des = info.Desciption.Split('#')[language];
                }
                catch
                {

                }

                string itemName = "";
                try
                {
                    itemName = info.ItemName.Split('#')[language];
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
