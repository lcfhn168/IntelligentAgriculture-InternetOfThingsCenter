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
            //ToolAPI.XMLOperation.WriteLogXmlNoTail("c", "");
            switch (MainStatic.DeviceType)
            {
                case 0:
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("d", "");
                    CommandIssuedInitEvent += BDS_HumitureAmmonia_issued.Get_BDS_HumitureAmmonia;
                    break;
                default: break;
            }
        }
        public static Action<IList<TcpSocketClient>> CommandIssuedInitEvent ;
       
    }
}
