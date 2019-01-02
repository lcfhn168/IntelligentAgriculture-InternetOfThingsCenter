using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GOYO_Architecture;
using SuperSocket.WebSocket;
using TCPAPI;

namespace GOYO_ProtocolAnalysis
{
    public class WsClient
    {
        /// <summary>
        /// 系统自动生成的id号
        /// </summary>
        public string UUID { set; get; }
        /// <summary>
        /// 用户自定义客户端名称
        /// </summary>
        public string Flag { set; get; }
        /// <summary>
        /// tcp客戶端
        /// </summary>
        public WebSocketSession Socket_client { get; set; }
        /// <summary>
        /// 订阅的主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 最后一次被更新的时间
        /// </summary>
        public DateTime Last_time { get; set; }

        public WsClient(WebSocketSession client, DateTime dt)
        {
            UUID = System.Guid.NewGuid().ToString("N");
            Flag = "";
            Socket_client = client;
            Last_time = dt;
            Topic = "#";
        }
        public WsClient(WebSocketSession client)
        {
            UUID = System.Guid.NewGuid().ToString("N");
            Flag = "";
            Socket_client = client;
            Last_time = DateTime.Now;
            Topic = "#";
        }

    }
}
