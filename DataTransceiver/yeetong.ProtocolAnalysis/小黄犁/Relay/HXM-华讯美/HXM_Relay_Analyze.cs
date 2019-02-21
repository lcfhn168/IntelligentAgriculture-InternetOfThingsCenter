using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCPAPI;
using ToolAPI;
using yeetong_Architecture;

namespace yeetong_ProtocolAnalysis
{
    public class HXM_Relay_Analyze
    {
        #region 解析入口
        //解析、存储数据
        public static void AnalyzeProcess(byte[] b, int c, TcpSocketClient client)
        {
            try
            {
                //if (c > 2)
                //{
                //    byte[] btemp = new byte[c - 2];
                //    Array.Copy(b, 0, btemp, 0, c - 2);
                //    byte[] value = Tool.ToModbus(btemp);
                //    if (value[0] != b[c - 2] || value[1] != b[c - 1])
                //    {
                //        ToolAPI.XMLOperation.WriteLogXmlNoTail("无效包", ConvertData.ToHexString(b, 0, c));
                //        return;
                //    }
                //}
                //先得到这个设备对应的dtu的设备编号
                TcpClientBindingExternalClass TcpExtendTemp = client.External.External as TcpClientBindingExternalClass;
                DBFrame df = new DBFrame();
                df.contenthex = ConvertData.ToHexString(b, 0, c);
                df.version = "1.0";//默认写成1.0

                byte addr485 = b[0];//485地址
                byte command = b[1];//命令

                //发送：01 01 00 00 00 08 3D CC
                //返回：01 01 01 00 51 88      继电器全关闭状态 1是开启 0是关闭
                //返回：01 01 01 03 11 89      继电器全部开启状态 1是开启 0是关闭
                if (command == 0x01)//查询状态的
                {
                    byte length = b[2];//数据长度
                    HXM_Relay_Status xM_Relay_Status = new HXM_Relay_Status();
                    if (TcpExtendTemp.EquipmentID == null || TcpExtendTemp.EquipmentID.Equals(""))
                        return;
                    xM_Relay_Status.DTUID = TcpExtendTemp.EquipmentID;
                    xM_Relay_Status.Addr485 = addr485.ToString();
                    xM_Relay_Status.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //状态
                    xM_Relay_Status.RelayStatus = Convert.ToString(b[3], 2).PadLeft(8, '0');

                    df.deviceid = xM_Relay_Status.DTUID;
                    df.datatype = "RelayStatus";
                    df.contentjson = JsonConvert.SerializeObject(xM_Relay_Status);

                    if (TcpExtendTemp.EquipmentID == null || TcpExtendTemp.EquipmentID.Equals(""))
                    {
                        TcpExtendTemp.EquipmentID = df.deviceid;
                    }
                    //存入数据库
                    if (df.contentjson != null && df.contentjson != "")
                    {
                        HXM_Relay_DB.Save_HXM_Relay_status(df);
                    }
                }
                // 地址2开启 02 05 00 00 FF 00 8C 09
                //应答：02 05 00 00 ff 00 8c 09
                else if (command == 0x05)//开启状态
                {
                    HXM_Relay_Switch hXM_Relay_OpenOrClose = new HXM_Relay_Switch();

                    if (TcpExtendTemp.EquipmentID == null || TcpExtendTemp.EquipmentID.Equals(""))
                        return;
                    hXM_Relay_OpenOrClose.DTUID = TcpExtendTemp.EquipmentID;
                    hXM_Relay_OpenOrClose.Addr485 = addr485.ToString();
                    hXM_Relay_OpenOrClose.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    //Int16 RelayI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 2);
                    //hXM_Relay_OpenOrClose.RelaySN = RelayI.ToString();
                    //RelayI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 4);
                    //if (RelayI > 0) hXM_Relay_OpenOrClose.SwitchStatus = "open";
                    //else hXM_Relay_OpenOrClose.SwitchStatus = "close";

                    //考虑到继电器不会被高频率开启，在此处直接进行数据存储
                    Relay_DB.UpdateRelaySwitch(hXM_Relay_OpenOrClose.DTUID, hXM_Relay_OpenOrClose.Addr485,"2");
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_Analyze.AnalyzeProcess异常", ex.Message);
            }
        }
        #endregion
    }
}
