using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ToolAPI;

namespace GOYO_Architecture
{
    public static class MainStatic
    {
        /// <summary>
        /// 配置文件的路径
        /// </summary>
        public static String Path
        {
            get { return Application.StartupPath + "\\Config.ini"; }
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public static String Port{ get; set;}
        public static String WsPort { get; set; }
      
        static  MainStatic()
        {
            try
            {
                Port = ToolAPI.INIOperate.IniReadValue("goyo", "Port", MainStatic.Path);
                WsPort = ToolAPI.INIOperate.IniReadValue("goyo", "wsPort", MainStatic.Path);
             
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MainStatic构造异常", ex.Message + ex.StackTrace);
                Port = "4999";
                WsPort = "4998";
            }
        }
    }
}
