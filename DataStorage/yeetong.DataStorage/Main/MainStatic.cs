using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ToolAPI;

namespace yeetong_DataStorage
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
        static  MainStatic()
        {
            try
            {
                
                DeviceType = int.Parse(ToolAPI.INIOperate.IniReadValue("yeetong", "DeviceType", MainStatic.Path));
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MainStatic构造异常", ex.Message + ex.StackTrace);
                DeviceType = 0;
            }
        }
    }
}
