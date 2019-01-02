using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yeetong_ProtocolAnalysis
{
    public  class BDS_HumitureAmmonia_Model
    {
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
}
