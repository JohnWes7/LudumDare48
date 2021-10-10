using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCPDriver : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// 保证外部不能实例化
    /// </summary>
    private TCPDriver()
    {

    }

    /// <summary>
    /// 储存单例的静态成员变量
    /// </summary>
    private static TCPDriver instance = null;

    /// <summary>
    /// 获取单例的静态方法
    /// </summary>
    /// <returns></returns>
    public static TCPDriver GetTCPDriver()
    {
        if (instance == null)
        {
            //创建单例的载体
            GameObject driver = new GameObject("TCPDriver");
            //附加上载体
            instance = driver.AddComponent<TCPDriver>();
            //防止切换场景，导致TCP取驱动失去
            DontDestroyOnLoad(driver);
        }

        return instance;
    }

    #endregion

    #region Connection

    public enum TCP_CONNECT_STATUS
    {
        Default
    }

    public TCP_CONNECT_STATUS ConnectStatus;

    public string Host = "";

    public int Port = 0;

    #endregion
}
