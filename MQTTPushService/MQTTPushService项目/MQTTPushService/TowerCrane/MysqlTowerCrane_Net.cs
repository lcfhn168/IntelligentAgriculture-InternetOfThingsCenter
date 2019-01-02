using SIXH.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace MQTTPushService
{
    public class MysqlTowerCrane_Net
    {
        static DbHelperSQL DbNet = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", "192.168.1.59", "3306", "wisdomsite", "root", "j@DFvPu66#rxcbd2"), DbProviderType.MySql);
        /// <summary>
        /// 查看该设备的报警近10分钟有没有报警
        /// </summary>
        /// <param name="craneNo"></param>
        /// <returns></returns>
        public static int IsTenMinPush(string craneNo, int type)
        {
            int count = 0;
            try
            {
                IList<DbParameter> paraList = new List<DbParameter>();
                paraList.Add(DbNet.CreateDbParameter("@p_sn", craneNo));
                paraList.Add(DbNet.CreateDbParameter("@p_type", type));
                DataTable dt = DbNet.ExecuteDataTable("pro_PushAlarm", paraList, CommandType.StoredProcedure);
                //DataTable dt = DbNet.ExecuteDataTable(sql, null, CommandType.Text);
                count = int.Parse(dt.Rows[0][0].ToString());
            }
            catch { }
            return count;
        }
        /// <summary>
        /// 获取推送类型
        /// </summary>
        /// <param name="craneNo"></param>
        /// <returns></returns>
        public static DataTable GetPushType(string craneNo, int type)
        {
            try
            {
                string sql = @"select uuid,type,pushType,(select name from unit_project where uuid=equipment_device_alarm_push_settings.proUuid) proname,proUuid   from equipment_device_alarm_push_settings where proUuid=
(select proUuid from equipment_basics where equipmentNo='" + craneNo + "' and type=" + type + " LIMIT 1) and state=1 LIMIT 1";
                return DbNet.ExecuteDataTable(sql, null, CommandType.Text);
            }
            catch { }
            return null;
        }

        public static DataTable GetPushTypeNew(string craneNo, int type)
        {
            try
            {
                string sql = @"SELECT l.name,l.phone,s.pushType,s.type,s.proUuid,i.uuid,(select name from unit_project where uuid=s.proUuid) proname FROM user_loginaccount l
LEFT JOIN user_identity i ON(l.uuid = i.userUuid)
LEFT JOIN equipment_device_alarm_push_user u ON(u.identityUuid = i.uuid)
LEFT JOIN equipment_device_alarm_push_settings s ON(s.uuid = u.pushSetUuid)
LEFT JOIN equipment_basics b ON(b.proUuid = s.proUuid AND s.equipmentType = b.type)
WHERE b.equipmentNo = '" + craneNo + "' AND i.state = 0 AND l.state = 0 AND b.state = 0 and s.state=1 GROUP BY l.phone";
                return DbNet.ExecuteDataTable(sql, null, CommandType.Text);
            }
            catch { }
            return null;
        }
        /// <summary>
        /// 获取下推送的人员手机号
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static string[] GetPushPerson(string uuid)
        {
            string[] result = null;
            try
            {
                string sql = "select phone from equipment_device_alarm_push_user where pushSetUuid='" + uuid + "'";
                DataTable dt = DbNet.ExecuteDataTable(sql, null, CommandType.Text);
                result = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                    result[i] = dt.Rows[i][0].ToString();
            }
            catch { }
            return result;
        }
        /// <summary>
        /// 推送短信
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static string GetPushPersonMsg(string uuid)
        {
            string result = "";
            try
            {
                string sql = "select phone from equipment_device_alarm_push_user where pushSetUuid='" + uuid + "'";
                DataTable dt = DbNet.ExecuteDataTable(sql, null, CommandType.Text);
                for (int i = 0; i < dt.Rows.Count; i++)
                    result += dt.Rows[i]["phone"].ToString() + ",";
            }
            catch { }
            return result.Trim(',');
        }
        /// <summary>
        /// 保存推送记录
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="type"></param>
        /// <param name="useruuid"></param>
        /// <param name="alarmtime"></param>
        /// <returns></returns>
        public static int SavePushrecoder(string sn, string type, string useruuid, string alarmtime) //
        {
            IList<DbParameter> paraList = new List<DbParameter>();
            paraList.Add(DbNet.CreateDbParameter("@p_sn", sn));
            paraList.Add(DbNet.CreateDbParameter("@p_type", type));
            paraList.Add(DbNet.CreateDbParameter("@p_useruuid", useruuid));
            paraList.Add(DbNet.CreateDbParameter("@p_alarmTime", alarmtime));
            return DbNet.ExecuteNonQuery("pro_PushRecoder", paraList, CommandType.StoredProcedure);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static string GetStrongName(string sn)
        {
            string result = "";
            try
            {
                string sql = "select name from equipment_basics where equipmentNo='" + sn + "'";
                DataTable dt = DbNet.ExecuteDataTable(sql, null, CommandType.Text);
                if (dt.Rows.Count > 0)
                    result = dt.Rows[0]["name"].ToString();
            }
            catch { }
            return result;
        }

    }
}
