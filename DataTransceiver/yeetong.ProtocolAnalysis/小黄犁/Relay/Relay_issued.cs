using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using TCPAPI;
using ToolAPI;
using yeetong_Architecture;

namespace yeetong_ProtocolAnalysis
{
    class Relay_issued
    {
        static DateTime? TimeFlag;
        static bool IsOperate=false;
        /// <summary>
        /// 获取继电器的状态
        /// </summary>
        public static void Get_HXM_Relay(IList<TcpSocketClient> SocketList)
        {
            try
            {
                //时间间隔为5分钟 执行一下数据获取的操作
                if (TimeFlag == null || (DateTime.Now - (DateTime)TimeFlag).TotalSeconds >= 300)
                {
                    TimeFlag = DateTime.Now;

                    for (int j = 0; j < SocketList.Count; j++)
                    {
                        string DTUID = (SocketList[j].External.External as TcpClientBindingExternalClass).EquipmentID;

                        if (!string.IsNullOrEmpty(DTUID))
                        {
                            //执行下发
                            DataTable dt = Relay_DB.Get_RelayAddr485(DTUID);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    byte[] sendbuffer = UploadCurrentSplitJoint(byte.Parse(dt.Rows[i]["equipment_485_addr"].ToString()));
                                    if (sendbuffer != null)
                                    {
                                        SocketList[j].SendBuffer(sendbuffer);
                                        ToolAPI.XMLOperation.WriteLogXmlNoTail("Relay查询状态命令下发：" + DTUID, ConvertData.ToHexString(sendbuffer, 0, sendbuffer.Length));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Get_HXM_Relay异常", ex.Message);
            }
        }

        /// <summary>
        /// 获取需要操作的继电器
        /// </summary>
        /// <param name="SocketList"></param>
        public static void Get_HXM_RelayUpdate(IList<TcpSocketClient> SocketList)
        {
            try
            {
                if (!IsOperate)
                {
                    IsOperate = true;
                    DataTable dt = Relay_DB.Get_RelayUpdate();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < SocketList.Count; j++)
                            {
                                string DTUID = (SocketList[j].External.External as TcpClientBindingExternalClass).EquipmentID;

                                if (!string.IsNullOrEmpty(DTUID))
                                {
                                    //执行下发
                                    string DTUIDServer = dt.Rows[i]["equipment_dtu_id"].ToString();
                                    if (DTUIDServer == DTUID)
                                    {
                                        //更新数据库
                                        Relay_DB.UpdateRelaySwitch(dt.Rows[i]["equipment_id"].ToString(), 1);
                                        string relayValue = dt.Rows[i]["value"].ToString();
                                        byte addr = byte.Parse(dt.Rows[i]["equipment_485_addr"].ToString());
                                        for (Int16 q = 0; q < relayValue.Length; q++)
                                        {
                                            byte[] sendbuffer = OperateSplitJoint(addr, q, relayValue[relayValue.Length - 1 - q].ToString());
                                            if (sendbuffer != null)
                                            {
                                                SocketList[j].SendBuffer(sendbuffer);
                                                ToolAPI.XMLOperation.WriteLogXmlNoTail("Switch命令下发：" + DTUID+";"+ addr+";"+ q, ConvertData.ToHexString(sendbuffer, 0, sendbuffer.Length));
                                            }
                                            Thread.Sleep(1000);
                                        }
                                        //下发查询状态的指令
                                        byte[] sendbuffer1 = UploadCurrentSplitJoint(addr);
                                        if (sendbuffer1 != null)
                                        {
                                            SocketList[j].SendBuffer(sendbuffer1);
                                            ToolAPI.XMLOperation.WriteLogXmlNoTail("Switch后查询状态命令下发：" + DTUID + ";" + addr, ConvertData.ToHexString(sendbuffer1, 0, sendbuffer1.Length));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    IsOperate = false;
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Get_HXM_Relay异常", ex.Message);
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
                //参考 02 01 00 00 00 08 3D FF
                List<byte> byteTemp = new List<byte>();
                byteTemp.Add(addr);//地址
                byteTemp.AddRange(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x08 });
                byteTemp.AddRange(Tool.ToModbus(byteTemp.ToArray()));//校验
                byte[] sendbutter = byteTemp.ToArray();
                return sendbutter;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Relay_issued.UploadCurrentSplitJoint异常", ex.Message);
                return null;
            }
        }

        public static byte[] OperateSplitJoint(byte addr,Int16 order,string Rswitch)
        {
            try
            {
                //参考 02 05 00 00 FF 00 8C 09  开启
                //02 05 00 00 00 00 CD F9 关闭
                List<byte> byteTemp = new List<byte>();
                byteTemp.Add(addr);//地址
                byteTemp.Add(0x05);//命令
                byteTemp.AddRange(ToolAPI.ValueTypeToByteArray.GetBytes_BigEndian(order));//继电器序号
                if (Rswitch == "1") byteTemp.AddRange(new byte[] { 0xFF,0x00});
                else byteTemp.AddRange(new byte[] { 0x00, 0x00 });

                byteTemp.AddRange(Tool.ToModbus(byteTemp.ToArray()));//校验
                byte[] sendbutter = byteTemp.ToArray();
                return sendbutter;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Relay_issued.OperateSplitJoint异常", ex.Message);
                return null;
            }
        }
        #endregion
    }
}
