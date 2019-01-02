using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ToolAPI;
namespace yeetong_Architecture
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
        /// <summary>
        /// 设备类型
        /// </summary>
        public static int DeviceType{get;set; }

        public static string DeviceCopy_BDS { get; set; }//设备拷贝毕达斯
       
        static  MainStatic()
        {
            try
            {
                Port = ToolAPI.INIOperate.IniReadValue("yeetong", "Port", MainStatic.Path);
                DeviceType = int.Parse(ToolAPI.INIOperate.IniReadValue("yeetong", "DeviceType", MainStatic.Path));
                DeviceCopy_BDS = ToolAPI.INIOperate.IniReadValue("DeviceCopy", "DeviceCopy_BDS", MainStatic.Path);
               
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MainStatic构造异常", ex.Message + ex.StackTrace);
                Port = "5000";
                DeviceType = 0;
                DeviceCopy_BDS = "";
            }
        }
    }
}
