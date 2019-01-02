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

        /// <summary>
        /// 塔吊和升降机是否需要身份验证
        /// </summary>
        public static String IsAuthentication { get; set;}

        public static string DeviceCopy_TowerCrane { get; set; }//设备拷贝塔吊
        public static string DeviceCopy_Lift { get; set; }//设备拷贝升降机
        public static string DeviceCopy_FogGun { get; set; }//设备拷贝雾炮
        public static string DeviceCopy_DisCharge { get; set; }//设备拷贝卸料
        public static string DeviceCopy_RaiseDustNoise { get; set; }//设备拷贝扬尘噪音
        static  MainStatic()
        {
            try
            {
                Port = ToolAPI.INIOperate.IniReadValue("yeetong", "Port", MainStatic.Path);
                DeviceType = int.Parse(ToolAPI.INIOperate.IniReadValue("yeetong", "DeviceType", MainStatic.Path));
                IsAuthentication = ToolAPI.INIOperate.IniReadValue("yeetong", "IsAuthentication", MainStatic.Path);

                DeviceCopy_TowerCrane = ToolAPI.INIOperate.IniReadValue("DeviceCopy", "TowerCrane", MainStatic.Path);
                DeviceCopy_Lift = ToolAPI.INIOperate.IniReadValue("DeviceCopy", "Lift", MainStatic.Path);
                DeviceCopy_FogGun = ToolAPI.INIOperate.IniReadValue("DeviceCopy", "FogGun", MainStatic.Path);
                DeviceCopy_DisCharge = ToolAPI.INIOperate.IniReadValue("DeviceCopy", "DisCharge", MainStatic.Path);
                DeviceCopy_RaiseDustNoise = ToolAPI.INIOperate.IniReadValue("DeviceCopy", "RaiseDustNoise", MainStatic.Path);
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MainStatic构造异常", ex.Message + ex.StackTrace);
                Port = "5000";
                DeviceType = 0;
                DeviceCopy_TowerCrane = "";
                DeviceCopy_Lift = "";
                DeviceCopy_FogGun = "";
                DeviceCopy_DisCharge = "";
                DeviceCopy_RaiseDustNoise = "";
            }
        }
    }
}
