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

namespace MQTTPushService._0E
{
    public class MqttTowerCrane_0E
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
                        mf = heartbeatEncapsulation(hb); break;
                    case "current":
                        CraneCurrent cu = Newtonsoft.Json.JsonConvert.DeserializeObject<CraneCurrent>(dbf.contentjson);
                        mf = CraneCurrentEncapsulation(cu); break;
                        //PushCurrentEncapsulation(cu); break;
                    default: break;
                }
                MysqlTowerCrane_Local.UpdateTowerCranedbtypeByid(dbf.id);
                return mf;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MqttTowerCrane_0E.MQTTFrameAnalyse异常", ex.Message);
                return null;
            }
        }

        #region 实时数据
        public static MQTTFrame CraneCurrentEncapsulation(CraneCurrent c)
        {
            try
            {
                MQTT_current config = new MQTT_current();
                config.sn = c.Craneno;//编号
                config.card = c.Card;//卡号
                config.wind = c.Wind;//风速
                config.weight = c.Weight;//重量
                config.angle = c.Angle;//回转
                config.radius = c.Radius;//幅度
                config.height = c.Height;//高度
                config.torque = c.Torquepercent;//力矩百分比
                config.fall = c.Times;//倍率
                config.alarmType = c.AlarmType;//警告码
                config.warnType = c.WarnType;//预警告码
                config.sensorStatus = c.SensorStatus;//传感器状态
                config.limitStatus = c.LimitStatus;//限位器状态
                config.rtimes = c.Rtime;//时间
                config.angleX = c.AngleX;//倾角x
                config.angleY = c.AngleY;//倾角y
                config.safeTorque = c.Safetorque;//安全力矩
                config.safeWeight = c.SafeWeight;//安全重量
                config.windLevel = c.WindLevel;//风速等级
                config.powerStatu = c.PowerStatu;//电源状态

                MQTTFrame mf = new MQTTFrame();
                mf.DataType = "current";
                mf.EquipmentID = config.sn;
                mf.mqc = config;
                return mf;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_0E.CraneCurrentEncapsulation异常", ex.Message);
                return null;
            }
        }
        public static void PushCurrentEncapsulation(CraneCurrent dataFrame)
        {
            try
            {
                if (dataFrame != null)
                {
                    string time = DateTime.Parse(dataFrame.Rtime).ToString("yyyy年MM月dd日 HH点mm分ss秒");
                    string Sn = dataFrame.Craneno;
                    string alarm = dataFrame.AlarmType;
                    //重量报警
                    if (alarm[30] == '1')
                    {
                        string connect = string.Format("塔吊{0}设备重量报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect,null,null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                    //风速报警
                    if (alarm[31] == '1')
                    {
                        string connect = string.Format("塔吊{0}设备风速报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                    //力矩报警
                    if (alarm[28] == '1')
                    {
                        string connect = string.Format("塔吊{0}设备力矩报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                    //倾斜报警
                    if (alarm[27] == '1')
                    {
                        string connect = string.Format("塔吊{0}设备倾斜报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                    //碰撞报警
                    if (alarm[29] == '1')
                    {
                        string connect = string.Format("塔吊{0}设备碰撞报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                     //限位报警
                    if (alarm.Substring(10,6).Contains("1"))
                    {
                        string connect = string.Format("塔吊{0}设备限位报警，时间：{1}", Sn, time);
                        //HTTp.CollectJsonToPost_Event.BeginInvoke(Sn, connect, null, null);
                        HTTp.CollectJsonToPost_Event(Sn, connect);
                    }
                      //区域保护报警
                    if (alarm.Substring(16,4).Contains("1"))
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

        #region 心跳
        public static MQTTFrame heartbeatEncapsulation(Heartbeat data)
        {
            try
            {
                MQTT_heartbeat mqttData = new MQTT_heartbeat();
                mqttData.sn = data.SN;
                mqttData.rtimes = data.OnlineTime;

                MQTTFrame mf = new MQTTFrame();
                mf.DataType = "heartbeat";
                mf.EquipmentID = mqttData.sn;
                mf.mqh = mqttData;
                return mf;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MqttTowerCrane_0E.heartbeatEncapsulation异常", ex.Message);
                return null;
            }

        }
        #endregion
    }
}
