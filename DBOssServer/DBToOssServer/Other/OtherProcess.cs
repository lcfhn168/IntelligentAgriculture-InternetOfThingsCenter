using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DBToOssServer
{
    public class OtherProcess
    {
        static bool IsValue = true;
        public static void DeleteHis_all_equipment_all_alarm()
        {

            DateTime now = System.DateTime.Now;
            string hour = now.Hour.ToString("00");
            string min = now.Minute.ToString("00");
            string time = hour + min;
            if (time.Equals("0130"))
            {
                if (IsValue)
                {
                    try
                    {
                        IsValue = false;
                        string ttemp = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                        DateTime dt = DateTime.Parse(ttemp);
                        long tt = (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        string sql = string.Format("delete from all_equipment_all_alarm where alarmTime < {0}", tt);
                        DbHelperSQL DbNet = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", "192.168.1.59", "3306", "wisdomsite", "root", "j@DFvPu66#rxcbd2"), DbProviderType.MySql);
                        int result = DbNet.ExecuteNonQuery(sql, null, CommandType.Text);
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("DeleteHis_all_equipment_all_alarm执行", result.ToString());
                    }
                    catch (Exception ex)
                    {
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("DeleteHis_all_equipment_all_alarm异常", ex.Message);
                    }
                }
            }
            else
            {
                IsValue = true;
            }
        }
    }
}
