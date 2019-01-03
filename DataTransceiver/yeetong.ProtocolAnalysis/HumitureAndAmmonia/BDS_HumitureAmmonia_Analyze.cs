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
