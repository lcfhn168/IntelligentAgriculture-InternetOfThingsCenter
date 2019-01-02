using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace MQTTPushService.TowerCrane
{
    public class HTTp
    {
        static string httpURL = "";
        static HTTp()
        {
            try
            {
                httpURL = ToolAPI.INIOperate.IniReadValue("towerCrane", "url", MainStatic.Path);
                ToolAPI.XMLOperation.WriteLogXmlNoTail("塔吊HTTp接口地址", httpURL);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("塔吊HTTp异常", ex.Message);
            }
        }

        public static Action<string, string> CollectJsonToPost_Event = CollectJsonToPost;
        /// <summary>
        /// 发送post
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="url"></param>
        public static void CollectJsonToPost(string e, string resultstr)
        {
            try
            {
                try
                {
                    if (httpURL != "")
                    {
                        System.Collections.Specialized.NameValueCollection para = new System.Collections.Specialized.NameValueCollection();
                        para.Add("craneo", e);
                        para.Add("remindcon", resultstr);
                        string url = httpURL;
                        System.Net.WebClient WebClientObj = new System.Net.WebClient();
                        byte[] byRemoteInfo = WebClientObj.UploadValues(url, "POST", para);//请求地址,传参方式,参数集合
                        string rtContent = System.Text.Encoding.UTF8.GetString(byRemoteInfo);//获取返回值 
                        //ToolAPI.XMLOperation.WriteLogXmlNoTail("塔吊报警", e + ";" + resultstr);
                    }

                    #region　http无返回
                    //HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(httpURL);
                    //webrequest.Method = "post";
                    //byte[] postdatabyte = Encoding.UTF8.GetBytes(resultstr);
                    //webrequest.ContentLength = postdatabyte.Length;
                    //Stream stream;
                    //stream = webrequest.GetRequestStream();
                    //stream.Write(postdatabyte, 0, postdatabyte.Length);
                    //stream.Close();
                    //result = true;
                    #endregion
                    #region http有返回
                    //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(httpURL);
                    //request.Method = "POST";
                    //request.ContentType = "application/x-www-form-urlencoded";
                    //request.ContentLength = resultstr.Length;
                    //StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                    //writer.Write(resultstr);
                    //writer.Flush();
                    //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //string encoding = response.ContentEncoding;
                    //if (encoding == null || encoding.Length < 1)
                    //{
                    //    encoding = "UTF-8"; //默认编码  
                    //}
                    //StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                    //string retString = reader.ReadToEnd();
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail(System.Windows.Forms.Application.StartupPath + @"\" + e.AgreementName, "HTTP数据发送", "地址：" + e.Server + ":" + e.Port + "; 结果：" + retString.ToString() + "; 数据：" + resultstr);
                    #endregion
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("塔吊HTTPSysc异常", e + ";" + resultstr + ";" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("塔吊CollectJsonToPost_Sysc异常", ex.Message);
                }
                catch (Exception) { }
            }
        }
    }
}
