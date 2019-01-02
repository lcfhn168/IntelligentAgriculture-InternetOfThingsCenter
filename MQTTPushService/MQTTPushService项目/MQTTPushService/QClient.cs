using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;
using System.Windows.Forms;

namespace MQTTPushService
{
    public class QClient
    {
        #region 字段和事件
        String USERNAME = "admin";
         String PASSWORD = "password";
         String MQTT_IP = "120.55.119.204";
         string MQTT_Port = "61613";
        public  MqttClient mqtt_client;

        public  event Action<string> show = new Action<string>((string s) =>
        {
            ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + @"\Received", "Data", s); 
        });
        #endregion

        #region 构造
        public  QClient() 
        {
            ClientInit();
        }
        #endregion

        #region 初始化并连接
        void ClientInit()
        {
            try
            {
                USERNAME = ToolAPI.INIOperate.IniReadValue("mqtt", "MQTT_User", MainStatic.Path);
                PASSWORD = ToolAPI.INIOperate.IniReadValue("mqtt", "MQTT_Password", MainStatic.Path);
                MQTT_IP = ToolAPI.INIOperate.IniReadValue("mqtt", "MQTT_IP", MainStatic.Path);
                MQTT_Port = ToolAPI.INIOperate.IniReadValue("mqtt", "MQTT_Port", MainStatic.Path);
                MqttSettings.MQTT_BROKER_DEFAULT_PORT = int.Parse(MQTT_Port);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("从配置文件获取连接参数异常", ex.Message + ex.StackTrace);
            }
            Connect();
        }
        public void ClientClose()
        {
            try
            {
               if(mqtt_client!=null)
               {
                   mqtt_client.Disconnect();
                   mqtt_client = null;
               }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("从配置文件获取连接参数异常", ex.Message + ex.StackTrace);
            }
            Connect();
        }
        #endregion

        
        //创建连接
        void Connect()
        {
            try
            {
                mqtt_client = new MqttClient(IPAddress.Parse(MQTT_IP));
                mqtt_client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;  
                string clientId = Guid.NewGuid().ToString();
                mqtt_client.Connect(clientId, USERNAME, PASSWORD);
            }
            catch (Exception ex) {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("Connect异常", ex.Message + ex.StackTrace);
                return; }
        }



        //接收事件的处理
         void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //处理接收到的消息  
            string msg = System.Text.Encoding.Default.GetString(e.Message);
            show(msg);
        }
        //发送数据方法
        public  void process_mqtt(string topic, string MSG)
        {
            try
            {
                if (mqtt_client == null || (mqtt_client != null && !mqtt_client.IsConnected))//没有被赋值 先赋值
                {
                    ClientInit();
                }
                mqtt_client.Publish(topic, Encoding.UTF8.GetBytes(MSG), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);//只有一次，需要确认回复
             }
            catch (Exception) { }
        }
        //主题订阅
        public void Subscribe(string RecTopic)
        {
            try
            {
                if (mqtt_client == null || (mqtt_client != null && !mqtt_client.IsConnected))//没有被赋值 先赋值
                {
                    ClientInit();
                }
                //订阅一个主题 "/home/temperature" 消息质量为 2
                mqtt_client.Subscribe(new string[] { RecTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            catch (Exception) { }
        }
    }
}
