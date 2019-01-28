using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TCPAPI;
using ToolAPI;
using yeetong_Architecture;

namespace yeetong_ProtocolAnalysis
{
    class BDS_Sensor_issued
    {
        /// <summary>
        /// 获取传感器数据
        /// </summary>
        public static void Get_BDS_Sensor(IList<TcpSocketClient> SocketList)
        {
            try
            {
                for (int j = 0; j < SocketList.Count; j++)
                {
                    string DTUID = (SocketList[j].External.External as TcpClientBindingExternalClass).EquipmentID;
                    DateTime? dateTimeIssued = (SocketList[j].External.External as TcpClientBindingExternalClass).DateTimeIssued;

                    if (!string.IsNullOrEmpty(DTUID))
                    {
                        if (dateTimeIssued == null || (DateTime.Now - (DateTime)dateTimeIssued).TotalSeconds >= 300)//进行下发处理
                        {
                            (SocketList[j].External.External as TcpClientBindingExternalClass).DateTimeIssued = DateTime.Now;
                            //执行下发
                            DataTable dt = BDS_Sensor_DB.Get_bds_sensorAddr485(DTUID);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    byte[] sendbuffer =UploadCurrentSplitJoint(byte.Parse(dt.Rows[i]["equipment_485_addr"].ToString()));
                                    if (sendbuffer != null)
                                    {
                                        SocketList[j].SendBuffer(sendbuffer);
                                        ToolAPI.XMLOperation.WriteLogXmlNoTail("命令下发：" + DTUID, ConvertData.ToHexString(sendbuffer, 0, sendbuffer.Length));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Get_BDS_Sensor异常", ex.Message);
            }
        }


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
                byteTemp.AddRange(Tool.ToModbus(byteTemp.ToArray()));//校验
                byte[] sendbutter = byteTemp.ToArray();
                return sendbutter;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_issued.UploadCurrentSplitJoint异常", ex.Message);
                return null;
            }
        }
        #endregion
    }
}
