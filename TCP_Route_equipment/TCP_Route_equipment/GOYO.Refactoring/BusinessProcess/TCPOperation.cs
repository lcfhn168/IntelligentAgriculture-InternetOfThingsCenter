using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TCPAPI;
using ToolAPI;

namespace GOYO_Architecture
{
    public class TCPOperation : AbstractBLL
    {
        Subject Subject;
        public TCPOperation(Subject sub)
        {
            Subject = sub;
        }

        /// <summary>
        /// 接收数据事件处理方法
        /// </summary>
        public override void tcp_OnSocketResolveRecvEvent(byte[] b, int c, TcpSocketClient client)
        {
            try
            {
                //写入文件做监控
                string Sn = "";
                if (client.External != null)
                    Sn = client.External.External.ToString();
                if (c > 0)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\OriginalPackage", Sn, ConvertData.ToHexString(b, 0, c));
                    #region 根据协议进行解包
                    //需要外部调用
                    Subject.DataAnalysis_trigger(b, c, client);
                    #endregion
                }
                else
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\OriginalPackage", Sn, "无接收设备数据信息");
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("tcp_OnSocketResolveRecvEvent异常", ex.Message);
            }
        }
        /// <summary>
        /// 绑定SOCKET事件处理方法 在解析里用吧，不需要多次解析了
        /// </summary>
        /// <param name="b"></param>
        /// <param name="client"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object tcp_InitSocketBindingEvent(byte[] b, TcpSocketClient client, object obj)
        {
            try
            {
                ////需要外部调用
                //return Subject.SocketBinding_trigger(b);
                return "";
            }
            catch
            { return ""; }
        }
    }
}
