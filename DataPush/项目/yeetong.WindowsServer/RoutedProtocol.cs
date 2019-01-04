using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using yeetong_Push;


namespace yeetong_WindowsServer
{
    partial class RoutedProtocol : ServiceBase
    {
        MainClass mc = new MainClass();
        /// <summary>
        /// 
        /// </summary>
        public RoutedProtocol()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            mc.App_Open();
        }
       
        /// <summary>
        /// 
        /// </summary>
        protected override void OnStop()
        {
            mc.App_Close();
        }
    }
}
