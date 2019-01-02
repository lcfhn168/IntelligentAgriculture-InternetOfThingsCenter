using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using SIXH.DBUtility;
using System.Data;

namespace DBToOssServer
{
   public class CraneDeleteDataTable
    {
       /// <summary>
       /// 删除昨天的数据
       /// </summary>
       public static void Delete(DataTable dt)
       {
           try
           {
               //DataTable dt = DBoperateClass.DBoperateObj.ExecuteDataTable("select sn,etable from t_report where etype=0", null, CommandType.Text);
               for (int i = 0; i < dt.Rows.Count; i++)
               {
                   string delete = "delete from " + dt.Rows[i]["etable"].ToString() + " where equipmentNo='" + dt.Rows[i]["sn"].ToString() + "' and to_days(creat_time) != to_days(now())";
                   DBoperateClass.DBoperateObj.ExecuteNonQuery(delete, null, CommandType.Text);
               }
           }
           catch(Exception ex) {
               ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlCrane_Oss.DeleteDataTable异常", ex.Message);
           }
       }
    }
}
