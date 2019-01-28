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
                    CommandIssuedInitEvent += BDS_Sensor_issued.Get_BDS_Sensor;
                    break;
                default: break;
            }
        }
        public static Action<IList<TcpSocketClient>> CommandIssuedInitEvent ;
       
    }
}
