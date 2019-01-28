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
                        HXM_RelayDBFrame cu = Newtonsoft.Json.JsonConvert.DeserializeObject<HXM_RelayDBFrame>(dbf.contentjson);
                        SaveHXM_RelayStatus(cu); break;
                    default: break;
                }
                BDS_Sensor_LocalDB.UpdateSensordbtypeByid(dbf.id);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("HXM_Relay_DB.HXM_RelayAnalyse异常", ex.Message);
            }
        }


        #region 实时数据
        public static int SaveHXM_RelayStatus(HXM_RelayDBFrame current)
        {
            try
            {
                if (dbNetdefault != null)
                {
                   
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
