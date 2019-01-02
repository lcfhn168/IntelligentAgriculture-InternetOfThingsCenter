using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SIXH.DBUtility;
using System.Threading;
using System.IO;

namespace DBToOssServer
{
    public class MysqlCrane_Oss
    {
        public static void Crane_Oss()
        {
            DateTime now = System.DateTime.Now;
            string hour = now.Hour.ToString("00");
            string min = now.Minute.ToString("00");
            string sen = now.Second.ToString("00");
            string time = hour + min + sen;
            if (time.Equals("000001"))
            {
                try
                {
                    string sql = "select sn,etable from t_report where etype=0";
                    DataTable dt = DBoperateClass.DBoperateObj.ExecuteDataTable(sql, null, CommandType.Text);

                    if (dt.Rows.Count > 0)
                    {
                        string del = "delete from t_report where etype=0 and to_days(createDate) != to_days(now())";
                        DBoperateClass.DBoperateObj.ExecuteNonQuery(del, null, CommandType.Text);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string sn = dt.Rows[i]["sn"].ToString();
                            string et = dt.Rows[i]["etable"].ToString();
                            if (!string.IsNullOrEmpty(sn) && !string.IsNullOrEmpty(et))
                                ReadCraneNoData(sn, et);
                        }
                        CraneUploadOss.FindTxt(); //上传OSS
                        new Action(CraneDeleteFile.DeleteFiles).BeginInvoke(null, null); //删除临时文件
                        CraneDeleteDataTable.Delete(dt); //删除昨天的历史数据
                        dt.Clear();
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlCrane_Oss.Crane_Oss异常", ex.Message);
                }
            }
        }
        #region 读取对应设备号的该设备下的所有历史记录
        /// <summary>
        /// 读取对应设备号的该设备下的所有历史记录
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="etable"></param>
        static void ReadCraneNoData(string sn, string etable)
        {
            string sql = "select * from " + etable + " where equipmentNo='" + sn + "'";
            WriterTxt(DBoperateClass.DBoperateObj.ExecuteDataTable(sql, null, CommandType.Text), sn);
        }
        #endregion
        #region 写入txt文件
        /// <summary>
        /// 写入txt文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sn"></param>
        static void WriterTxt(DataTable dt, string sn)
        {
            string root = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = root.Remove(root.LastIndexOf('\\') + 1) + "Cranetxt\\";
            StringBuilder builder = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                builder.AppendLine(dr[1].ToString() + '|' + dr[2].ToString() + '|' + dr[3].ToString() + '|' + dr[4].ToString() + '|' + dr[5].ToString()
                    + '|' + dr[6].ToString() + '|' + dr[7].ToString() + '|' + dr[8].ToString() + '|' + dr[9].ToString() + '|' + dr[10].ToString()
                    + '|' + dr[11].ToString() + '|' + dr[12].ToString() + '|' + dr[13].ToString() + '|' + dr[14].ToString() + '|' + dr[15].ToString()
                    + '|' + dr[16].ToString() + '|' + dr[17].ToString() + '|' + dr[18].ToString() + '|' + dr[19].ToString() + '|' + dr[20].ToString() + '|'
                    + dr[21].ToString() + '|' + DateTime.Parse(dr[22].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists)//不存在
            {
                directory.Create();
            }
            using (System.IO.FileStream file = new System.IO.FileStream(path + sn + ".txt", System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                using (System.IO.TextWriter text = new System.IO.StreamWriter(file, System.Text.Encoding.Default))
                {
                    text.Write(builder.ToString());
                }
            }
        }
        #endregion
    }
}
