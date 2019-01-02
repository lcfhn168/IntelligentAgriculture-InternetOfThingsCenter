using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOYO_Architecture;
using GOYO_ProtocolAnalysis.TowerCrane._021303;
using GOYO_ProtocolAnalysis.TowerCrane.OE;
using TCPAPI;

namespace GOYO_ProtocolAnalysis
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
