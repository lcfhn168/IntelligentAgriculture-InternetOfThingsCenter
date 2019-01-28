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
    public class HXM_RelayDBFrame
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
    public class HXM_Relay_LocalDB
    {
        /// <summary>
        /// 得到数据库未处理的设备
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        public static IList<HXM_RelayDBFrame> GetHXM_Relay()
        {
            try
            {
                string sql = string.Format("Select * from hxm_relay where dbtype=0 ");
                DataTable result = DBoperateClass.DBoperateObj.ExecuteDataTable(sql, null, CommandType.Text);
                if (result != null && result.Rows.Count>0)
                {
                    IList<HXM_RelayDBFrame> forwardconfigResult = Extensions.ToList<HXM_RelayDBFrame>(result);
                    return forwardconfigResult;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_LocalDB.GetHXM_Relay异常", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 更新数据库标识通过id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int UpdateHXM_RelaytypeByid(string id)
        {
            try
            {
                string sql = string.Format("UPDATE hxm_relay set dbtype=1 where id={0}", id);
                int result = DBoperateClass.DBoperateObj.ExecuteNonQuery(sql, null, CommandType.Text);
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_LocalDB.UpdateHXM_RelaytypeByid异常", ex.Message);
                return 0;
            }
        }

    }
}
