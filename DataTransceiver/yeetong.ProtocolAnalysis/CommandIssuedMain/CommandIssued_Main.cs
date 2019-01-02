using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using yeetong_Architecture;
using TCPAPI;

namespace yeetong_ProtocolAnalysis
{
    public class CommandIssued_Main
    {
        public static void CommandIssued_MainInit()
        {
            switch (MainStatic.DeviceType)
            {
                case 0:
                    //CommandIssuedInitEvent += CommandIssued_TC0E.Crane_SetIPConfig;
                    break;
                default: break;
            }
        }
        public static Action<IList<TcpSocketClient>> CommandIssuedInitEvent ;
       
    }
}
