using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using StriveEngine;
using SuperSocket.WebSocket;
using TCPAPI;

namespace GOYO_Architecture
{
    public class Subject
    {
        /// <summary>
        /// 数据解析事件
        /// </summary>
        public event Action<byte[], int, TcpSocketClient> DataAnalysis = (byte[] byteAry, int count, TcpSocketClient socketClient) => { };

        /// <summary>
        /// 数据解析事件触发
        /// </summary>
        /// <param name="byteAry"></param>
        public void DataAnalysis_trigger(byte[] byteAry, int count, TcpSocketClient socketClient)
        {
            DataAnalysis(byteAry, count, socketClient);
        }

        /// <summary>
        /// 数据解析事件
        /// </summary>
        public event Action<string, WebSocketSession> DataAnalysisWS = (string Message, WebSocketSession socketClient) => { };

        /// <summary>
        /// 数据解析事件触发
        /// </summary>
        /// <param name="byteAry"></param>
        public void DataAnalysisWS_trigger(string Message, WebSocketSession socketClient)
        {
            DataAnalysisWS(Message, socketClient);
        }
    }
}
