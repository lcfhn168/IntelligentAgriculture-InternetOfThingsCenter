using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Text.RegularExpressions;
using SIXH.DBUtility;
using System.Threading;
using Newtonsoft.Json;

namespace yeetong_DataStorage
{
    public class HXM_Relay_DB
    {
        static DbHelperSQL dbNetdefault = null;
        static HXM_Relay_DB()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_DB异常", ex.Message);
            }
        }

        public static void HXM_RelayAnalyse(HXM_RelayDBFrame dbf)
        {
            try
            {
                switch (dbf.datatype)
                {
                    case "RelayStatus":
                        HXM_Relay_Status cu = Newtonsoft.Json.JsonConvert.DeserializeObject<HXM_Relay_Status>(dbf.contentjson);
                        SaveHXM_RelayStatus(cu); break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_DB.HXM_RelayAnalyse异常", ex.Message);
            }
        }


        #region 实时数据
        public static int SaveHXM_RelayStatus(HXM_Relay_Status current)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    //IN `dtu_id` varchar(32) ,IN `addr485` varchar(8) ,IN `recordtime` varchar(24) ,IN `valuejson` text
                    IList<DbParameter> paraList = new List<DbParameter>();
                    paraList.Add(dbNetdefault.CreateDbParameter("@dtu_id", current.DTUID));
                    paraList.Add(dbNetdefault.CreateDbParameter("@addr485", current.Addr485));
                    paraList.Add(dbNetdefault.CreateDbParameter("@recordtime", current.RecordTime));
                    paraList.Add(dbNetdefault.CreateDbParameter("@relaystatus", current.RelayStatus));
                    int y = dbNetdefault.ExecuteNonQuery("hls_current_save", paraList, CommandType.StoredProcedure);
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_DB.SaveHXM_RelayStatus异常", ex.Message);
                return 0;
            }
        }

        #endregion

    }
}
