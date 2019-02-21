using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yeetong_DataStorage
{
    [Serializable]
    public class BDS_Sensor_Current
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

        public SensorValue SnsorValue { get; set; }

        public class SensorValue
        {
            /// <summary>
            /// 温度
            /// </summary>
            public double A { get; set; }
            /// <summary>
            /// 湿度
            /// </summary>
            public double B { get; set; }
            /// <summary>
            /// 光照
            /// </summary>
            public double C { get; set; }
            /// <summary>
            /// 紫外线
            /// </summary>
            public double D { get; set; }
            /// <summary>
            /// 大气压
            /// </summary>
            public double E { get; set; }
            /// <summary>
            /// 声音
            /// </summary>
            public double F { get; set; }
            /// <summary>
            /// PM1.0
            /// </summary>
            public double G { get; set; }
            /// <summary>
            /// PM2.5
            /// </summary>
            public double H { get; set; }
            /// <summary>
            /// PM10
            /// </summary>
            public double I { get; set; }
            /// <summary>
            /// 电压值
            /// </summary>
            public double J { get; set; }
            /// <summary>
            /// 烟雾
            /// </summary>
            public double K { get; set; }
            /// <summary>
            /// 酒精
            /// </summary>
            public double L { get; set; }
            /// <summary>
            /// 甲烷
            /// </summary>
            public double M { get; set; }
            /// <summary>
            /// 丙烷
            /// </summary>
            public double N { get; set; }
            /// <summary>
            /// 氢气
            /// </summary>
            public double O { get; set; }
            /// <summary>
            /// NH3
            /// </summary>
            public double P { get; set; }
            /// <summary>
            /// 甲苯
            /// </summary>
            public double Q { get; set; }
            /// <summary>
            /// 一氧化碳
            /// </summary>
            public double R { get; set; }
            /// <summary>
            /// 二氧化碳
            /// </summary>
            public double S { get; set; }
            /// <summary>
            /// 氧气
            /// </summary>
            public double T { get; set; }
        }

    }
}
