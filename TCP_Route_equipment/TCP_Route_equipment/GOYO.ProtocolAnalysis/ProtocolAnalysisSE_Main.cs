using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GOYO_Architecture;
using StriveEngine.Tcp.Server;
using SuperSocket.WebSocket;
using TCPAPI;
using ToolAPI;

namespace GOYO_ProtocolAnalysis
{
    public class ProtocolAnalysisSE_Main
    {

        static List<Client> client_List = new List<Client>();
        static List<WsClient> wsclient_List = new List<WsClient>();
        static object lock_object = new object();
        static object lock_objectws = new object();


        static ProtocolAnalysisSE_Main()
        {
            //回收站 定时清除长时间没活动client
            Thread recycle_binT = new Thread(Recycle_bin) { IsBackground = true };
            recycle_binT.Start();
            ToolAPI.XMLOperation.WriteLogXmlNoTail("初始化", "初始化完成");
        }

        #region 回收站
        static void Recycle_bin()
        {
            while (true)
            {
                Thread.Sleep(30000);
                try
                {
                    int result = 0;
                    int result1 = 0;
                    lock (lock_object)
                    {
                        result = client_List.RemoveAll(u => (DateTime.Now - u.Last_time).TotalSeconds > 180);
                    }
                    lock (lock_objectws)
                    {
                        result1 = wsclient_List.RemoveAll(u => (DateTime.Now - u.Last_time).TotalSeconds > 180);
                    }
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\Recycle", "回收结果", result.ToString() + ";" + result1.ToString());
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\Recycle", "回收异常", ex.Message);
                }

            }
        }
        #endregion

