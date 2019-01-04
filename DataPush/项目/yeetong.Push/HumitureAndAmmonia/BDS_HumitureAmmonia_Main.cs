using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace yeetong_Push
{
    public class BDS_HumitureAmmonia_Main
    {
        public static QClient qc_TowerCrane = null;

        private static Thread TowerCraneProcess = null;

        private static List<Thread> WorkLst = new List<Thread>();
        public static void App_Open()
        {
            try
            {
                WorkLst.Add(TowerCraneProcess);
               
                #region//为了提高转发效率 十个线程分流
                //实时数据的同步和转发
                TowerCraneProcess = new Thread(ForwardSNEndNumberIs) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcess.Start();
                #endregion
                qc_TowerCrane = new QClient();

                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }

        private static void ForwardSNEndNumberIs()
        {
            PushService();
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
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }
        private  static void PushService()
        {
            while (true)
            {
                try
                {
                    //先从数据库获取
                    IList<HumitureAndAmmoniaDBFrame> forwardconfigResult = HumitureAndAmmonia_LocalDB.GetHumitureAndAmmonia();
                    if (forwardconfigResult != null && forwardconfigResult.Count > 0)
                    {
                        foreach (HumitureAndAmmoniaDBFrame dbf in forwardconfigResult)
                        {
                            if (dbf.version == "1.0")
                                ;
                            //获得值接下来进行拼接推送
                            else
                                HumitureAndAmmonia_LocalDB.UpdateHumitureAndAmmoniadbtypeByid(dbf.id);
                            //走推送的业务
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
