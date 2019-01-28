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
    public class BDS_Sensor_Analyze
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
                DBFrame df = new DBFrame();
                df.contenthex = ConvertData.ToHexString(b, 0, c);
                df.version = "1.0";//默认写成1.0
                //实时数据发：01 03 00 00 00 11 85 c6
                //实时数据收：01 03 22 00 0A 0E AA 10 52 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 06 19 4D
                //寄存器 温度01 湿度02 氨气 16
                byte addr485 = b[0];//485地址
                byte command = b[1];//命令
                byte length = b[2];//数据长度

                BDS_Sensor_Current bDS_Sensor_Current = new BDS_Sensor_Current();
                if (TcpExtendTemp.EquipmentID == null || TcpExtendTemp.EquipmentID.Equals(""))
                    return;
                bDS_Sensor_Current.DTUID = TcpExtendTemp.EquipmentID;
                bDS_Sensor_Current.Addr485 = addr485.ToString();
                bDS_Sensor_Current.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //温度
                Int16 TemperatureI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 5);
                double TemperatureD = ((double)(TemperatureI - 2000)) / 100d;
                bDS_Sensor_Current.Temperature = TemperatureD;
                //湿度
                Int16 HumidityI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 7);
                double HumidityD = (double)HumidityI / 100d;
                bDS_Sensor_Current.Humidity = HumidityD;
                //氨气
                Int16 AmmoniaI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 35);
                bDS_Sensor_Current.Ammonia = AmmoniaI;

                df.deviceid = bDS_Sensor_Current.DTUID;
                df.datatype = "current";
                df.contentjson = JsonConvert.SerializeObject(bDS_Sensor_Current);

                if (TcpExtendTemp.EquipmentID == null || TcpExtendTemp.EquipmentID.Equals(""))
                {
                    TcpExtendTemp.EquipmentID = df.deviceid;
                }
                //存入数据库
                if (df.contentjson != null && df.contentjson != "")
                {
                    BDS_Sensor_DB.Save_bds_sensor(df);
                }
                //设备的拷贝
                string sourId = df.deviceid;
                //数据库的拷贝
                if (!string.IsNullOrEmpty(MainStatic.DeviceCopy_BDS))
                {
                    if (MainStatic.DeviceCopy_BDS.Contains(sourId + "#"))
                    {
                        try
                        {
                            string[] strary = MainStatic.DeviceCopy_BDS.Split(';');
                            foreach (string dev in strary)
                            {
                                if (dev.Contains(sourId + "#"))
                                {
                                    string[] devcopy = dev.Split('#');
                                    DBFrame dfcopy = DBFrame.DeepCopy(df);
                                    dfcopy.deviceid = devcopy[1];
                                    dfcopy.datatype = "current";
                                    dfcopy.contentjson.Replace(sourId, devcopy[1]);
                                    ToolAPI.XMLOperation.WriteLogXmlNoTail("5", "");
                                    if (dfcopy.contentjson != null && dfcopy.contentjson != "")
                                    {
                                        BDS_Sensor_DB.Save_bds_sensor(df);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_Analyze.AnalyzeProcess拷贝异常", ex.Message);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_Analyze.AnalyzeProcess异常", ex.Message);
            }
        }
        #endregion
    }
}
