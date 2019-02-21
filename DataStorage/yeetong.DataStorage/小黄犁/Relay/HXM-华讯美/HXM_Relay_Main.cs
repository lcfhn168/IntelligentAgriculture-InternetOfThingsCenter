using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace yeetong_DataStorage
{
    public class HXM_Relay_Main
    {
        private static Thread HXM_RelayProcessT = null;//实时数据的同步和转发

        public static void App_Open()
        {
            try
            {
                //实时数据的同步和转发
                HXM_RelayProcessT = new Thread(HXM_RelayProcess) { IsBackground = true, Priority = ThreadPriority.Highest };
                HXM_RelayProcessT.Start();

                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void App_Close()
        {
            try
            {
                if (HXM_RelayProcessT != null && HXM_RelayProcessT.IsAlive)
                {
                    HXM_RelayProcessT.Abort();
                    HXM_RelayProcessT = null;
                }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }

        static void HXM_RelayProcess()
        {

            while (true)
            {
                try
                {
                    //先从数据库获取
                    IList<HXM_RelayDBFrame> forwardconfigResult = HXM_Relay_LocalDB.GetHXM_Relay();
                    if (forwardconfigResult != null && forwardconfigResult.Count > 0)
                    {
                        foreach (HXM_RelayDBFrame dbf in forwardconfigResult)
                        {
                            if (dbf.version == "1.0")
                                HXM_Relay_DB.HXM_RelayAnalyse(dbf);

                            HXM_Relay_LocalDB.UpdateHXM_RelaytypeByid(dbf.id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_RelayProcess异常", ex.Message);
                }
                Thread.Sleep(1000);
            }

        }
    }
}
