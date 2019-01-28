using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yeetong_ProtocolAnalysis
{
    public class DTU_Data_Heartbeat
    {
        /// <summary>
        /// 网关编号
        /// </summary>
        public string DTUID { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string RecordTime { get; set; }
    }
}
