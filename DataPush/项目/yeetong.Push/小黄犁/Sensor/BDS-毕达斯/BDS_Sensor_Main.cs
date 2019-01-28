using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace yeetong_Push
{
    public class BDS_Sensor_Main
    {
        private static Thread SensorProcess = null;

        public static void App_Open()
        {
            try
            {
                #region//为了提高转发效率 十个线程分流
                //实时数据的同步和转发
                SensorProcess = new Thread(PushService) { IsBackground = true, Priority = ThreadPriority.Highest };
                SensorProcess.Start();
                #endregion

                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void App_Close()
        {
            try
            {
             
                    if(SensorProcess != null&& SensorProcess.IsAlive)
                    {
                        SensorProcess.Abort();
                        SensorProcess = null;
                    }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_Main启动关闭出现异常", ex.Message + ex.StackTrace);
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
                    IList<BDS_SensorDBFrame> forwardconfigResult = BDS_Sensor_LocalDB.GetSensor();
                    if (forwardconfigResult != null && forwardconfigResult.Count > 0)
                    {
                        foreach (BDS_SensorDBFrame dbf in forwardconfigResult)
                        {
                            if (dbf.version == "1.0")
                                BDS_Sensor_PushProcess.BDS_SensorAnalyse(dbf);
                            else
                                BDS_Sensor_LocalDB.UpdateSensordbtypeByid(dbf.id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("PushService异常", ex.Message);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
