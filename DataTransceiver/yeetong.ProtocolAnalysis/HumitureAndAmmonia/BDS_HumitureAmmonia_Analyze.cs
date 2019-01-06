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
    public class BDS_HumitureAmmonia_Analyze
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
                //心跳79 74 31 32 33 34 35 36
                if (c > 2 && (b[0] == 0x79) && (b[1] == 0x74))//心跳 默认yt开头
                {
                    byte[] contentAry = new byte[8];
                    Array.Copy(b, 0, contentAry, 0, 8);
                    string contentStr = Encoding.ASCII.GetString(contentAry);
                    BDS_HumitureAmmonia_Heartbeat bDS_HumitureAmmonia_Heartbeat = new BDS_HumitureAmmonia_Heartbeat();
                    bDS_HumitureAmmonia_Heartbeat.DTUID = contentStr;
                    bDS_HumitureAmmonia_Heartbeat.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    df.deviceid = bDS_HumitureAmmonia_Heartbeat.DTUID;
                    df.datatype = "heartbeat";
                    df.contentjson = JsonConvert.SerializeObject(bDS_HumitureAmmonia_Heartbeat);
                }
                //实时数据发：01 03 00 00 00 11 85 c6
                //实时数据收：01 03 22 00 0A 0E AA 10 52 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 01 00 06 19 4D
                else
                {
                    //校验
                    if (c > 2)
                    {
                        byte[] btemp = new byte[c - 2];
                        Array.Copy(b, 0, btemp, 0, c - 2);
                        byte[] value = ToModbus(btemp);
                        if (value[0] != b[c - 2] || value[1] != b[c - 1])
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("无效包", ConvertData.ToHexString(b, 0, c));
                            return;
                        }
                    }
                    //寄存器 温度01 湿度02 氨气 16
                    byte addr485 = b[0];//485地址
                    byte command = b[1];//命令
                    byte length = b[2];//数据长度

                    BDS_HumitureAmmonia_Current bds_HumitureAmmonia_Current = new BDS_HumitureAmmonia_Current();
                    if (TcpExtendTemp.EquipmentID == null || TcpExtendTemp.EquipmentID.Equals(""))
                        return;
                    bds_HumitureAmmonia_Current.DTUID = TcpExtendTemp.EquipmentID;
                    bds_HumitureAmmonia_Current.Addr485 = addr485.ToString();
                    bds_HumitureAmmonia_Current.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //温度
                    Int16 TemperatureI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 5);
                    double TemperatureD = ((double)(TemperatureI - 2000)) / 100d;
                    bds_HumitureAmmonia_Current.Temperature = TemperatureD;
                    //湿度
                    Int16 HumidityI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 7);
                    double HumidityD = (double)HumidityI / 100d;
                    bds_HumitureAmmonia_Current.Humidity = HumidityD;
                    //氨气
                    Int16 AmmoniaI = ToolAPI.ByteArrayToValueType.GetInt16_BigEndian(b, 35);
                    bds_HumitureAmmonia_Current.Ammonia = AmmoniaI;

                    df.deviceid = bds_HumitureAmmonia_Current.DTUID;
                    df.datatype = "current";
                    df.contentjson = JsonConvert.SerializeObject(bds_HumitureAmmonia_Current);
                }

                if (TcpExtendTemp.EquipmentID == null || TcpExtendTemp.EquipmentID.Equals(""))
                {
                    TcpExtendTemp.EquipmentID = df.deviceid;
                }
                //存入数据库
                if (df.contentjson != null && df.contentjson != "")
                {
                    BDS_HumitureAmmonia_DB.SaveBDSHumitureAmmonia(df);
                }
                //设备的拷贝
                string sourId = df.deviceid;
                //数据库的拷贝
                if (!string.IsNullOrEmpty(MainStatic.DeviceCopy_BDS))
                {
                //    ToolAPI.XMLOperation.WriteLogXmlNoTail("1.0", MainStatic.DeviceCopy_BDS);
                //    ToolAPI.XMLOperation.WriteLogXmlNoTail("1.1", sourId);
                    if (MainStatic.DeviceCopy_BDS.Contains(sourId + "#"))
                    {
                        //ToolAPI.XMLOperation.WriteLogXmlNoTail("2", sourId + "#");
                        try
                        {
                            string[] strary = MainStatic.DeviceCopy_BDS.Split(';');
                            //ToolAPI.XMLOperation.WriteLogXmlNoTail("3", "");
                            foreach (string dev in strary)
                            {
                                //ToolAPI.XMLOperation.WriteLogXmlNoTail("4", "");
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
                                        //ToolAPI.XMLOperation.WriteLogXmlNoTail("5", "");
                                        BDS_HumitureAmmonia_DB.SaveBDSHumitureAmmonia(df);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Analyze.AnalyzeProcess拷贝异常", ex.Message);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Analyze.AnalyzeProcess异常", ex.Message);
            }
        }
        #endregion

        #region 下发的拼接
        /// <summary>
        /// 上传实时数据拼接
        /// </summary>
        /// <param name="addr">485对应的地址</param>
        /// <returns></returns>
        public static byte[] UploadCurrentSplitJoint(byte addr)
        {
            try
            {
                List<byte> byteTemp = new List<byte>();
                byteTemp.Add(addr);//地址
                byteTemp.AddRange(new byte[] { 0x03, 0x00, 0x00, 0x00, 0x11 });//命令+寄存此起始地址+寄存器个数
                byteTemp.AddRange(ToModbus(byteTemp.ToArray()));//校验
                byte[] sendbutter = byteTemp.ToArray();
                return sendbutter;
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Analyze.UploadCurrentSplitJoint异常", ex.Message);
                return null;
            }
        }
        #endregion

        #region 其他
        //CRC16校验
        public static byte[] ToModbus(byte[] byteData)
        {
            byte[] CRC = new byte[2];

            UInt16 wCrc = 0xFFFF;
            for (int i = 0; i < byteData.Length; i++)
            {
                wCrc ^= Convert.ToUInt16(byteData[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) == 1)
                    {
                        wCrc >>= 1;
                        wCrc ^= 0xA001;//异或多项式
                    }
                    else
                    {
                        wCrc >>= 1;
                    }
                }
            }

            CRC[1] = (byte)((wCrc & 0xFF00) >> 8);//高位在后
            CRC[0] = (byte)(wCrc & 0x00FF);       //低位在前
            return CRC;

        }
        #endregion
    }
}
