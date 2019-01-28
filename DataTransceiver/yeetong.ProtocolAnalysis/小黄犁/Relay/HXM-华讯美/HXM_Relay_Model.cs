using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yeetong_ProtocolAnalysis
{
    public class HXM_Relay_Status
    {
        /// <summary>
        /// 网关编号
        /// </summary>
        public string DTUID { get; set; }
        /// <summary>
        /// 485地址 
        /// </summary>
        public string Addr485 { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string RecordTime { get; set; }
        /// <summary>
        /// 状态 1是开启 0是关闭
        /// </summary>
        public string RelayStatus { get; set; }
    }
}
