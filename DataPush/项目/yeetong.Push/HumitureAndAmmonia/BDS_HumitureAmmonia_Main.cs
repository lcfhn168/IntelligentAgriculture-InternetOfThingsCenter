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
        private static Thread HumitureAndAmmoniaProcess = null;

        public static void App_Open()
        {
            try
            {
                #region//为了提高转发效率 十个线程分流
                //实时数据的同步和转发
                HumitureAndAmmoniaProcess = new Thread(PushService) { IsBackground = true, Priority = ThreadPriority.Highest };
                HumitureAndAmmoniaProcess.Start();
                #endregion

                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void App_Close()
        {
            try
            {
             
                    if(HumitureAndAmmoniaProcess != null&& HumitureAndAmmoniaProcess.IsAlive)
                    {
                        HumitureAndAmmoniaProcess.Abort();
                        HumitureAndAmmoniaProcess = null;
                    }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 推送服务
        /// </summary>
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
                                BDS_HumitureAmmonia_PushProcess.BDS_HumitureAmmoniaAnalyse(dbf);
                            else
                                HumitureAndAmmonia_LocalDB.UpdateHumitureAndAmmoniadbtypeByid(dbf.id);
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
