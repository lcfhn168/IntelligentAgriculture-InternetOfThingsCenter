using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using MQTTPushService._021303;
using MQTTPushService._0E;
using MQTTPushService.TowerCrane;

namespace MQTTPushService
{
    public class TowerCrane_Main
    {
        public static QClient qc_TowerCrane = null;

        private static Thread TowerCraneProcess0 = null;
        private static Thread TowerCraneProcess1 = null;
        private static Thread TowerCraneProcess2 = null;
        private static Thread TowerCraneProcess3 = null;
        private static Thread TowerCraneProcess4 = null;
        private static Thread TowerCraneProcess5 = null;
        private static Thread TowerCraneProcess6 = null;
        private static Thread TowerCraneProcess7 = null;
        private static Thread TowerCraneProcess8 = null;
        private static Thread TowerCraneProcess9 = null;

        private static List<Thread> WorkLst = new List<Thread>();
        public static void App_Open()
        {
            try
            {
                WorkLst.Add(TowerCraneProcess0);
                WorkLst.Add(TowerCraneProcess1);
                WorkLst.Add(TowerCraneProcess2);
                WorkLst.Add(TowerCraneProcess3);
                WorkLst.Add(TowerCraneProcess4);
                WorkLst.Add(TowerCraneProcess5);
                WorkLst.Add(TowerCraneProcess6);
                WorkLst.Add(TowerCraneProcess7);
                WorkLst.Add(TowerCraneProcess8);
                WorkLst.Add(TowerCraneProcess9);
                #region//为了提高转发效率 十个线程分流
                //实时数据的同步和转发
                TowerCraneProcess0 = new Thread(ForwardSNEndNumberIs0) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess0.Start();
                TowerCraneProcess1 = new Thread(ForwardSNEndNumberIs1) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess1.Start();
                TowerCraneProcess2 = new Thread(ForwardSNEndNumberIs2) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess2.Start();
                TowerCraneProcess3 = new Thread(ForwardSNEndNumberIs3) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess3.Start();
                TowerCraneProcess4 = new Thread(ForwardSNEndNumberIs4) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess4.Start();
                TowerCraneProcess5 = new Thread(ForwardSNEndNumberIs5) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess5.Start();
                TowerCraneProcess6 = new Thread(ForwardSNEndNumberIs6) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess6.Start();
                TowerCraneProcess7 = new Thread(ForwardSNEndNumberIs7) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess7.Start();
                TowerCraneProcess8 = new Thread(ForwardSNEndNumberIs8) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess8.Start();
                TowerCraneProcess9 = new Thread(ForwardSNEndNumberIs9) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess9.Start();
                #endregion
                qc_TowerCrane = new QClient();

                ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCrane_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCrane_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }

        private static void ForwardSNEndNumberIs0()
        {
            string endNumer = "0";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs1()
        {
            string endNumer = "1";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs2()
        {
            string endNumer = "2";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs3()
        {
            string endNumer = "3";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs4()
        {
            string endNumer = "4";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs5()
        {
            string endNumer = "5";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs6()
        {
            string endNumer = "6";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs7()
        {
            string endNumer = "7";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs8()
        {
            string endNumer = "8";
            PushService(endNumer);
        }
        private static void ForwardSNEndNumberIs9()
        {
            string endNumer = "9";
            PushService(endNumer);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void App_Close()
        {
            try
            {
                WorkLst.ForEach(x => 
                {
                    if(x!=null&&x.IsAlive)
                    {
                        x.Abort();
                        x = null;
                    }
                });
                qc_TowerCrane.ClientClose();
                ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCrane_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCrane_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }
        private  static void PushService(string endNumber)
        {
            while (true)
            {
                try
                {
                    //先从数据库获取
                    IList<DBFrame> forwardconfigResult = MysqlTowerCrane_Local.GetTowerCrane(endNumber);
                    if (forwardconfigResult != null && forwardconfigResult.Count > 0)
                    {
                        foreach (DBFrame dbf in forwardconfigResult)
                        {
                            MQTTFrame mf = null;
                            if (dbf.version == "A55A021303")
                                mf = MqttDataEncapsulation_021303.MQTTFrameAnalyse(dbf);
                            else if (dbf.version == "7E7E0E")
                                mf = MqttTowerCrane_0E.MQTTFrameAnalyse(dbf);
                            else
                                MysqlTowerCrane_Local.UpdateTowerCranedbtypeByid(dbf.id);
                            MQTTSend.MQTT_SendData(mf);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCraneProcess异常", ex.Message);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
