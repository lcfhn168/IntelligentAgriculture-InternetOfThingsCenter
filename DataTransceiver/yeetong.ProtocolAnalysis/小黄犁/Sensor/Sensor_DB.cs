using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using yeetong_Architecture;

namespace yeetong_ProtocolAnalysis
{
    public  class Sensor_DB
    {
        #region 访问远端数据库
        static DbHelperSQL dbNetdefault = null;
        static Sensor_DB()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Sensor_DB异常", ex.Message);
            }
        }
        public static DataTable Get_SensorAddr485(string DTUID)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    //找到黄犁眼和小黄犁只能盒子
                    string sql = "SELECT equipment_485_addr FROM smart_culture_equipment where (equipment_type_id='5c2085cb7e29123757fd3fe9' OR equipment_type_id='5c2085cb7e29123757fd3feb') and equipment_dtu_id='" + DTUID + "' GROUP BY equipment_485_addr;";
                    DataTable y = dbNetdefault.ExecuteDataTable(sql, null, CommandType.Text);
                    return y;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Sensor_DB.Get_SensorAddr485异常", ex.Message);
                return null;
            }

        }
        #endregion
    }
}
