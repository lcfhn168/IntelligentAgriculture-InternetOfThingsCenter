using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MQTTPushService.tPush
{
    public class VoicePush
    {
        /// <summary>
        /// 访问java接口调用阿里云的语音服务
        /// </summary>
        /// <returns></returns>
        public static bool VoicePushJava(string[] tel,string proname,string sn)
        {
            //本地访问地址：192.168.66.20:8082
            //线上访问地址：https://newadmin.igongdi.cn/goyoAdmin/deviceAlarmVoice/send?phone=
            bool result = false;
            try
            {
                foreach (var i in tel)
                {
                    var request = (HttpWebRequest)WebRequest.Create("https://newadmin.igongdi.cn/goyoAdmin/deviceAlarmVoice/send?phone=" + i + @"&projectName=" + proname + @"&craneNo=" + sn + @"&out=报警&recordTime=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (responseString == "1")
                        result = true;
                }
            }
            catch { }
            return result;
        }
        #region 备用
        public static string GetHttpResponse(string url, int Timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = Timeout;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        #endregion
    }
}
