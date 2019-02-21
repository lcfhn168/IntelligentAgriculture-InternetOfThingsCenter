using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace yeetong_DataStorage
{
    public class BDS_Sensor_Main
    {
        private static Thread SensorProcessT = null;//实时数据的同步和转发

        public static void App_Open()
        {
            try
            {
                //实时数据的同步和转发
                SensorProcessT = new Thread(SensorProcess) { IsBackground = true, Priority = ThreadPriority.Highest };
                SensorProcessT.Start();

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
                if (SensorProcessT != null && SensorProcessT.IsAlive)
                {
                    SensorProcessT.Abort();
                    SensorProcessT = null;
                }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }

        static void SensorProcess()
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
                                BDS_Sensor_DB.BDS_SensorAnalyse(dbf);

                            BDS_Sensor_LocalDB.UpdateSensordbtypeByid(dbf.id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("SensorProcess异常", ex.Message);
                }
                Thread.Sleep(1000);
            }

        }
    }
}
