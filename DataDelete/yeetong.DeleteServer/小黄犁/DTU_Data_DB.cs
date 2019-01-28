using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace yeetong_DataDelete
{
    public  class DTU_Data_DB
    {
        public static void DeleteDTU()
        {
            try
            {
                string sql = string.Format("DELETE FROM  dtu_data where dbtype=1 ");
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                sql = string.Format("DELETE FROM  dtu_data where  TIMESTAMPDIFF(SECOND,creattime,now())>60");
                int result1 = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DeleteDTU", result.ToString() + ";" + result1.ToString());
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_DB.DeleteDTU异常", ex.Message);
            }
        }
    }
}
