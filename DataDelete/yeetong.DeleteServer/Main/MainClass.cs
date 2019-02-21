using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace yeetong_DataDelete
{
    public class MainClass
    {
        private static Thread ProcessT = null;
        /// <summary>
        /// 启动程序
        /// </summary>
        public void App_Open()
        {
            try
            {
                ProcessT = new Thread(Process) { IsBackground = true, Priority = ThreadPriority.Highest };
                ProcessT.Start();

                ToolAPI.XMLOperation.WriteLogXmlNoTail("程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 关闭程序
        /// </summary>
        public void App_Close()
        {
            try
            {
                if (ProcessT != null && ProcessT.IsAlive)
                {
                    ProcessT.Abort();
                    ProcessT = null;
                }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }

        static void Process()
        {

            while (true)
            {
                try
                {
                    DTU_Data_DB.DeleteDTU();
                    BDS_Sensor_DB.DeleteBDSSensor();
                    HXM_Relay_DB.DeleteHXMRelay();
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("Process异常", ex.Message);
                }
                Thread.Sleep(10000);
            }
        }
    }
}
