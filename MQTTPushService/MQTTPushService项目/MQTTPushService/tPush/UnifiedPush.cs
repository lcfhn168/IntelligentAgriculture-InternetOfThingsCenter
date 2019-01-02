using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MQTTPushService
{
    public class UnifiedPush
    {
        public static void PushJG(string alarm, ForwardModel fm)
        {
            if (!string.IsNullOrEmpty(alarm) && alarm.Contains("1")) //如果报警码不等于空并且报警码里面含有1的时候继续执行
            {
                //ToolAPI.XMLOperation.WriteLogXmlNoTail("有报警值需要推送", fm.sn + ";" + alarm);
                if (MysqlTowerCrane_Net.IsTenMinPush(fm.sn, fm.etype) <= 0) //如果最近10分钟没有发送推送，即可推送
                {
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("时间允许推送", fm.sn + ";" + alarm);
                    bool status = false;
                    DataTable dt = MysqlTowerCrane_Net.GetPushTypeNew(fm.sn, fm.etype); //获取推送的设置信息
                    string[] person = null;
                    if (dt != null)
                        if (dt.Rows.Count > 0)                                //找到推送设置的信息
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("获得了推送设置信息", fm.sn + ";" + alarm);
                            if (!string.IsNullOrEmpty(IsAlarm(alarm, dt.Rows[0]["type"].ToString(), fm.etype)))//查看有没有报警，并且查看设置的报警推送存在不存在，如果存在即推送
                            {
                                string pushtype = dt.Rows[0]["pushType"].ToString();//推送类型，分别推送与那些设备
                                if (pushtype.Contains("2")) //如果推送类型中存在2，及发送APP，极光推送
                                {
                                    person = GetPerson(dt);  //获取要推送的人员数组
                                    if (person != null)
                                        if (person.Length > 0)                                   //如果后台设置了要推送的人员数组
                                            if (tPush.JGPush.JGSendIOSAndAndroid(person, fm.etype)) //极光推送的app
                                                status = true;
                                    ToolAPI.XMLOperation.WriteLogXmlNoTail("极光推送", fm.sn + ";" + alarm);
                                }
                                if (pushtype.Contains("3")) //如果推送类型中存在3，及发送短信到客户手机
                                {
                                    string personsplit = ListToString(person);
                                    if (!string.IsNullOrEmpty(personsplit))
                                    {
                                        ALIYUNMsg msg = new ALIYUNMsg();
                                        if (fm.etype == 13)  //只针对强电监测的设备进行设备号转成设备名称并推送
                                            msg.craneNo = fm.AgreementName + IntToString(fm.etype);
                                        else
                                            msg.craneNo = fm.sn + IntToString(fm.etype);
                                        msg.name = "用户";
                                        msg.@out = "报警";
                                        msg.recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        msg.projectName = dt.Rows[0]["proname"].ToString();
                                        msg.tel = personsplit;
                                        if (tPush.MsgPush.AlyMsg(msg))
                                            status = true;
                                        ToolAPI.XMLOperation.WriteLogXmlNoTail("短信推送", fm.sn + ";" + alarm);
                                    }
                                }
                                if (pushtype.Contains("4")) //如果推送类型中存在4，即推送消息到用户电话
                                {
                                    string alarmsn = "";
                                    if (fm.etype == 13)  //只针对强电监测的设备进行设备号转成设备名称并推送
                                        alarmsn = fm.AgreementName + IntToString(fm.etype);
                                    else
                                        alarmsn = fm.sn + IntToString(fm.etype);
                                    if (tPush.VoicePush.VoicePushJava(person, dt.Rows[0]["proname"].ToString(), alarmsn))
                                        status = true;
                                    ToolAPI.XMLOperation.WriteLogXmlNoTail("语音推送", fm.sn + ";" + alarm);
                                }
                                if (pushtype.Contains("1")) //如果推送类型中存在1，即推送消息到平台
                                {
                                    fm.AgreementName = dt.Rows[0]["proUuid"].ToString(); //工地号
                                    if (TCp.SendMessageToServerNotice(fm, "1"))
                                        status = true;
                                    ToolAPI.XMLOperation.WriteLogXmlNoTail("平台推送", fm.sn + ";" + alarm);
                                }
                            }
                        }
                        else
                        {
                            //ToolAPI.XMLOperation.WriteLogXmlNoTail("没有获得推送设置信息", fm.sn + ";" + alarm);
                        }
                    if (status) //如果有一方推送成功把该推送记录保存
                    {
                        if (dt != null)
                            for (int i = 0; i < dt.Rows.Count; i++)
                                MysqlTowerCrane_Net.SavePushrecoder(fm.sn, IsAlarm(alarm, dt.Rows[i]["type"].ToString(), fm.etype), dt.Rows[i]["uuid"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("存储平台推送记录", fm.sn + ";" + alarm);
                    }
                }
            }
            else
            {
                //ToolAPI.XMLOperation.WriteLogXmlNoTail("不需要推送", fm.sn+";"+ alarm);
            }
        }
        #region 判断塔机是否有报警,并且获取那些报警
        /// <summary>
        /// 判断塔机是否有报警,并且获取那些报警
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        static string IsAlarm(string alarm, string type, int etype)
        {
            string result = "";
            try
            {
                if (etype.Equals(1)) //塔吊设备
                {
                    alarm = alarm.Substring(alarm.Length - 5, 5);
                    if (alarm.Substring(4, 1).Equals("1") && type.Contains("3"))
                        result += "风速报警,";
                    else if (alarm.Substring(3, 1).Equals("1") && type.Contains("1"))
                        result += "超重报警,";
                    else if (alarm.Substring(2, 1).Equals("1") && type.Contains("5"))
                        result += "碰撞报警,";
                    else if (alarm.Substring(1, 1).Equals("1") && type.Contains("2"))
                        result += "力矩报警,";
                    else if (alarm.Substring(0, 1).Equals("1") && type.Contains("4"))
                        result += "倾斜报警";
                }
                else if (etype.Equals(2)) //升降机设备
                {
                    if (alarm.Substring(14, 1).Equals("1") && type.Contains("1"))
                        result += "重量报警,";
                    else if (alarm.Substring(12, 1).Equals("1") && type.Contains("3"))
                        result += "顶层报警,";
                    else if (alarm.Substring(11, 1).Equals("1") && type.Contains("4"))
                        result += "蹲低,";
                    else if (alarm.Substring(10, 1).Equals("1") && type.Contains("5"))
                        result += "门打开,";
                    else if (alarm.Substring(8, 1).Equals("1") && type.Contains("6"))
                        result += "风速报警,";
                    else if (alarm.Substring(7, 1).Equals("1") && type.Contains("2"))
                        result += "人数报警,";
                    else if (alarm.Substring(6, 1).Equals("1") && type.Contains("7"))
                        result += "防坠器报警";
                }
                else if (etype.Equals(3)) //卸料平台
                {
                    if (alarm.Substring(6, 1).Equals("1") && type.Contains("1"))
                        result += "重量报警,";
                    if (alarm.Substring(4, 1).Equals("1") && type.Contains("2"))
                        result += "倾斜报警";
                }
                else if (etype.Equals(5)) //扬尘噪声
                {
                    if (alarm.Substring(2, 1).Equals("1") && type.Contains("3"))
                        result += "噪音报警,";
                    if (alarm.Substring(1, 1).Equals("1") && type.Contains("1"))
                        result += "PM2.5报警,";
                    if (alarm.Substring(0, 1).Equals("1") && type.Contains("2"))
                        result += "PM10报警";
                }
                else if (etype.Equals(6)) //红外
                {
                    if (alarm.Contains("1"))
                        result = "红外报警";
                }
                else if (etype.Equals(7)) //烟感
                {
                    if (alarm.Contains("1"))
                        result = "烟感报警";
                }
                else if (etype.Equals(8)) //气体
                {
                    if (alarm.Contains("1"))
                        result = "气体报警";
                }
                else if (etype.Equals(13)) //强电监测
                {
                    if (alarm.Substring(15, 1).Equals("1") && type.Contains("1"))
                        result = "漏电报警";
                }
            }
            catch { }
            return result.Trim(',');
        }
        #endregion
        #region 把数组转成字符串
        /// <summary>
        /// 把数组转成字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string ListToString(string[] str)
        {
            string result = "";
            try
            {
                if (str != null)
                    if (str.Length > 0)
                    {
                        foreach (var item in str)
                        {
                            result += item + ",";
                        }
                        return result.Trim(',');
                    }
            }
            catch { }
            return result;
        }
        #endregion
        #region 数组转设备类型
        static string[] GetPerson(DataTable dt)
        {
            string[] result = null;
            try
            {
                result = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result[i] = dt.Rows[i]["phone"].ToString();
                }
            }
            catch { }
            return result;
        }
        /// <summary>
        /// 数组转设备类型
        /// </summary>
        /// <param name="etype"></param>
        /// <returns></returns>
        static string IntToString(int etype)
        {
            string result = "";
            switch (etype)
            {
                case 1:
                    result = " 塔机";
                    break;
                case 2:
                    result = " 升降机";
                    break;
                case 3:
                    result = " 卸料平台";
                    break;
                case 5:
                    result = " 扬尘噪音";
                    break;
                case 6:
                    result = " 红外对射";
                    break;
                case 7:
                    result = " 烟感";
                    break;
                case 8:
                    result = " 气体检测";
                    break;
                case 13:
                    result = " 强电监测";
                    break;
            }
            return result;
        }
        #endregion
    }
}
