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
    class BDS_HumitureAmmonia_issued
    {
        /// <summary>
        /// 获取温湿度和氨气
        /// </summary>
        public static void Get_BDS_HumitureAmmonia(IList<TcpSocketClient> SocketList)
        {
            try
            {
                //ToolAPI.XMLOperation.WriteLogXmlNoTail("1", "");
                for (int j = 0; j < SocketList.Count; j++)
                {
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("2", "");
                    string DTUID = (SocketList[j].External.External as TcpClientBindingExternalClass).EquipmentID;
                    DateTime? dateTimeIssued = (SocketList[j].External.External as TcpClientBindingExternalClass).DateTimeIssued;
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("3", "");
                    if (!string.IsNullOrEmpty(DTUID))
                    {
                        //ToolAPI.XMLOperation.WriteLogXmlNoTail("4", "");
                        if (dateTimeIssued == null || (DateTime.Now - (DateTime)dateTimeIssued).TotalSeconds >= 600)//进行下发处理
                        {
                            //ToolAPI.XMLOperation.WriteLogXmlNoTail("5", "");
                            (SocketList[j].External.External as TcpClientBindingExternalClass).DateTimeIssued = DateTime.Now;
                            //执行下发
                            DataTable dt = BDS_HumitureAmmonia_DB.Get_BDS_HumitureammoniaAddr485(DTUID);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                //ToolAPI.XMLOperation.WriteLogXmlNoTail("6", "");
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("7", "");
                                    byte[] sendbuffer = BDS_HumitureAmmonia_Analyze.UploadCurrentSplitJoint(byte.Parse(dt.Rows[i]["equipment_485_addr"].ToString()));
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
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Get_BDS_HumitureAmmonia异常", ex.Message);
            }
        } 
    }
}
