using cn.jpush.api;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MQTTPushService.tPush
{
    public class JGPush
    {
        public static String TITLE = "您有一条新消息";
        public static String ALERT = "点击查看";
        public static String MSG_CONTENT = "json";
        public static String REGISTRATION_ID = "bd085a98c4";
        public static String SMSMESSAGE = "Test from C# v3 sdk - SMSMESSAGE";
        public static int DELAY_TIME = 1;
        public static String TAG = "";
        public static String app_key = "f6e55989a86be9729db87065";
        public static String master_secret = "9a67c3ee8588dbd28b55b498";
        public static bool JGSendIOSAndAndroid(string[] tel,  int etype)
        {
            bool t = false;
            try
            {
                JPushClient client = new JPushClient(app_key, master_secret);
                TITLE = GetTitle(etype);
                var result = client.SendPush(PushObject_Android_Tag_AlertWithTitle(tel));
                if (((cn.jpush.api.common.BaseResult)(result)).ResponseResult.responseCode.ToString().Equals("OK"))
                    t = true;
                else
                    t = false;
            }
            catch
            { }
            return t;
        }
        /// <summary>
        /// Android和iOS极光推送
        /// </summary>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static PushPayload PushObject_Android_Tag_AlertWithTitle(string[] tel)
        {
            var pushPayload = new PushPayload();
            pushPayload.platform = Platform.android_ios();
            pushPayload.audience = Audience.s_alias(tel);
            var notification = new Notification();
            notification.IosNotification = new IosNotification().setAlert(TITLE).setBadge(1);
            notification.AndroidNotification = new AndroidNotification().setAlert(ALERT).setTitle(TITLE);
            pushPayload.options.apns_production = true;
            pushPayload.notification = notification;
            return pushPayload;
        }
        /// <summary>
        /// 提示消息
        /// </summary>
        /// <param name="otype"></param>
        /// <param name="etype"></param>
        /// <returns></returns>
        static string GetTitle(int etype)
        {
            string msg = "您有一条 ";
            #region 塔机的报警通知
            switch (etype)
            {
                case 1:
                    msg += "塔机报警消息";
                    break;
                case 2:
                    msg += "升降机报警消息";
                    break;
                case 3:
                    msg += "卸料平台报警消息";
                    break;
                case 5:
                    msg += "扬尘噪声报警消息";
                    break;
                case 6:
                    msg += "红外对射报警消息";
                    break;
                case 7:
                    msg += "烟感报警消息";
                    break;
                case 8:
                    msg += "气体检测消息";
                    break;
            }
            #endregion
            return msg;
        }
    }
}
