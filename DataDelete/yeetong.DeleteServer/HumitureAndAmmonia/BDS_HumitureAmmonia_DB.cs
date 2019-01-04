using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace yeetong_DataDelete
{
    public  class BDS_HumitureAmmonia_DB
    {
        public static void DeleteBDSHumitureAmmonia()
        {
            try
            {
                string sql = string.Format("DELETE FROM  humitureandammonia where dbtype=1 and pushtype=1");//暂时加上mqtt推送
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                sql = string.Format("DELETE FROM  humitureandammonia where  TIMESTAMPDIFF(SECOND,creattime,now())>60");
                int result1 = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DeleteBDSHumitureAmmonia", result.ToString() + ";" + result1.ToString());
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB.DeleteBDSHumitureAmmonia异常", ex.Message);
            }
        }
    }
}
