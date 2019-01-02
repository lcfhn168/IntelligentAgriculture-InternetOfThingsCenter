using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Text.RegularExpressions;
using SIXH.DBUtility;
using MQTTPushService.TowerCrane;

namespace MQTTPushService._021303
{
    public class MqttDataEncapsulation_021303
    {
        public static MQTTFrame MQTTFrameAnalyse(DBFrame dbf)
        {
            try
            {
                MQTTFrame mf = null;
                switch (dbf.datatype)
                {
                    case "heartbeat":
                        Heartbeat hb = Newtonsoft.Json.JsonConvert.DeserializeObject<Heartbeat>(dbf.contentjson);
                        mf =  heartbeatEncapsulation(hb); break;
                    case "current":
                        Current cu = Newtonsoft.Json.JsonConvert.DeserializeObject<Current>(dbf.contentjson);
                        mf =  CurrentEncapsulation(cu);
                        PushCurrentEncapsulation(cu);break;
                }
                MysqlTowerCrane_Local.UpdateTowerCranedbtypeByid(dbf.id);
              
                return mf;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MqttDataEncapsulation_021303.MQTTFrameAnalyse异常", ex.Message);
                return null;
            }
        }

        #region 心跳相关
        public static MQTTFrame heartbeatEncapsulation(Heartbeat data)
        {
            try
            {
                MQTT_heartbeat mqttData = new MQTT_heartbeat();
                mqttData.sn = data.EquipmentID;
                mqttData.rtimes = data.OnlineTime;

                MQTTFrame mf = new MQTTFrame();
                mf.DataType = "heartbeat";
                mf.EquipmentID = mqttData.sn;
                mf.mqh = mqttData;
                return mf;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MqttDataEncapsulation_021303.heartbeatEncapsulation异常", ex.Message);
                return null;
            }
        }
        #endregion

        #region 实时数据相关
        public static MQTTFrame CurrentEncapsulation(Current data)
        {
            try
            {
                MQTT_current mqttCurrent = new MQTT_current();
                mqttCurrent.sn = data.EquipmentID;
                mqttCurrent.card = data.DriverCardNo;
                mqttCurrent.rtimes = data.RTC;
                mqttCurrent.powerStatu = data.PowerState.ToString();
                mqttCurrent.fall = data.Times.ToString();
                mqttCurrent.height = ((double)data.Height / 100d).ToString("0.00");
                mqttCurrent.radius = ((double)data.Range / 100d).ToString("0.00");
                mqttCurrent.angle = ((double)data.Rotation / 10d).ToString("0.00");
                mqttCurrent.weight = ((double)data.Weight / 100d).ToString("0.00");
                mqttCurrent.wind = ((double)data.WindSpeed / 100d).ToString("0.00");
                #region 风速等级
                mqttCurrent.windLevel = ConvertWind.WindToLeve(float.Parse(mqttCurrent.wind)).ToString();
                if (int.Parse(mqttCurrent.windLevel) > 13)
                    mqttCurrent.windLevel = "12";
                #endregion
                mqttCurrent.angleX = ((double)data.DipAngle_X / 100d).ToString("0.00");
                mqttCurrent.angleY = ((double)data.DipAngle_Y / 100d).ToString("0.00");
                mqttCurrent.safeWeight = ((double)data.SafeWeight / 100d).ToString("0.00");
                mqttCurrent.safeTorque = ((double)data.SafeMoment / 10d).ToString("0.00");
                #region 力矩百分比
                double Torque = (double.Parse(mqttCurrent.weight) * double.Parse(mqttCurrent.radius));
                //力矩百分比
                if (data.SafeMoment != 0)
                    mqttCurrent.torque = (Torque / (double)data.SafeMoment).ToString("0.00");
                else
                    mqttCurrent.torque = "0.00";
                #endregion
                mqttCurrent.limitStatus = Convert.ToString(data.RelayState, 2).PadLeft(16, '0');
                mqttCurrent.sensorStatus = Convert.ToString(data.SensorState, 2).PadLeft(16, '0');
                mqttCurrent.warnType = Convert.ToString(data.WarningMessage, 2).PadLeft(32, '0');
                mqttCurrent.alarmType = Convert.ToString(data.AlarmMessage, 2).PadLeft(32, '0');

                MQTTFrame mf = new MQTTFrame();
                mf.DataType = "current";
                mf.EquipmentID = mqttCurrent.sn;
                mf.mqc = mqttCurrent;
                return mf;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MqttDataEncapsulation_021303.CurrentEncapsulation异常", ex.Message);
                return null;
            }
        }
         public static void PushCurrentEncapsulation(Current dataFrame)
        {
            try
            {
                if (dataFrame != null)
                {
                    string time = DateTime.Parse(dataFrame.RTC).ToString("yyyy年MM月dd日 HH点mm分ss秒");
                    string Sn = dataFrame.EquipmentID;
                    string alarm = Convert.ToString(dataFrame.AlarmMessage, 2).PadLeft(32, '0');
                    //重量预警
                    if (alarm[31] == '1')
                    {
                        string connect = string.Format("塔吊{0}设备重量报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect,null,null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                    //风速报警
                    if (alarm[29] == '1')
                    {
                        string connect = string.Format("塔吊{0}设备风速报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                    //力矩报警
                    if (alarm[30] == '1')
                    {
                        string connect = string.Format("塔吊{0}设备力矩报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                    //倾斜报警
                    if (alarm.Substring(8,4).Contains("1"))
                    {
                        string connect = string.Format("塔吊{0}设备倾斜报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                    //碰撞报警
                    if (alarm.Substring(4,4).Contains("1"))
                    {
                        string connect = string.Format("塔吊{0}设备碰撞报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                     //限位报警
                    if (alarm.Substring(18,6).Contains("1"))
                    {
                        string connect = string.Format("塔吊{0}设备限位报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                      //区域保护报警
                    if (alarm.Substring(0,4).Contains("1"))
                    {
                        string connect = string.Format("塔吊{0}设备区域保护报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MqttDataEncapsulation_021303.CurrentEncapsulation异常", ex.Message);
            }
        }
        #endregion
    }
}
