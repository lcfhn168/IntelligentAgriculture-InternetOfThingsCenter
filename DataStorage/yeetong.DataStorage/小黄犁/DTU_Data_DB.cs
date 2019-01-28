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
    public class DTU_Data_DB
    {
        static DbHelperSQL dbNetdefault = null;
        static DTU_Data_DB()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("BDS_Sensor_DB异常", ex.Message);
            }
        }

        public static void DTU_Data_Analyse(DTU_DataDBFrame dbf)
        {
            try
            {
                switch (dbf.datatype)
                {
                    case "heartbeat":
                        DTU_Data_Heartbeat hb = Newtonsoft.Json.JsonConvert.DeserializeObject<DTU_Data_Heartbeat>(dbf.contentjson);
                        SaveHeartbeat(hb); break;
                    default: break;
                }
                DTU_Data_LocalDB.UpdateDTU_DatatypeByid(dbf.id);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_DB.DTU_Data_Analyse异常", ex.Message);
            }
        }

        #region 心跳
        public static int SaveHeartbeat(DTU_Data_Heartbeat o)
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
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DTU_Data_DB.SaveHeartbeat异常", ex.Message);
                return 0;
            }

        }
        #endregion

    }
}
