using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace yeetong_DataDelete
{
    public  class BDS_Sensor_DB
    {
        public static void DeleteBDSSensor()
        {
            try
            {
                string sql = string.Format("DELETE FROM  bds_sensor where dbtype=1 and pushtype=1 ");
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                sql = string.Format("DELETE FROM  bds_sensor where  TIMESTAMPDIFF(SECOND,creattime,now())>60");
                int result1 = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DeleteBDSsensor", result.ToString() + ";" + result1.ToString());
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_DB.DeleteBDSSensor异常", ex.Message);
            }
        }
    }
}
