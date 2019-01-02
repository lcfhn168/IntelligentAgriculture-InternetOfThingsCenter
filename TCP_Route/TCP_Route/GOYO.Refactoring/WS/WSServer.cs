using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using StriveEngine;
using StriveEngine.Core;
using StriveEngine.Tcp.Server;
using SuperSocket.WebSocket;

namespace GOYO_Architecture
{
    public class WSServer
    {
        private WebSocketServer server;
        public event Action<string, WebSocketSession> OnSocketResolveRecvEvent;
        Thread ClientAddT, ReleaseResourcesT, ClientRemoveT;
        Dictionary<WebSocketSession, DateTime> wsList = new Dictionary<WebSocketSession, DateTime>();
        object wsListLock = new object();

        public void WSServerStart(Subject sub)
        {
            OnSocketResolveRecvEvent = sub.DataAnalysisWS_trigger;
            server = new WebSocketServer();
            server.NewSessionConnected += server_NewSessionConnected;
            server.NewMessageReceived += server_NewMessageReceived;
            server.SessionClosed += server_SessionClosed;
            try
            {
                string[] ipaddr = MainStatic.WsPort.Split(':');
                server.Setup(ipaddr[0], int.Parse(ipaddr[1]));//设置端口
                server.Start();//开启监听
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("WSS server启动异常", ex.Message);
            }
            ReleaseResourcesT = new Thread(ReleaseResources) { IsBackground = true };
            ReleaseResourcesT.Start();
        }
        public void WSServerStop()
        {
            try
            {
                if (ClientAddT != null)
                {
                    ClientAddT.Abort();
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("WS WSServerStop ClientAddT异常", ex.Message);
            }
            try
            {
                if (ReleaseResourcesT != null)
                {
                    ReleaseResourcesT.Abort();
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("WS WSServerStop ReleaseResourcesT异常", ex.Message);
            }
            try
            {
                if (server != null)
                {
                    server.Stop();
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("WS WSServerStop listener异常", ex.Message);
            }
        }

        /// <summary>
        /// 接收新的客户端
        /// </summary>
        void server_NewSessionConnected(WebSocketSession session)
        {
            try
            {
                if (!wsList.ContainsKey(session))
                {
                    lock (wsListLock)
                    {
                        wsList.Add(session, DateTime.Now);
                    }
                }
                else
                    wsList[session] = DateTime.Now;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("WS StartRecvMessage异常", ex.Message);
            }
        }
        /// <summary>
        /// 客户端被关闭的时候执行的方法
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        void server_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            try
            {
                //执行这个的时候，就证明已经被close了
                if (wsList.ContainsKey(session))
                {
                    lock (wsListLock)
                    {
                        wsList.Remove(session);
                    }
                }
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("WSS server关闭连接异常", ex.Message);
            }

        }
        /// <summary>
        /// 接收数据的时候执行的方法
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        void server_NewMessageReceived(WebSocketSession session, string value)
        {
            try
            {
                if (wsList.ContainsKey(session))
                {
                    wsList[session] = DateTime.Now;
                }
                if (OnSocketResolveRecvEvent != null)
                    OnSocketResolveRecvEvent(value, session);
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("WSS server数据接收连接异常", ex.Message);
            }
        }

        /// <summary>
        /// 清除不符合要求的客户端
        /// </summary>
        void ReleaseResources()
        {
            while (true)
            {
                int i = 0;
                try
                {

                    foreach (var item in wsList.ToList())
                    {
                        if ((DateTime.Now - item.Value).TotalSeconds > 190)
                        {
                            lock (wsListLock)
                            {
                                item.Key.Close();//关闭这个链接
                                wsList.Remove(item.Key);
                                i++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("WSS ReleaseResources异常", ex.Message);
                }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("移除超时个数", i.ToString());
                Thread.Sleep(10000);
            }
        }

    }
}
