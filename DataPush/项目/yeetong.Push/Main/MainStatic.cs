using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ToolAPI;

namespace yeetong_Push
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
        /// 设备类型
        /// </summary>
        public static int DeviceType
        {
            get;
            set;
        }
        /// <summary>
        /// tcp服务器地址
        /// </summary>
        public static string putServer
        {
            get;
            set;
        }
        /// <summary>
        /// tcp服务器端口
        /// </summary>
        public static string putPort
        {
            get;
            set;
        }
        /// <summary>
        /// 推送类型
        /// </summary>
        public static string putType
        {
            get;
            set;
        }
        /// <summary>
        /// 用于安全帽定位
        /// </summary>
        public static int Timeinterval { get; set; }
        static  MainStatic()
        {
            try
            {
                Timeinterval = int.Parse(ToolAPI.INIOperate.IniReadValue("softOffLine", "time", MainStatic.Path));
                DeviceType = int.Parse(ToolAPI.INIOperate.IniReadValue("yeetong", "DeviceType", MainStatic.Path));
                putServer = ToolAPI.INIOperate.IniReadValue("yeetong", "putServer", MainStatic.Path);
                putPort = ToolAPI.INIOperate.IniReadValue("yeetong", "putPort", MainStatic.Path);
                putType = ToolAPI.INIOperate.IniReadValue("yeetong", "putType", MainStatic.Path);
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MainStatic构造异常", ex.Message + ex.StackTrace);
                DeviceType = 0;
                Timeinterval = 10;
            }
        }
    }
}
