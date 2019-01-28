using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace yeetong_DataDelete
{
    public  class HXM_Relay_DB
    {
        public static void DeleteHXMRelay()
        {
            try
            {
                string sql = string.Format("DELETE FROM  hxm_relay where dbtype=1 ");
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                sql = string.Format("DELETE FROM  hxm_relay where  TIMESTAMPDIFF(SECOND,creattime,now())>60");
                int result1 = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DeleteHXMRelay", result.ToString() + ";" + result1.ToString());
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_DB.DeleteHXMRelay异常", ex.Message);
            }
        }
    }
}
