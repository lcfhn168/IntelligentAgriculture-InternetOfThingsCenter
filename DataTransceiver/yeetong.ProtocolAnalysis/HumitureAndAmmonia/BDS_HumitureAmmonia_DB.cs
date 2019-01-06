using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using yeetong_Architecture;

namespace yeetong_ProtocolAnalysis
{
    public  class BDS_HumitureAmmonia_DB
    {
        #region 存入本地数据库用的
        /// <summary>
        /// 保存温湿度和氨气
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        public static int SaveBDSHumitureAmmonia(DBFrame df)
        {
            try
            {
                string sql = string.Format("INSERT INTO humitureandammonia (deviceid,datatype,contentjson,contenthex,version) VALUES('{0}','{1}','{2}','{3}','{4}')", df.deviceid, df.datatype, df.contentjson, df.contenthex, df.version);
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                //ToolAPI.XMLOperation.WriteLogXmlNoTail("心跳存数据库", result.ToString());
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("SaveBDSHumitureAmmonia", ex.Message);
                return 0;
            }
        }
        #endregion

        #region 访问远端数据库
        static DbHelperSQL dbNetdefault = null;
        static BDS_HumitureAmmonia_DB()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB异常", ex.Message);
            }
        }
        public static DataTable Get_BDS_HumitureammoniaAddr485(string DTUID)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    string sql = "SELECT equipment_485_addr FROM smart_culture_equipment where equipment_dtu_id='"+ DTUID + "' GROUP BY equipment_485_addr;";
                    DataTable y = dbNetdefault.ExecuteDataTable(sql, null, CommandType.Text);
                    return y;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB.Get_BDS_HumitureammoniaAddr485异常", ex.Message);
                return null;
            }

        }
        #endregion
    }
}
