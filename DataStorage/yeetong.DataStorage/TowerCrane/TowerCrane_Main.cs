using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using yeetong_DataStorage._021303;
using yeetong_DataStorage._0E;

namespace yeetong_DataStorage
{
    public class TowerCrane_Main
    {
        private static  Thread TowerCraneProcessT = null;//实时数据的同步和转发

        public static void App_Open()
        {
            try
            {
                //实时数据的同步和转发
                TowerCraneProcessT = new Thread(TowerCraneProcess) { IsBackground = true, Priority = ThreadPriority.Highest };
                TowerCraneProcessT.Start();

                ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCrane_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCrane_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void App_Close()
        {
            try
            {
                if (TowerCraneProcessT != null && TowerCraneProcessT.IsAlive)
                {
                    TowerCraneProcessT.Abort();
                    TowerCraneProcessT = null;
                }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCrane_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("TowerCrane_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }

        static void TowerCraneProcess()
        {

            while (true)
            {
                try
                {
                    //先从数据库获取
                    IList<TowerCraneDBFrame> forwardconfigResult = MysqlTowerCrane_Local.GetTowerCrane();
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("GetTowerCrane数量", forwardconfigResult.Count.ToString()+"; "+DateTime.Now.ToString());
                    if (forwardconfigResult != null && forwardconfigResult.Count > 0)
                    {
                        foreach (TowerCraneDBFrame dbf in forwardconfigResult)
                        {
                            if (dbf.version == "A55A021303")
                                MysqlTowerCrane_021303.TowerCraneDBFrameAnalyse(dbf);
                            else if (dbf.version == "7E7E0E")
                                MysqlTowerCrane_0E.TowerCraneTowerCraneDBFrameAnalyse(dbf);
                            else
                                MysqlTowerCrane_Local.UpdateTowerCranedbtypeByid(dbf.id);
                        }
                    }
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("GetTowerCrane遍历结束",  DateTime.Now.ToString());
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
