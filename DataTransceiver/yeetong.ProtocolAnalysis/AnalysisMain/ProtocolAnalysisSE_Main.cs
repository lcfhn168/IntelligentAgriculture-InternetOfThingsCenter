using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yeetong_Architecture;
using TCPAPI;
using ToolAPI;
using yeetong_UdpServer;

namespace yeetong_ProtocolAnalysis
{
    public class ProtocolAnalysisSE_Main
    {
        public static void ProtocolPackageResolver(byte[] b, int c, TcpSocketClient client)
        {
            switch (MainStatic.DeviceType)
            {
                //小黄犁
                case 0: DTU_Data_Analyze.AnalyzeProcess(b, c, client); break;
                default: break;
               
            }
        }
        #region 拆包算法
        /// <summary>
        /// 拆包算法
        /// </summary>
        /// <param name="b">tcp接收到的字节流</param>
        /// <param name="c">tcp接收到的字节流长度</param>
        /// <param name="client">TcpSocketClient对象</param>
        /// <param name="startStr">可以进行拆分表示的协议头</param>
        /// <param name="endStr">可以进行拆分表示的协议尾</param>
        /// <param name="OnResolveRecvMessagede">最后调用的解析类方法</param>
        //private static void yeetongUnpack(byte[] b, int c, TcpSocketClient client, string startStr, string endStr, OnResolveRecvMessagedelegate OnResolveRecvMessagede)
        //{
        //    //得到帧组集合
        //    string dataHexString = ConvertData.ToHexString(b, 0, c);
        //    string[] stringSeparators = new string[] { startStr };
        //    //判断起始符+版本号进行分割包
        //    string[] DataHexAry = dataHexString.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
        //    for (int i = 0; i < DataHexAry.Length; i++)
        //    {
        //        //判断结尾来确定帧是否完整
        //        if (DataHexAry[i].Length > endStr.Length)
        //        {
        //            string ending = DataHexAry[i].Substring(DataHexAry[i].Length - endStr.Length);
        //            if (ending.Equals(endStr))//一个完整的帧
        //            {
        //                //转换为字节数组
        //                string frames = startStr + DataHexAry[i];
        //                byte[] framesByte = ConvertData.HexToByte(frames);
        //                //FileHelp.FileAppend(string.Format("【{0}】设备连接传入数据：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ConvertData.ToHexString(framesByte, 0, framesByte.Length)));
        //                //进入对应的解析类
        //                OnResolveRecvMessagede(framesByte, framesByte.Length, client);
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
