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
using GOYO_Architecture;
using GOYO_ProtocolAnalysis;
using StriveEngine;
using System.Net;
using StriveEngine.Tcp.Server;
using StriveEngine.Core;


namespace GOYO_SpecialEquipmentServer
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
            //解析
            Subject sub = new Subject();
            sub.DataAnalysis += ProtocolAnalysisSE_Main.ProtocolPackageResolver;
            sub.DataAnalysisWS += ProtocolAnalysisSE_Main.ProtocolPackageResolver_Web;
            mc.App_Open(sub);
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
