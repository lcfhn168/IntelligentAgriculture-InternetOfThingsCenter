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
                    CommandIssuedInitEvent += Sensor_issued.Get_BDS_Sensor;
                    CommandIssuedInitEvent += Relay_issued.Get_HXM_Relay;
                    CommandIssuedInitEvent += Relay_issued.Get_HXM_RelayUpdate;
                    break;
                default: break;
            }
        }
        public static Action<IList<TcpSocketClient>> CommandIssuedInitEvent ;
       
    }
}
