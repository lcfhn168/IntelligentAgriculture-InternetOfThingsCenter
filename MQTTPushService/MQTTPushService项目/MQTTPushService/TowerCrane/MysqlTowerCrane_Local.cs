using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIXH.DBUtility;
namespace MQTTPushService
{
    public class DBFrame
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
        public string mqtttype { get; set; }
        public string forwardtype { get; set; }
    }
    public class MysqlTowerCrane_Local
    {
        /// <summary>
        /// 得到数据库未处理的设备
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        public static IList<DBFrame> GetTowerCrane(string endNumber)
        {
            try
            {
                string sql = string.Format("Select * from towerCrane where mqtttype=0 and deviceid like '%{0}'", endNumber);
                DataTable result = DBoperateClass.DBoperateObj.ExecuteDataTable(sql, null, CommandType.Text);
                if (result != null && result.Rows.Count>0)
                {
                    IList<DBFrame> forwardconfigResult = Extensions.ToList<DBFrame>(result);
                    return forwardconfigResult;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_Local.GetTowerCrane异常", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 更新数据库标识通过id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int UpdateTowerCranedbtypeByid(string id)
        {
            try
            {
                string sql = string.Format("UPDATE towerCrane set mqtttype=1 where id={0}", id);
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_Local.UpdateTowerCranedbtypeByid异常", ex.Message);
                return 0;
            }
        }
        

    }
}
