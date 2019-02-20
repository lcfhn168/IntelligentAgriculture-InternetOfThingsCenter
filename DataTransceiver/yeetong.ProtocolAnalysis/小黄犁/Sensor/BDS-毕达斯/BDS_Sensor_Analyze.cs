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
                //地址0未知 b[3] b[4]
                //温度 b[5] b[6]
                Int16 TemperatureI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 5);
                double TemperatureD = ((double)(TemperatureI - 2000)) / 100d;
                bDS_Sensor_Current.SnsorValue.A = TemperatureD;
                //湿度
                Int16 HumidityI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 7);
                double HumidityD = (double)HumidityI / 100d;
                bDS_Sensor_Current.SnsorValue.B = HumidityD;
                //光照
                Int16 ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 9);
                bDS_Sensor_Current.SnsorValue.C = ValueI;
                //紫外线
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 11);
                bDS_Sensor_Current.SnsorValue.D = ValueI;
                //大气压
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 13);
                bDS_Sensor_Current.SnsorValue.E = ValueI;
                //声音
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 15);
                bDS_Sensor_Current.SnsorValue.F = ValueI;
                //PM1.0
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 17);
                bDS_Sensor_Current.SnsorValue.G = ValueI;
                //PM2.5
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 19);
                bDS_Sensor_Current.SnsorValue.H = ValueI;
                //PM10
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 21);
                bDS_Sensor_Current.SnsorValue.I = ValueI;
                //电压值
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 23);
                bDS_Sensor_Current.SnsorValue.J = ValueI;
                //烟雾
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 25);
                bDS_Sensor_Current.SnsorValue.K = ValueI;
                //酒精
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 27);
                bDS_Sensor_Current.SnsorValue.L = ValueI;
                //甲烷
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 29);
                bDS_Sensor_Current.SnsorValue.M = ValueI;
                //丙烷
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 31);
                bDS_Sensor_Current.SnsorValue.N = ValueI;
                //氢气
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 33);
                bDS_Sensor_Current.SnsorValue.O = ValueI;
                //氨气NH3
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 35);
                bDS_Sensor_Current.SnsorValue.P = ValueI;
                //甲苯
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 37);
                bDS_Sensor_Current.SnsorValue.Q = ValueI;
                //一氧化碳
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 39);
                bDS_Sensor_Current.SnsorValue.R = ValueI;
                //二氧化碳
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 41);
                bDS_Sensor_Current.SnsorValue.S = ValueI;
                //氧气
                ValueI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 43);
                bDS_Sensor_Current.SnsorValue.T = ValueI;

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
