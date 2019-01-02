using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using yeetong_Architecture;
using yeetong_ProtocolAnalysis.TowerCrane._021303;
using yeetong_ProtocolAnalysis.TowerCrane.OE;
using TCPAPI;

namespace yeetong_ProtocolAnalysis
{
    public class CommandIssued_Main
    {
        public static void CommandIssued_MainInit()
        {
            switch (MainStatic.DeviceType)
            {
                //塔吊
                case 0:
                    CommandIssuedInitEvent += CommandIssued_TC0E.Crane_SetIPConfig;
                    CommandIssuedInitEvent += CommandIssued_TC0E.Crane_SetControl;
                    CommandIssuedInitEvent += CommandIssued_021303.SetIPConfig;
                    CommandIssuedInitEvent += CommandIssued_021303.CommandIssued;
                    break;
                default: break;
            }
        }
        public static Action<IList<TcpSocketClient>> CommandIssuedInitEvent ;
       
    }
}
