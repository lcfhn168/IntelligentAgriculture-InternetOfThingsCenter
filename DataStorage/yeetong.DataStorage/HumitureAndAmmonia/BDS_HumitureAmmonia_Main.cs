using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace yeetong_DataStorage
{
    public class HumitureAndAmmonia_Main
    {
        private static  Thread HumitureAndAmmoniaProcessT = null;//实时数据的同步和转发

        public static void App_Open()
        {
            try
            {
                //实时数据的同步和转发
                HumitureAndAmmoniaProcessT = new Thread(HumitureAndAmmoniaProcess) { IsBackground = true, Priority = ThreadPriority.Highest };
                HumitureAndAmmoniaProcessT.Start();

                ToolAPI.XMLOperation.WriteLogXmlNoTail("HumitureAndAmmonia_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HumitureAndAmmonia_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void App_Close()
        {
            try
            {
                if (HumitureAndAmmoniaProcessT != null && HumitureAndAmmoniaProcessT.IsAlive)
                {
                    HumitureAndAmmoniaProcessT.Abort();
                    HumitureAndAmmoniaProcessT = null;
                }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HumitureAndAmmonia_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HumitureAndAmmonia_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }

        static void HumitureAndAmmoniaProcess()
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
                                BDS_HumitureAmmonia_DB.BDS_HumitureAmmoniaAnalyse(dbf);
                            else
                                HumitureAndAmmonia_LocalDB.UpdateHumitureAndAmmoniadbtypeByid(dbf.id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("HumitureAndAmmoniaProcess异常", ex.Message);
                }
                Thread.Sleep(1000);
            }

        }
    }
}
