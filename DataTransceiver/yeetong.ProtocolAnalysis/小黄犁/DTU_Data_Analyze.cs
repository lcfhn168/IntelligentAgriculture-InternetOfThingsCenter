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
    public class DTU_Data_Analyze
    {
        #region 解析入口
        /// <summary>
        /// 解析、存储数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void AnalyzeProcess(byte[] b, int c, TcpSocketClient client)
        {
            try
            {
                //先得到这个设备对应的dtu的设备编号
                TcpClientBindingExternalClass TcpExtendTemp = client.External.External as TcpClientBindingExternalClass;
                #region 心跳
                //心跳79 74 31 32 33 34 35 36
                if (c > 2 && (b[0] == 0x79) && (b[1] == 0x74))//心跳 默认yt开头
                {
                    DBFrame df = new DBFrame();
                    df.contenthex = ConvertData.ToHexString(b, 0, c);
                    df.version = "1.0";//默认写成1.0

                    byte[] contentAry = new byte[8];
                    Array.Copy(b, 0, contentAry, 0, 8);
                    string contentStr = Encoding.ASCII.GetString(contentAry);
                    DTU_Data_Heartbeat dTU_Data_Heartbeat = new DTU_Data_Heartbeat();
                    dTU_Data_Heartbeat.DTUID = contentStr;
                    dTU_Data_Heartbeat.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    df.deviceid = dTU_Data_Heartbeat.DTUID;
                    df.datatype = "heartbeat";
                    df.contentjson = JsonConvert.SerializeObject(dTU_Data_Heartbeat);

                    if (TcpExtendTemp.EquipmentID == null || TcpExtendTemp.EquipmentID.Equals(""))
                    {
                        TcpExtendTemp.EquipmentID = df.deviceid;
                    }
                    //存入数据库
                    if (df.contentjson != null && df.contentjson != "")
                    {
                        DTU_Data_DB.SaveDTU_Data(df);
                    }
                }
                #endregion
                else
                {
                    //无论是传感器还是控制器都遵循modbus校验
                    if (c > 2)
                    {
                        byte[] btemp = new byte[c - 2];
                        Array.Copy(b, 0, btemp, 0, c - 2);
                        byte[] value = Tool.ToModbus(btemp);
                        if (value[0] != b[c - 2] || value[1] != b[c - 1])
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("无效包", ConvertData.ToHexString(b, 0, c));
                            return;
                        }
                    }
                    //根据485的地址来判断是传感器还是继电器 默认奇数为传感器，偶数为继电器
                    if(b[0]%2==0)//继电器
                    {
                        HXM_Relay_Analyze.AnalyzeProcess(b, c, client);
                    }
                    else//传感器
                    {
                        BDS_Sensor_Analyze.AnalyzeProcess(b,c, client);
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Analyze.AnalyzeProcess异常", ex.Message);
            }
        }
        #endregion
    }
}
