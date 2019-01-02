using MQTTPushService.TowerCrane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ToolAPI;

namespace MQTTPushService
{
    public class TCp
    {
        public static void SendMessageToServer(ForwardModel e, string json)
        {
            try
            {
                string result = "#" + e.sn + "#" + json + "#";
                byte[] cache = Encoding.ASCII.GetBytes(result);
                byte[] revcache = new byte[512];
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(e.Server, int.Parse(e.Port));
                int t = client.Send(cache);
                client.Close();

                //ToolAPI.XMLOperation.WriteLogXmlNoTail(System.Windows.Forms.Application.StartupPath + @"\" + e.AgreementName, "TCP数据发送", "地址：" + e.Server + ":" + e.Port + "; 结果：" + t.ToString() + "; 字节流：" + ConvertData.ToHexString(cache, 0, cache.Length));
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(System.Windows.Forms.Application.StartupPath + @"\" + e.AgreementName, "TCP转发结果失败", json + ";" + ex.Message);
            }
        }
        /// <summary>
        /// 报警通知
        /// </summary>
        /// <param name="e"></param>
        /// <param name="json"></param>
        public static bool SendMessageToServerNotice(ForwardModel e, string json)
        {
            bool ret = false;
            try
            {
                string result = "#" + e.AgreementName + "#" + json + "#";
                byte[] cache = Encoding.ASCII.GetBytes(result);
                byte[] revcache = new byte[512];
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(e.Server, int.Parse(e.Port));
                int t = client.Send(cache);
                client.Close();
                ret = true;
               // ToolAPI.XMLOperation.WriteLogXmlNoTail(System.Windows.Forms.Application.StartupPath + @"\" + e.AgreementName, "TCP数据发送", "地址：" + e.Server + ":" + e.Port + "; 结果：" + t.ToString() + "; 字节流：" + ConvertData.ToHexString(cache, 0, cache.Length));
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(System.Windows.Forms.Application.StartupPath + @"\" + e.AgreementName, "TCP转发结果失败", json + ";" + ex.Message);
            }
            return ret;
        }
        
    }
}
public class ForwardModel
{
    /// <summary>
    /// tcp链接地址
    /// </summary>
    public string Server { get; set; }
    /// <summary>
    /// 端口号
    /// </summary>
    public string Port { get; set; }
    /// <summary>
    /// 说明备注
    /// </summary>
    public string AgreementName { get; set; }
    /// <summary>
    /// 推送类型
    /// </summary>
    public string putType { get; set; }
    /// <summary>
    /// 设备号
    /// </summary>
    public string sn { get; set; }
    /// <summary>
    /// 设备类型
    /// </summary>
    public int etype { get; set; }
}