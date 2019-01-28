using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using yeetong_Architecture;

namespace yeetong_ProtocolAnalysis
{
    public  class HXM_Relay_DB
    {
        #region 存入本地数据库用的
        /// <summary>
        /// 保存毕达斯对应的传感器的数据
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        public static int Save_HXM_Relay_status(DBFrame df)
        {
            try
            {
                string sql = string.Format("INSERT INTO hxm_relay (deviceid,datatype,contentjson,contenthex,version) VALUES('{0}','{1}','{2}','{3}','{4}')", df.deviceid, df.datatype, df.contentjson, df.contenthex, df.version);
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Save_HXM_Relay_status", ex.Message);
                return 0;
            }
        }
        #endregion

        #region 访问远端数据库
        static DbHelperSQL dbNetdefault = null;
        static HXM_Relay_DB()
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
        #endregion
    }
}
