﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOYO_Architecture;
using TCPAPI;
using ToolAPI;

namespace GOYO_ProtocolAnalysis
{
    public class ProtocolAnalysisSE_MainUdp
    {
        public delegate string OnResoleRecvMessageUdpdelegate(byte[] b, int c, UdpState udp); //用于udp接收

        public static void ProtocolPackageUdpResolver(byte[] b, UdpState udp)
        {
            //string log= ConvertData.ToHexString(b, 0, b.Length);
           // ToolAPI.XMLOperation.WriteLogXmlNoTail(System.Windows.Forms.Application.StartupPath + @"\OriginalPackage", "", log);
            //烟感及强电
            switch (MainStatic.DeviceType)
            {
                //case 9: ProtocolPackageResolver_Smoke(b, udp); break;
                //case 11: ProtocolPackageResolver_StrongEMonitor(b, udp); break;
                default: break;
            }
        }
       
    }
}