        #region TCPSocket
        public static void ProtocolPackageResolver(byte[] b, int c, TcpSocketClient client)
        {
            try
            {
                string data_str = Encoding.UTF8.GetString(b, 0, c);//这里使用utf-8来表示的
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\value", "", data_str);
                string[] data_strAry = data_str.Split('#');

                if (data_strAry.Length == 3)//数据展示端订阅
                {
                    Topic_analyze(client, data_strAry[1]);
                }
                else if (data_strAry.Length == 4)//数据推送端
                {
                    Topic_analyze(data_strAry[1], data_strAry[2], client);
                }
                else if (data_strAry.Length == 5)//数据推送端
                {
                    if (data_strAry[2] == "remove")
                    Topic_remove_analyze(client, data_strAry[1]);
                }
                else//异常的 想办法直接去断开
                {
                    try
                    {
                        client.DisSocket();
                        lock (lock_object)
                        {
                            client_List.RemoveAll(u => (DateTime.Now - u.Last_time).TotalSeconds > 180);
                        }
                    }
                    catch(Exception ex)
                    {
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("ProtocolPackageResolver 数据异常处理异常", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("解析异常", ex.Message);
            }
        }
        #region 解析
        /// <summary>
        /// 设备订阅
        /// </summary>
        /// <param name="client_temp">socket</param>
        /// <param name="value_str">主题</param>
        static void Topic_analyze(TcpSocketClient client_temp, string value_str)
        {
            try
            {
                client_temp.SendMessage("#" + value_str + "#true#");
                Client client = client_List.Where(u => u.Socket_client == client_temp).FirstOrDefault();
                if (client == null)
                {
                    Client clientt = new Client(client_temp);
                    clientt.Topic += value_str + "#";
                    lock (lock_object)
                    {
                        client_List.Add(clientt);
                    }
                    //移除不需要执行操作，因为压根就不存在
                }
                else
                {
                    client.Last_time = DateTime.Now;
                    if (!client.Topic.Contains("#" + value_str + "#"))
                    {
                        client.Topic += value_str + "#";
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("主题订阅异常", ex.Message);
                client_temp.SendMessage("#" + value_str + "#true#");
            }
        }

        /// <summary>
        /// 移除某个订阅
        /// </summary>
        /// <param name="client_temp"></param>
        /// <param name="value_str"></param>
        static void Topic_remove_analyze(TcpSocketClient client_temp, string value_str)
        {
            try
            {
                client_temp.SendMessage("#" + value_str + "#true#remove#");
                Client client = client_List.Where(u => u.Socket_client == client_temp).FirstOrDefault();
                if (client == null)
                {
                    Client clientt = new Client(client_temp);
                    clientt.Topic += "";
                    lock (lock_object)
                    {
                        client_List.Add(clientt);
                    }
                    //移除不需要执行操作，因为压根就不存在
                }
                else
                {
                    client.Last_time = DateTime.Now;
                    if (client.Topic.Contains("#" + value_str + "#"))
                    {
                        client.Topic = client.Topic.Replace(value_str + "#", "");
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("移除订阅异常", ex.Message);
                client_temp.SendMessage("#" + value_str + "#false#remove#");
            }
        }
        /// <summary>
        /// 数据发送
        /// </summary>
        /// <param name="client_temp"></param>
        /// <param name="value_str"></param>
        static void Topic_analyze(string topic_temp, string value_str, TcpSocketClient client_temp)
        {
            try
            {
                int ccount = 0;
                List<Client> clientList = client_List.Where(u => u.Topic.Contains("#" + topic_temp + "#")).ToList();
                if (clientList != null && clientList.Count > 0)
                {
                    ccount = clientList.Count;
                    foreach (Client cl in clientList)
                    {
                        try
                        {
                            bool isc = cl.Socket_client.IsConnect;
                            if (isc)
                            {
                                cl.Socket_client.SendMessage(value_str);
                            }
                            else
                            {
                                lock (lock_object)
                                {
                                    client_List.Remove(cl);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\send", "发送异常", ex.Message);
                        }
                    }
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\send", "发送结果", clientList.Count.ToString());
                }

                int wscount = 0;
                List<WsClient> wsclientList = wsclient_List.Where(u => u.Topic.Contains("#" + topic_temp + "#")).ToList();
                if (wsclientList != null && wsclientList.Count > 0)
                {
                    wscount = wsclientList.Count;
                    foreach (WsClient cl in wsclientList)
                    {
                        try
                        {
                            bool isc = cl.Socket_client.InClosing;
                            if (!isc)
                            {
                                SendMessage(cl.Socket_client, value_str);
                            }
                            else
                            {
                                lock (lock_objectws)
                                {
                                    wsclient_List.Remove(cl);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\send", "ws发送异常", ex.Message);
                        }
                    }
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\send", "ws发送结果", wsclientList.Count.ToString());
                }
                int sum = wscount + ccount;
                client_temp.SendMessage("#true#" + sum.ToString() + "#");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("数据发送异常", ex.Message);
                client_temp.SendMessage("#false#0#");
            }
        }
        #endregion


        #endregion

        #region WEBSocket
        public static void ProtocolPackageResolver_Web(string data_str, WebSocketSession wsclient)
        {
            try
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\value", "", data_str);
                string[] data_strAry = data_str.Split('#');

                if (data_strAry.Length == 3)//数据展示端订阅
                {
                    Topic_analyze(wsclient, data_strAry[1]);
                }
                else if (data_strAry.Length == 4)//数据推送端
                {
                    Topic_analyze(data_strAry[1], data_strAry[2], wsclient);
                }
                else if (data_strAry.Length == 5)//数据推送端
                {
                    if (data_strAry[2] == "remove")
                        Topic_remove_analyze(wsclient, data_strAry[1]);
                }
                else//异常的 想办法直接去断开
                {
                    try
                    {
                        wsclient.Close();
                        lock (lock_object)
                        {
                            client_List.RemoveAll(u => (DateTime.Now - u.Last_time).TotalSeconds > 180);
                        }
                    }
                    catch (Exception ex)
                    {
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("ProtocolPackageResolver_Web 数据异常处理异常", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("解析异常", ex.Message);
            }
        }
        static void SendMessage(WebSocketSession client, string msgt)
        {
            client.Send(msgt);
        }
        #region 解析
        /// <summary>
        /// 主题订阅
        /// </summary>
        /// <param name="client_temp">socket</param>
        /// <param name="value_str">主题</param>
        static void Topic_analyze(WebSocketSession client_temp, string value_str)
        {
            try
            {
                SendMessage(client_temp,"#" + value_str + "#true#");
                WsClient client = wsclient_List.Where(u => u.Socket_client == client_temp).FirstOrDefault();
                if (client == null)
                {
                    WsClient clientt = new WsClient(client_temp);
                    clientt.Topic += value_str + "#";
                    lock (lock_objectws)
                    {
                        wsclient_List.Add(clientt);
                    }
                    //移除不需要执行操作，因为压根就不存在
                }
                else
                {
                    client.Last_time = DateTime.Now;
                    if (!client.Topic.Contains("#" + value_str + "#"))
                    {
                        client.Topic += value_str + "#";
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("主题订阅异常", ex.Message);
                SendMessage(client_temp, "#topic#false#");
            }
        }


        /// <summary>
        /// 移除某个订阅
        /// </summary>
        /// <param name="client_temp"></param>
        /// <param name="value_str"></param>
        static void Topic_remove_analyze(WebSocketSession client_temp, string value_str)
        {
            try
            {
                SendMessage(client_temp,"#" + value_str + "#true#remove#");
                WsClient client = wsclient_List.Where(u => u.Socket_client == client_temp).FirstOrDefault();
                if (client == null)
                {
                    WsClient clientt = new WsClient(client_temp);
                    clientt.Topic += "";
                    lock (lock_object)
                    {
                        wsclient_List.Add(clientt);
                    }
                    //移除不需要执行操作，因为压根就不存在
                }
                else
                {
                    client.Last_time = DateTime.Now;
                    if (client.Topic.Contains("#" + value_str + "#"))
                    {
                        client.Topic = client.Topic.Replace(value_str + "#", "");
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("移除订阅异常", ex.Message);
                SendMessage(client_temp, "#" + value_str + "#false#remove#");
            }
        }


        /// <summary>
        /// 数据发送
        /// </summary>
        /// <param name="client_temp"></param>
        /// <param name="value_str"></param>
        static void Topic_analyze(string topic_temp, string value_str, WebSocketSession client_temp)
        {
            try
            {
                int wscount = 0;
                List<WsClient> wsclientList = wsclient_List.Where(u => u.Topic.Contains("#" + topic_temp + "#")).ToList();
                if (wsclientList != null && wsclientList.Count > 0)
                {
                    wscount = wsclientList.Count;
                    foreach (WsClient cl in wsclientList)
                    {
                        try
                        {
                            bool isc = cl.Socket_client.InClosing;
                            if (!isc)
                            {
                                SendMessage(cl.Socket_client, value_str);
                            }
                            else
                            {
                                lock (lock_objectws)
                                {
                                    wsclient_List.Remove(cl);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\send", "ws发送异常", ex.Message);
                        }
                    }
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\send", "ws发送结果", wsclientList.Count.ToString());
                }

                int ccount = 0;
                List<Client> clientList = client_List.Where(u => u.Topic.Contains("#" + topic_temp + "#")).ToList();
                if (clientList != null && clientList.Count > 0)
                {
                    ccount = clientList.Count;
                    foreach (Client cl in clientList)
                    {
                        try
                        {
                            bool isc = cl.Socket_client.IsConnect;
                            if (isc)
                            {
                                cl.Socket_client.SendMessage(value_str);
                            }
                            else
                            {
                                lock (lock_object)
                                {
                                    client_List.Remove(cl);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\send", "发送异常", ex.Message);
                        }
                    }
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\send", "发送结果", clientList.Count.ToString());
                }

                int sum = wscount + ccount;
                SendMessage(client_temp, "#true#" + sum.ToString() + "#");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("数据发送异常", ex.Message);
                SendMessage(client_temp, "#false#0#");
            }
        }
        #endregion

        #endregion
    }
}
