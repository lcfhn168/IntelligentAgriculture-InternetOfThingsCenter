using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIXH.DBUtility;
namespace yeetong_DataStorage
{
    public class DTU_DataDBFrame
    {
        public string id { get; set; }
        public string deviceid { get; set; }
        public string datatype { get; set; }
        public string contentjson { get; set; }
        public string contenthex { get; set; }
        public string version { get; set; }
        public string creattime { get; set; }
        public string usetype { get; set; }
        public string dbtype { get; set; }
        public string pushtype { get; set; }
        public string forwardtype { get; set; }
    }
    public class DTU_Data_LocalDB
    {
        /// <summary>
        /// 得到数据库未处理的设备
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        public static IList<DTU_DataDBFrame> GetDTU_Data()
        {
            try
            {
                string sql = string.Format("Select * from dtu_data where dbtype=0 ");
                DataTable result = DBoperateClass.DBoperateObj.ExecuteDataTable(sql, null, CommandType.Text);
                if (result != null && result.Rows.Count>0)
                {
                    IList<DTU_DataDBFrame> forwardconfigResult = Extensions.ToList<DTU_DataDBFrame>(result);
                    return forwardconfigResult;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_LocalDB.GetDTU_Data异常", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 更新数据库标识通过id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int UpdateDTU_DatatypeByid(string id)
        {
            try
            {
                string sql = string.Format("UPDATE dtu_data set dbtype=1 where id={0}", id);
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_LocalDB.UpdateDTU_DatatypeByid异常", ex.Message);
                return 0;
            }
        }

    }
}
