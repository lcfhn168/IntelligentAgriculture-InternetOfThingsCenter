using Aliyun.OSS;
using Aliyun.OSS.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBToOssServer
{
    public class CraneUploadOss
    {
        /// <summary>
        /// 上传到OSS
        /// </summary>
        /// <param name="txtName"></param>
        /// <param name="path"></param>
        static void UploadTxtToOss(string txtName, string path)
        {
            try
            {
                string date = "eCrane/" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd").ToString() + "/" + txtName;
                string key = date;
                var conf = new ClientConfiguration();
                conf.IsCname = true;
                var client = new OssClient("http://static.igongdi.cn", "LTAIGkF4EFD33JS0", "kJLAMchcmr6XRoFUlCKPrzdyO1MERI", conf);
                var result = client.PutObject("igd-app", key, path);
               // string msg = ((Aliyun.OSS.Model.GenericResult)(result)).HttpStatusCode.ToString();
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlCrane_Oss.UploadOss", "上传OSS成功,文件：" + txtName);
            }
            catch //第一次不成功，再来一次。
            {
                try
                {
                    string date = "eCrane/" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd").ToString() + "/" + txtName;
                    string key = date;
                    var conf = new ClientConfiguration();
                    conf.IsCname = true;
                    var client = new OssClient("http://static.igongdi.cn", "LTAIGkF4EFD33JS0", "kJLAMchcmr6XRoFUlCKPrzdyO1MERI", conf);
                    var result = client.PutObject("igd-app", key, path);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlCrane_Oss.UploadOss", "上传OSS成功,文件：" + txtName);
                }
                catch (Exception ex){ ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlCrane_Oss.UploadOss上传异常", ex.Message);}
            }
        }
        /// <summary>
        /// 查出生成的txt文档
        /// </summary>
        public static void FindTxt()
        {
            string root = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = root.Remove(root.LastIndexOf('\\') + 1) + "Cranetxt\\";
            DirectoryInfo folder = new DirectoryInfo(path);
            foreach (FileInfo file in folder.GetFiles("*.txt"))
            {
                UploadTxtToOss(file.Name, file.FullName);
            }
        }

    }
}
