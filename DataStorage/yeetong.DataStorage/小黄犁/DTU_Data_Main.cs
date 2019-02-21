using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace yeetong_DataStorage
{
    public class DTU_Data_Main
    {
        private static Thread DTU_Data_ProcessT = null;//实时数据的同步和转发

        public static void App_Open()
        {
            try
            {
                //实时数据的同步和转发
                DTU_Data_ProcessT = new Thread(DTU_Data_Process) { IsBackground = true, Priority = ThreadPriority.Highest };
                DTU_Data_ProcessT.Start();

                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void App_Close()
        {
            try
            {
                if (DTU_Data_ProcessT != null && DTU_Data_ProcessT.IsAlive)
                {
                    DTU_Data_ProcessT.Abort();
                    DTU_Data_ProcessT = null;
                }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }

        static void DTU_Data_Process()
        {

            while (true)
            {
                try
                {
                    //先从数据库获取
                    IList<DTU_DataDBFrame> forwardconfigResult = DTU_Data_LocalDB.GetDTU_Data();
                    if (forwardconfigResult != null && forwardconfigResult.Count > 0)
                    {
                        foreach (DTU_DataDBFrame dbf in forwardconfigResult)
                        {
                            if (dbf.version == "1.0")
                                DTU_Data_DB.DTU_Data_Analyse(dbf);
                            //去掉标志
                            DTU_Data_LocalDB.UpdateDTU_DatatypeByid(dbf.id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_Process异常", ex.Message);
                }
                Thread.Sleep(1000);
            }

        }
    }
}
