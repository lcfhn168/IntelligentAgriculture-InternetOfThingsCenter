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
    public class BDS_HumitureAmmonia_PushProcess
    {
        static DbHelperSQL dbNetdefault = null;
        static BDS_HumitureAmmonia_PushProcess()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_PushProcess异常", ex.Message);
            }
        }

        public static void BDS_HumitureAmmoniaAnalyse(HumitureAndAmmoniaDBFrame dbf)
        {
            try
            {
                switch (dbf.datatype)
                {
                    //case "heartbeat":
                    //BDS_HumitureAmmonia_Heartbeat hb = Newtonsoft.Json.JsonConvert.DeserializeObject<BDS_HumitureAmmonia_Heartbeat>(dbf.contentjson);
                    // SavehumitureammoniaHeartbeat(hb); break;
                    case "current":
                        BDS_HumitureAmmonia_Current cu = Newtonsoft.Json.JsonConvert.DeserializeObject<BDS_HumitureAmmonia_Current>(dbf.contentjson);
                        PushHumitureammoniaCurrent(cu); break;
                    default: break;
                }
                HumitureAndAmmonia_LocalDB.UpdateHumitureAndAmmoniadbtypeByid(dbf.id);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB.TowerCraneTowerCraneDBFrameAnalyse异常", ex.Message);
            }
        }


        #region 实时数据
        public static void PushHumitureammoniaCurrent(BDS_HumitureAmmonia_Current current)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    string sql = string.Format("select smart_culture_alarm_conf.* from smart_culture_alarm_conf  cross join smart_culture_equipment where smart_culture_alarm_conf.equipment_id = smart_culture_equipment.equipment_id and smart_culture_equipment.equipment_dtu_id = '{0}' and smart_culture_equipment.equipment_485_addr = '{1}' and monitor_state='1'", current.DTUID, current.Addr485);
                    DataTable dt = dbNetdefault.ExecuteDataTable(sql, null, CommandType.Text);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string alarmConfId = dt.Rows[i]["conf_id"].ToString();
                            string monitorTime = current.RecordTime;

                            switch (dt.Rows[i]["equipment_type_id"].ToString())
                            {
                                //温度
                                case "5c20b71a7e29f28d5eaf774c":
                                    bool isAlarm = false;
                                    switch (dt.Rows[i]["operational_character"].ToString())
                                    {
                                        //大于
                                        case "Gt":
                                            isAlarm = current.Temperature > double.Parse(dt.Rows[i]["monitor_alarm_value"].ToString());
                                            break;
                                        //小于
                                        case "Lt":
                                            isAlarm = current.Temperature < double.Parse(dt.Rows[i]["monitor_alarm_value"].ToString());
                                            break;
                                        //大于等于
                                        case "Gte":
                                            isAlarm = current.Temperature >= double.Parse(dt.Rows[i]["monitor_alarm_value"].ToString());
                                            break;
                                        //小于等于
                                        case "Lte":
                                            isAlarm = current.Temperature <= double.Parse(dt.Rows[i]["monitor_alarm_value"].ToString());
                                            break;
                                        default: break;
                                    }
                                    if (isAlarm)//有报警
                                    {
                                        IList<DbParameter> paraList = new List<DbParameter>();
                                        paraList.Add(dbNetdefault.CreateDbParameter("@equipment_id_temp", dt.Rows[i]["equipment_id"].ToString()));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@alarm_temp", (double)dt.Rows[i]["monitor_alarm_value"]));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@value_temp", current.Temperature));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@operational_character_temp", dt.Rows[i]["operational_character"].ToString()));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@type", "t"));
                                        int y = dbNetdefault.ExecuteNonQuery("humitureammonia_alarm", paraList, CommandType.StoredProcedure);
                                        PushAPIProcess(alarmConfId, monitorTime, current.Temperature.ToString());
                                    }
                                    break;
                                //湿度
                                case "5c20b71a7e29f28d5eaf774d":
                                    bool isAlarH = false;
                                    switch (dt.Rows[i]["operational_character"].ToString())
                                    {
                                        //大于
                                        case "Gt":
                                            isAlarH = current.Humidity > (double)dt.Rows[i]["monitor_alarm_value"];
                                            break;
                                        //小于
                                        case "Lt":
                                            isAlarH = current.Humidity < (double)dt.Rows[i]["monitor_alarm_value"];
                                            break;
                                        //大于等于
                                        case "Gte":
                                            isAlarH = current.Humidity >= (double)dt.Rows[i]["monitor_alarm_value"];
                                            break;
                                        //小于等于
                                        case "Lte":
                                            isAlarH = current.Humidity <= (double)dt.Rows[i]["monitor_alarm_value"];
                                            break;
                                        default: break;
                                    }
                                    if (isAlarH)//有报警
                                    {
                                        IList<DbParameter> paraList = new List<DbParameter>();
                                        paraList.Add(dbNetdefault.CreateDbParameter("@equipment_id_temp", dt.Rows[i]["equipment_id"].ToString()));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@alarm_temp", (double)dt.Rows[i]["monitor_alarm_value"]));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@value_temp", current.Humidity));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@operational_character_temp", dt.Rows[i]["operational_character"].ToString()));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@type", "h"));
                                        int y = dbNetdefault.ExecuteNonQuery("humitureammonia_alarm", paraList, CommandType.StoredProcedure);

                                        PushAPIProcess(alarmConfId, monitorTime, current.Humidity.ToString());
                                    }
                                    break;
                                //氨气
                                case "5c2085cb7e29123757fd3fe8":
                                    bool isAlarA = false;
                                    switch (dt.Rows[i]["operational_character"].ToString())
                                    {
                                        //大于
                                        case "Gt":
                                            isAlarA = current.Ammonia > (double)dt.Rows[i]["monitor_alarm_value"];
                                            break;
                                        //小于
                                        case "Lt":
                                            isAlarA = current.Ammonia < (double)dt.Rows[i]["monitor_alarm_value"];
                                            break;
                                        //大于等于
                                        case "Gte":
                                            isAlarA = current.Ammonia >= (double)dt.Rows[i]["monitor_alarm_value"];
                                            break;
                                        //小于等于
                                        case "Lte":
                                            isAlarA = current.Ammonia <= (double)dt.Rows[i]["monitor_alarm_value"];
                                            break;
                                        default: break;
                                    }
                                    if(isAlarA)
                                    {
                                        IList<DbParameter> paraList = new List<DbParameter>();
                                        paraList.Add(dbNetdefault.CreateDbParameter("@equipment_id_temp", dt.Rows[i]["equipment_id"].ToString()));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@alarm_temp", (double)dt.Rows[i]["monitor_alarm_value"]));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@value_temp", current.Ammonia));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@operational_character_temp", dt.Rows[i]["operational_character"].ToString()));
                                        paraList.Add(dbNetdefault.CreateDbParameter("@type", "a"));
                                        int y = dbNetdefault.ExecuteNonQuery("humitureammonia_alarm", paraList, CommandType.StoredProcedure);

                                        PushAPIProcess(alarmConfId, monitorTime, current.Ammonia.ToString());
                                    }
                                    break;
                                default: break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB.SavehumitureammoniaCurrent异常", ex.Message);
            }
        }

        public static string PushAPIProcess(string alarmConfId, string monitorTime, string monitorValue)
        {
            // string url = string.Format("http://39.104.20.2:9091/zhyz/api/notice/equipment/alarm?alarmConfId={0}&monitorTime={1}&monitorValue={2}", alarmConfId, monitorTime, monitorValue);
            //线上局域网IP
            string url = string.Format("http://172.24.108.167:9091/zhyz/api/notice/equipment/alarm?alarmConfId={0}&monitorTime={1}&monitorValue={2}", alarmConfId, monitorTime, monitorValue);
            string result = HttpProcess.HttpGet(url);
            return result;
        }

        #endregion

        #region 心跳
        public static int SavehumitureammoniaHeartbeat(BDS_HumitureAmmonia_Heartbeat o)
        {
            try
            {
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB.SavehumitureammoniaHeartbeat异常", ex.Message);
                return 0;
            }

        }
        #endregion

    }
}
