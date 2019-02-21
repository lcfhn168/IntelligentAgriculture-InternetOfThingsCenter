using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using yeetong_Architecture;

namespace yeetong_ProtocolAnalysis
{
    public  class Relay_DB
    {
        #region 访问远端数据库
        static DbHelperSQL dbNetdefault = null;
        static Relay_DB()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Relay_DB异常", ex.Message);
            }
        }
        public static DataTable Get_RelayAddr485(string DTUID)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    //找到黄犁手和小黄犁智能盒子
                    string sql = "SELECT equipment_485_addr FROM smart_culture_equipment where (equipment_type_id='5c2085cb7e29123757fd3fea' OR equipment_type_id='5c2085cb7e29123757fd3feb') and equipment_dtu_id='" + DTUID + "' GROUP BY equipment_485_addr;";
                    DataTable y = dbNetdefault.ExecuteDataTable(sql, null, CommandType.Text);
                    return y;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Relay_DB.Get_RelayAddr485异常", ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 得到需要进行操作的继电器
        /// </summary>
        /// <returns></returns>
        public static DataTable Get_RelayUpdate()
        {
            try
            {
                if (dbNetdefault != null)
                {
                    //找到黄犁手和小黄犁智能盒子
                    string sql = "select  e.equipment_dtu_id ,e.equipment_485_addr,hls.`value`,hls.equipment_id from smart_culture_hls_control as hls,smart_culture_equipment as e where stage_flag = 0 and hls.equipment_id = e.equipment_id";
                    DataTable y = dbNetdefault.ExecuteDataTable(sql, null, CommandType.Text);
                    return y;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Relay_DB.Get_RelayUpdate异常", ex.Message);
                return null;
            }

        }
        public static int UpdateRelaySwitch(string equipment_id,int stage_flag)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    string sql = "update smart_culture_hls_control set stage_flag="+stage_flag+ " ,update_at='"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"' where equipment_id = '" + equipment_id + "'";
                    int y = dbNetdefault.ExecuteNonQuery(sql, null, CommandType.Text);
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Relay_DB.UpdateRelaySwitch异常", ex.Message);
                return 0;
            }
        }
        public static int UpdateRelaySwitch(string DTUID, string Addr485,string result)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    //找到黄犁手和小黄犁智能盒子
                    string sql = "update smart_culture_hls_control set stage_flag="+ result + " ,update_at='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where equipment_id = (select equipment_id from smart_culture_equipment where equipment_dtu_id='"+ DTUID + "' and equipment_485_addr='"+ Addr485 + "' LIMIT 1)";
                    int y = dbNetdefault.ExecuteNonQuery(sql, null, CommandType.Text);
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Relay_DB.UpdateRelaySwitch异常", ex.Message);
                return 0;
            }
        }
        #endregion
    }
}
