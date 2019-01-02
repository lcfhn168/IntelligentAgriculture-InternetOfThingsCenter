using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIXH.DBUtility;
namespace DeleteServer
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
        public static void DeleteTowerCrane()
        {
            try
            {
                string sql = string.Format("DELETE FROM  towerCrane where dbtype=1 and mqtttype=1");//给珠海转发的所做的去掉MQTT
                //string sql = string.Format("DELETE FROM  towerCrane where dbtype=1 and mqtttype=1 and forwardtype=1");
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                sql = string.Format("DELETE FROM  towerCrane where  TIMESTAMPDIFF(SECOND,creattime,now())>60");
                int result1 = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DeleteTowerCrane", result.ToString() + ";" + result1.ToString());
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_Local.GetTowerCrane异常", ex.Message);
            }
        }
    }
}
