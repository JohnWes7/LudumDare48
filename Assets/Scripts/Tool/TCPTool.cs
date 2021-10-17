using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TCPTool
{
    public enum TCP_MESSAGE_CODE
    {
        /// <summary>
        /// 前端心跳
        /// </summary>
        C2S_HeartBeat = 666,
        /// <summary>
        /// 服务器心跳
        /// </summary>
        S2C_HeartBeat = 667,
        
        /// <summary>
        /// 前端消息
        /// </summary>
        C2S_ChatToWhole = 11000,
        /// <summary>
        /// 服务器消息
        /// </summary>
        S2C_ChatToWhole = 11001
    }

    public static byte[] Pack(TcpMessage tcpMessage)
    {
        //包头8字节 消息号4字节 消息体长度 共同构成包体大小
        byte[] length = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(tcpMessage.Data == null ? 12 : tcpMessage.Data.Length + 12));
        //干扰字节
        byte[] noise = BitConverter.GetBytes(0);
        //消息号
        byte[] code = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(tcpMessage.Code));

        List<byte> message = new List<byte>();
        message.AddRange(length);
        message.AddRange(noise);
        message.AddRange(code);
        if (tcpMessage.Data != null)
        {
            message.AddRange(tcpMessage.Data);
        }

        return message.ToArray();
    }

    public static TcpMessage Unpack(byte[] message)
    {
        TcpMessage data;

        //获取消息号
        byte[] code = new byte[4];
        Array.Copy(message, 8, code, 0, 4);
        data.Code = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(code, 0));

        //计算整个包体大小
        byte[] l = new byte[4];
        Array.Copy(message, 0, l, 0, 4);
        int length = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(l, 0));
        //提取消息体
        if (length > 12)
        {
            //去掉包头
            data.Data = new byte[length - 12];
            Array.Copy(message, 12, data.Data, 0, length - 12);
        }
        else
        {
            data.Data = null;
        }

        return data;
    }
}

public struct TcpMessage
{
    /// <summary>
    /// 消息号 类似于URL地址 告诉对方执行那些代码逻辑
    /// </summary>
    public int Code;

    /// <summary>
    /// 数据消息
    /// </summary>
    public byte[] Data;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code"></param>
    /// <param name="data"></param>
    public TcpMessage(int code, byte[] data)
    {
        this.Code = code;
        this.Data = data;
    }
}



//System.Net.IPAddress.HostToNetworkOrder() 主机字节序转网络字节序 不管主机是大端字节序还是小端
//