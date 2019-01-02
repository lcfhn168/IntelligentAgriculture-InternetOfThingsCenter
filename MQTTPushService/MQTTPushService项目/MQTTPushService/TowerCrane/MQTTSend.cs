using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Data;

namespace MQTTPushService.TowerCrane
{


    public class MQTTSend
    {
        public static void MQTT_SendData(MQTTFrame frame)
        {
            try
            {

                if (frame != null)
                {
                    ForwardModel fm = new ForwardModel();
                    fm.AgreementName = "";
                    fm.Port = MainStatic.putPort;
                    fm.Server = MainStatic.putServer;
                    fm.putType = MainStatic.putType;
                    fm.sn = frame.EquipmentID;
                    fm.etype = 1;
                    if (frame.DataType == "heartbeat")
                    {
                        //标识符
                        string topic = "/root/" + frame.EquipmentID + "/heartbeat";
                        //数据对象的JOin
                        string joinstring = JsonConvert.SerializeObject(frame.mqh);
                        //发送数据

                        if (fm.putType.Equals("0"))
                            TowerCrane_Main.qc_TowerCrane.process_mqtt(topic, joinstring);
                        else
                            TCp.SendMessageToServer(fm, joinstring);
                    }
                    else if (frame.DataType == "current")
                    {
                        string topic = "/root/" + frame.EquipmentID + "/current";
                        //数据对象的JOin
                        string joinstring = JsonConvert.SerializeObject(frame.mqc);
                        //发送数据
                        if (fm.putType.Equals("0"))
                            TowerCrane_Main.qc_TowerCrane.process_mqtt(topic, joinstring);
                        else
                            TCp.SendMessageToServer(fm, joinstring);
                        UnifiedPush.PushJG(frame.mqc.alarmType, fm); //塔机推送
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MQTT_SendData异常", ex.Message + ex.StackTrace);
            }
        }
       
    }
}

