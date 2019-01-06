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
    public class BDS_HumitureAmmonia_DB
    {
        static DbHelperSQL dbNetdefault = null;
        static BDS_HumitureAmmonia_DB()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB异常", ex.Message);
            }
        }

        public static void BDS_HumitureAmmoniaAnalyse(HumitureAndAmmoniaDBFrame dbf)
        {
            try
            {
                switch (dbf.datatype)
                {
                    case "heartbeat":
                        BDS_HumitureAmmonia_Heartbeat hb = Newtonsoft.Json.JsonConvert.DeserializeObject<BDS_HumitureAmmonia_Heartbeat>(dbf.contentjson);
                        SavehumitureammoniaHeartbeat(hb); break;
                    case "current":
                        BDS_HumitureAmmonia_Current cu = Newtonsoft.Json.JsonConvert.DeserializeObject<BDS_HumitureAmmonia_Current>(dbf.contentjson);
                        SavehumitureammoniaCurrent(cu); break;
                    default: break;
                }
                HumitureAndAmmonia_LocalDB.UpdateHumitureAndAmmoniadbtypeByid(dbf.id);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB.TowerCraneTowerCraneDBFrameAnalyse异常", ex.Message);
            }
        }


        #region 实时数据
        public static int SavehumitureammoniaCurrent(BDS_HumitureAmmonia_Current current)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    IList<DbParameter> paraList = new List<DbParameter>();
                    paraList.Add(dbNetdefault.CreateDbParameter("@equipment_dtu_id_temp", current.DTUID));
                    paraList.Add(dbNetdefault.CreateDbParameter("@equipment_485_addr_temp", current.Addr485));
                    paraList.Add(dbNetdefault.CreateDbParameter("@onlineTimes", current.RecordTime));
                    paraList.Add(dbNetdefault.CreateDbParameter("@temperature_temp", current.Temperature));
                    paraList.Add(dbNetdefault.CreateDbParameter("@humidity_temp", current.Humidity));
                    paraList.Add(dbNetdefault.CreateDbParameter("@nh3_temp", current.Ammonia));
                    int y = dbNetdefault.ExecuteNonQuery("humitureammonia_save_current", paraList, CommandType.StoredProcedure);
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB.SavehumitureammoniaCurrent异常", ex.Message);
                return 0;
            }
        }

        #endregion

        #region 心跳
        public static int SavehumitureammoniaHeartbeat(BDS_HumitureAmmonia_Heartbeat o)
        {
            try
            {
                if (dbNetdefault != null)
                {
                    IList<DbParameter> paraList = new List<DbParameter>();
                    paraList.Add(dbNetdefault.CreateDbParameter("@equipment_dtu_id_temp", o.DTUID));
                    paraList.Add(dbNetdefault.CreateDbParameter("@onlineTimes", o.RecordTime));
                    int y = dbNetdefault.ExecuteNonQuery("humitureammonia_save_heartbeat", paraList, CommandType.StoredProcedure);
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_HumitureAmmonia_DB.SavehumitureammoniaHeartbeat异常", ex.Message);
                return 0;
            }

        }
        #endregion

    }
}
