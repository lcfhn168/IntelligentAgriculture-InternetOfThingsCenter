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
    public  class BDS_HumitureAmmonia_Analyze
    {
        #region 解析入口
        /// <summary>
        /// 解析、存储数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static string AnalyzeProcess(byte[] b, int c, TcpSocketClient client)
        {
            try
            {
                DBFrame df = new DBFrame();
                df.contenthex = ConvertData.ToHexString(b, 0, c);
                df.version = "1.0";//默认写成1.0
                                   //解析 todo

                ////心跳测试
                //BDS_HumitureAmmonia_Heartbeat bDS_HumitureAmmonia_Heartbeat = new BDS_HumitureAmmonia_Heartbeat();
                //bDS_HumitureAmmonia_Heartbeat.DTUID = "12345678";
                //bDS_HumitureAmmonia_Heartbeat.Addr485 = "1";
                //bDS_HumitureAmmonia_Heartbeat.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //df.deviceid = bDS_HumitureAmmonia_Heartbeat.DTUID;
                //df.datatype = "heartbeat";
                //df.contentjson = JsonConvert.SerializeObject(bDS_HumitureAmmonia_Heartbeat);
                //实时数据测试
                BDS_HumitureAmmonia_Current bds_HumitureAmmonia_Current = new BDS_HumitureAmmonia_Current();
                bds_HumitureAmmonia_Current.DTUID = "12345678";
                bds_HumitureAmmonia_Current.Addr485 = "1";
                bds_HumitureAmmonia_Current.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                bds_HumitureAmmonia_Current.Temperature = 12.1;
                bds_HumitureAmmonia_Current.Humidity = 11.1;
                bds_HumitureAmmonia_Current.Ammonia = 10.1;

                df.deviceid = bds_HumitureAmmonia_Current.DTUID;
                df.datatype = "current";
                df.contentjson = JsonConvert.SerializeObject(bds_HumitureAmmonia_Current);

                TcpClientBindingExternalClass TcpExtendTemp = client.External.External as TcpClientBindingExternalClass;
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
                return "";
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Analyze.AnalyzeProcess异常", ex.Message);
                return "";
            }
        }
        #endregion
    }
}
