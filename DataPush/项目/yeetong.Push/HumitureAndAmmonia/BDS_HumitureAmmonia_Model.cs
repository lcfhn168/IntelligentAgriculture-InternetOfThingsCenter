using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yeetong_ProtocolAnalysis
{
    public class BDS_HumitureAmmonia_Current
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
        /// 温度
        /// </summary>
        public double Temperature { get; set; }
        /// <summary>
        /// 湿度
        /// </summary>
        public double Humidity { get; set; }
        /// <summary>
        /// 氨气
        /// </summary>
        public double Ammonia { get; set; }
    }

    public class BDS_HumitureAmmonia_Heartbeat
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
    }
}
