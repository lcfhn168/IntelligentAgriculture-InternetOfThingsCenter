using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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
                string sql = string.Format("INSERT INTO towerCrane (deviceid,datatype,contentjson,contenthex,version) VALUES('{0}','{1}','{2}','{3}','{4}')", df.deviceid, df.datatype, df.contentjson, df.contenthex, df.version);
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("SaveBDSHumitureAmmonia", ex.Message);
                return 0;
            }
        }
        #endregion
    }
}
