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
                //先得到这个设备对应的dtu的设备编号
                TcpClientBindingExternalClass TcpExtendTemp = client.External.External as TcpClientBindingExternalClass;
                DBFrame df = new DBFrame();
                df.contenthex = ConvertData.ToHexString(b, 0, c);
                df.version = "1.0";//默认写成1.0
                //发送：01 01 00 00 00 08 3D CC
                //返回：01 01 01 00 51 88      继电器全关闭状态 1是开启 0是关闭
                //返回：01 01 01 03 11 89      继电器全部开启状态 1是开启 0是关闭
                byte addr485 = b[0];//485地址
                byte command = b[1];//命令
                byte length = b[2];//数据长度
                if (command == 0x01)//查询状态的
                {
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
                else if(command == 0x05)//开启状态
                {

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
