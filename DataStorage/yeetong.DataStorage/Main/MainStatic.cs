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
       
    }
}
