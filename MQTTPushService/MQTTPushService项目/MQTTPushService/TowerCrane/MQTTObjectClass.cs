using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MQTTPushService.TowerCrane
{
    /// <summary>
    /// MQTT分支类
    /// </summary>
    public class MQTTFrame
    {
        public string DataType { get; set; }
        public string EquipmentID { get; set; }
        public MQTT_heartbeat mqh { get; set; }
        public MQTT_current mqc { get; set; }
    }
   

    /// <summary>
    /// MQTT心跳类
    /// {"sn":"610065",""rtimes":"2017-04-13"}
    /// </summary>
    public class MQTT_heartbeat
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sn { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string rtimes { get; set; }

    }

    /// <summary>
    /// MQTT实时数据类
    /// {"sn":"610065","card":"00000000","wind":"0.00","weight":"0.36","angle":"115.30","radius":"10.60","height":"32.40","torque":"0.01","fall":"4","alarmType":"00000000000000000000000000000000","sensorStatus":"0000000000000000","rtimes":"2017-04-13 16:16:46","angleX":"0.00","angleY":"0.00","safeTorque":"282.00","safeWeight":"12.00","windLevel":"0","limitStatus":"0000000000000000","warnType":"00000000000000000000000000000000","powerStatu":"0"}
    /// </summary>
    public class MQTT_current
    {
        public string sn { get; set; }
        public string card { get; set; }
        public string wind { get; set; }
        public string weight { get; set; }
        public string angle { get; set; }
        public string radius { get; set; }
        public string height { get; set; }
        public string torque { get; set; }
        public string fall { get; set; }
        public string alarmType { get; set; }
        public string sensorStatus { get; set; }
        public string rtimes { get; set; }
        public string angleX { get; set; }
        public string angleY { get; set; }
        public string safeTorque { get; set; }
        public string safeWeight { get; set; }
        public string windLevel { get; set; }
        public string limitStatus { get; set; }
        public string warnType { get; set; }
        public string powerStatu { get; set; }
    }
}
