using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOYO_Architecture;
using TCPAPI;
using ToolAPI;
/*---------------------------------------------
    Copyright (c) 2017 共友科技
    版权所有：共友科技
    创建人名：赵通
    创建描述：协议解析主入口
    创建时间：2017.10.11
    文件功能描述：协议解析主入口，根据版本号进行分流
    修改人名：
    修改描述：
    修改标识：
    修改时间：
    ---------------------------------------------*/
namespace GOYO_ProtocolAnalysis
{
    public class ProtocolAnalysisSE_Main
    {
        //用于塔吊和升降机的
        public delegate string OnResolveRecvMessagedelegate(byte[] b, int c, TcpSocketClient client);
        public delegate string OnResoleRecvMessageUdpdelegate(byte[] b, int c, UdpState udp); //用于udp接收
        public static void ProtocolPackageResolver(byte[] b, int c, TcpSocketClient client)
        {
            switch (MainStatic.DeviceType)
            {
                //塔吊
                case 0: ProtocolPackageResolver_TowerCrane(b, c, client); break;
                
                default: break;
               
            }
        }
        

        #region 塔吊
        public static void ProtocolPackageResolver_TowerCrane(byte[] b, int c, TcpSocketClient client)
        {
            switch (b[0])
            {
                case 0xA5:
                    GoYOTower0xA5(b, c, client);
                    break;
                case 0x7E:
                    GoYOTower0x7E(b, c, client);
                    break;
            }
        }
        #region 0x7E开头的处理
        static OnResolveRecvMessagedelegate OnResolveRecvMessagede_7E0E = GprsResolveDataV0E.OnResolveRecvMessage;
        static OnResolveRecvMessagedelegate OnResolveRecvMessagede_7E01 = GprsResolveDataV01.OnResolveRecvMessage;
        private static void GoYOTower0x7E(byte[] b, int c, TcpSocketClient client)
        {
            TcpClientBindingExternalClass TcpExtendTemp = client.External.External as TcpClientBindingExternalClass;
            switch (b[2])
            {
                case 0x0E:
                    TcpExtendTemp.TVersion = "7E7E0E";
                    GoYOUnpack(b, c, client, "7E7E0E", "7D7D", OnResolveRecvMessagede_7E0E);
                    break;
                case 0x01:
                     TcpExtendTemp.TVersion = "7E7E0E";
                     GoYOUnpack(b, c, client, "7E7E01", "7D7D", OnResolveRecvMessagede_7E01);
                    break;
                default: break;
            }
        }
        #endregion
        #region 0xA5开头的处理
        static OnResolveRecvMessagedelegate OnResolveRecvMessagedeV021303 = GprsResolveDataV021303.OnResolveRecvMessage;
        private static void GoYOTower0xA5(byte[] b, int c, TcpSocketClient client)
        {
            TcpClientBindingExternalClass TcpExtendTemp = client.External.External as TcpClientBindingExternalClass;
            if (b[2] == 0x02 && b[3] == 0x13 && b[4] == 0x03)
            {
                TcpExtendTemp.TVersion = "A55A021303";
                GoYOUnpack(b, c, client, "A55A021303", "EEFF", OnResolveRecvMessagedeV021303);
            }
        }
        #endregion
        #endregion

     
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
        private static void GoYOUnpack(byte[] b, int c, TcpSocketClient client, string startStr, string endStr, OnResolveRecvMessagedelegate OnResolveRecvMessagede)
        {
            //得到帧组集合
            string dataHexString = ConvertData.ToHexString(b, 0, c);
            string[] stringSeparators = new string[] { startStr };
            //判断起始符+版本号进行分割包
            string[] DataHexAry = dataHexString.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < DataHexAry.Length; i++)
            {
                //判断结尾来确定帧是否完整
                if (DataHexAry[i].Length > endStr.Length)
                {
                    string ending = DataHexAry[i].Substring(DataHexAry[i].Length - endStr.Length);
                    if (ending.Equals(endStr))//一个完整的帧
                    {
                        //转换为字节数组
                        string frames = startStr + DataHexAry[i];
                        byte[] framesByte = ConvertData.HexToByte(frames);
                        //FileHelp.FileAppend(string.Format("【{0}】设备连接传入数据：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ConvertData.ToHexString(framesByte, 0, framesByte.Length)));
                        //进入对应的解析类
                        OnResolveRecvMessagede(framesByte, framesByte.Length, client);
                    }
                }
            }
        }
        #endregion
    }
}
