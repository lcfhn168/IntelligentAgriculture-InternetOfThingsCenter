using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace yeetong_Push
{
    public class Equipment_status_Main
    {
        private static Thread Equipment_statusProcess = null;
        public static void App_Open()
        {
            try
            {
               // PushAPIProcessOn("5ca71bc3e4b02eae9c455aa3");

                //实时数据的同步和转发
                Equipment_statusProcess = new Thread(Equipment_status_Service) { IsBackground = true, Priority = ThreadPriority.Highest };
                Equipment_statusProcess.Start();

                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Equipment_status_Main程序启动", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Equipment_status_Main启动程序出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void App_Close()
        {
            try
            {

                if (Equipment_statusProcess != null && Equipment_statusProcess.IsAlive)
                {
                    Equipment_statusProcess.Abort();
                    Equipment_statusProcess = null;
                }
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Equipment_status_Main程序关闭", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Equipment_status_Main启动关闭出现异常", ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private static void Equipment_status_Service()
        {
            while (true)
            {
                Thread.Sleep(10000);
                try
                {
                    //时间差表示在线 但是状态确实离线，所以需要进行上线推送
                    string sql = string.Format("select id,equipment_id,working_status from smart_culture_equipment where TIMESTAMPDIFF(SECOND,last_update_time,NOW())<300 and working_status = 0");
                    DataTable dt = BDS_Sensor_PushProcess.dbNetdefault.ExecuteDataTable(sql, null, CommandType.Text);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            //执行上线推送
                            PushAPIProcessOn(dr["equipment_id"].ToString());
                            //更新
                            string sql1 = string.Format("update smart_culture_equipment set working_status=1 where id={0}", dr["id"].ToString());
                            int result = BDS_Sensor_PushProcess.dbNetdefault.ExecuteNonQuery(sql1, null, CommandType.Text);
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("上线且推送", dr["equipment_id"].ToString());
                        }
                    }

                    //时间差表示离线 但是状态确实在线，所以需要进行上线推送
                    sql = string.Format("select id,equipment_id,working_status from smart_culture_equipment where TIMESTAMPDIFF(SECOND,last_update_time,NOW())>300 and  working_status = 1");
                    dt = BDS_Sensor_PushProcess.dbNetdefault.ExecuteDataTable(sql, null, CommandType.Text);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            //执行上线推送
                            PushAPIProcess(dr["equipment_id"].ToString());
                            //更新数据库状态
                            string sql1 = string.Format("update smart_culture_equipment set working_status=0 where id={0}", dr["id"].ToString());
                            int result = BDS_Sensor_PushProcess.dbNetdefault.ExecuteNonQuery(sql1, null, CommandType.Text);
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("离线且推送", dr["equipment_id"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("Equipment_status_Service异常", ex.Message + ex.StackTrace);
                }
                Thread.Sleep(140000);
            }
        }

        public static string PushAPIProcess(string equipment_id)
        {
            //线上局域网IP
            string url = string.Format("http://172.24.108.167:9091/zhyz/api/notice/equipment/outline/alarm?equipmentId={0}", equipment_id);
            string result = HttpProcess.HttpGet(url);
            ToolAPI.XMLOperation.WriteLogXmlNoTail("设备状态接口调用", equipment_id + ";" + result);
            return result;
        }
        public static string PushAPIProcessOn(string equipment_id)
        {
            //线上局域网IP
            string url = string.Format("http://172.24.108.167:9091/zhyz/api/notice/equipment/online/alarm?equipmentId={0}", equipment_id);
           // string url = string.Format("http://wxapi.yeetong.cn/zhyz/api/notice/equipment/online/alarm?equipmentId={0}", equipment_id);
            string result = HttpProcess.HttpGet(url);
            ToolAPI.XMLOperation.WriteLogXmlNoTail("设备上线状态接口调用", equipment_id + ";" + result);
            return result;
        }
    }
}
