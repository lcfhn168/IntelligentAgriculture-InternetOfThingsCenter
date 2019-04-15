using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Text.RegularExpressions;
using SIXH.DBUtility;
using System.Threading;


namespace yeetong_Push
{
    public class BDS_Sensor_PushProcess
    {
        public static DbHelperSQL dbNetdefault = null;
        static BDS_Sensor_PushProcess()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_PushProcess异常", ex.Message);
            }
        }

        public static void BDS_SensorAnalyse(BDS_SensorDBFrame dbf)
        {
            try
            {
                switch (dbf.datatype)
                {
                    case "current":
                        BDS_Sensor_Current cu = Newtonsoft.Json.JsonConvert.DeserializeObject<BDS_Sensor_Current>(dbf.contentjson);
                        PushSensorCurrent(cu); break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_DB.TowerCraneTowerCraneDBFrameAnalyse异常", ex.Message);
            }
        }


        #region 实时数据
        public static void PushSensorCurrent(BDS_Sensor_Current current)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    string sql = string.Format("select al.conf_id,al.alarm_term,al.monitor_alarm_value ,al.operational_character from smart_culture_alarm_conf as al, smart_culture_equipment  as e where al.equipment_id = e.equipment_id and e.equipment_dtu_id = '{0}' and e.equipment_485_addr = '{1}' and al.monitor_state='1'", current.DTUID, current.Addr485);
                    DataTable dt = dbNetdefault.ExecuteDataTable(sql, null, CommandType.Text);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string alarmConfId = dt.Rows[i]["conf_id"].ToString();
                            string monitorTime = current.RecordTime;

                            bool isAlarm = false;
                            string operational = dt.Rows[i]["operational_character"].ToString();
                            double alarm_value = double.Parse(dt.Rows[i]["monitor_alarm_value"].ToString());
                            double value = 0;
                            switch (dt.Rows[i]["alarm_term"].ToString())
                            {
                                case "A": IsAlarm(operational, current.SnsorValue.A, alarm_value); value = current.SnsorValue.A; break;
                                case "B": IsAlarm(operational, current.SnsorValue.B, alarm_value); value = current.SnsorValue.B; break;
                                case "C": IsAlarm(operational, current.SnsorValue.C, alarm_value); value = current.SnsorValue.C; break;
                                case "D": IsAlarm(operational, current.SnsorValue.D, alarm_value); value = current.SnsorValue.D; break;
                                case "E": IsAlarm(operational, current.SnsorValue.E, alarm_value); value = current.SnsorValue.E; break;
                                case "F": IsAlarm(operational, current.SnsorValue.F, alarm_value); value = current.SnsorValue.F; break;
                                case "G": IsAlarm(operational, current.SnsorValue.G, alarm_value); value = current.SnsorValue.G; break;
                                case "H": IsAlarm(operational, current.SnsorValue.H, alarm_value); value = current.SnsorValue.H; break;
                                case "I": IsAlarm(operational, current.SnsorValue.I, alarm_value); value = current.SnsorValue.I; break;
                                case "J": IsAlarm(operational, current.SnsorValue.J, alarm_value); value = current.SnsorValue.J; break;
                                case "K": IsAlarm(operational, current.SnsorValue.K, alarm_value); value = current.SnsorValue.K; break;
                                case "L": IsAlarm(operational, current.SnsorValue.L, alarm_value); value = current.SnsorValue.L; break;
                                case "M": IsAlarm(operational, current.SnsorValue.M, alarm_value); value = current.SnsorValue.M; break;
                                case "N": IsAlarm(operational, current.SnsorValue.N, alarm_value); value = current.SnsorValue.N; break;
                                case "O": IsAlarm(operational, current.SnsorValue.O, alarm_value); value = current.SnsorValue.O; break;
                                case "P": IsAlarm(operational, current.SnsorValue.P, alarm_value); value = current.SnsorValue.P; break;
                                case "Q": IsAlarm(operational, current.SnsorValue.Q, alarm_value); value = current.SnsorValue.Q; break;
                                case "R": IsAlarm(operational, current.SnsorValue.R, alarm_value); value = current.SnsorValue.R; break;
                                case "S": IsAlarm(operational, current.SnsorValue.S, alarm_value); value = current.SnsorValue.S; break;
                                case "T": IsAlarm(operational, current.SnsorValue.T, alarm_value); value = current.SnsorValue.T; break;
                                default: break;
                            }
                            PushAPIProcess(alarmConfId, monitorTime, value.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_DB.SaveSensorCurrent异常", ex.Message);
            }
        }

        public static string PushAPIProcess(string alarmConfId, string monitorTime, string monitorValue)
        {
            // string url = string.Format("http://39.104.20.2:9091/zhyz/api/notice/equipment/alarm?alarmConfId={0}&monitorTime={1}&monitorValue={2}", alarmConfId, monitorTime, monitorValue);
            //线上局域网IP
            string url = string.Format("http://172.24.108.167:9091/zhyz/api/notice/equipment/alarm?alarmConfId={0}&monitorTime={1}&monitorValue={2}", alarmConfId, monitorTime, monitorValue);
            string result = HttpProcess.HttpGet(url);
            ToolAPI.XMLOperation.WriteLogXmlNoTail("推送接口调用", alarmConfId+";"+ monitorValue+";"+ result);
            return result;
        }

        public static bool IsAlarm(string operational,double value,double alarm_value)
        {
            bool isAlarm = false;
            switch (operational)
            {
                //大于
                case "Gt":
                    isAlarm = value > alarm_value;
                    break;
                //小于
                case "Lt":
                    isAlarm = value < alarm_value;
                    break;
                //大于等于
                case "Gte":
                    isAlarm = value >= alarm_value;
                    break;
                //小于等于
                case "Lte":
                    isAlarm = value <= alarm_value;
                    break;
                default: break;
            }
            return isAlarm;
        }
        #endregion
    }
}
