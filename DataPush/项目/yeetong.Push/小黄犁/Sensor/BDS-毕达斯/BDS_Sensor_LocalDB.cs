using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIXH.DBUtility;
namespace yeetong_Push
{
    public class BDS_SensorDBFrame
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
    public class BDS_Sensor_LocalDB
    {
        /// <summary>
        /// 得到数据库未处理的设备
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        public static IList<BDS_SensorDBFrame> GetSensor()
        {
            try
            {
                string sql = string.Format("Select * from bds_sensor where pushtype=0 ");
                DataTable result = DBoperateClass.DBoperateObj.ExecuteDataTable(sql, null, CommandType.Text);
                if (result != null && result.Rows.Count>0)
                {
                    IList<BDS_SensorDBFrame> forwardconfigResult = Extensions.ToList<BDS_SensorDBFrame>(result);
                    return forwardconfigResult;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_LocalDB.GetSensor异常", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 更新数据库标识通过id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int UpdateSensordbtypeByid(string id)
        {
            try
            {
                string sql = string.Format("UPDATE bds_sensor set pushtype=1 where id={0}", id);
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_LocalDB.UpdateSensordbtypeByid异常", ex.Message);
                return 0;
            }
        }

    }
}
