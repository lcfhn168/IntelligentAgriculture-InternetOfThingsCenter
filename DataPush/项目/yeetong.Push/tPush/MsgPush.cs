using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace yeetong_Push.tPush
{
   public class MsgPush
    {
       public static bool AlyMsg(ALIYUNMsg msg)
       {
           String product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
           String domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
           String accessKeyId = "LTAIGkF4EFD33JS0";//你的accessKeyId，参考本文档步骤2
           String accessKeySecret = "kJLAMchcmr6XRoFUlCKPrzdyO1MERI";//你的accessKeySecret，参考本文档步骤2
           IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
           //IAcsClient client = new DefaultAcsClient(profile);
           // SingleSendSmsRequest request = new SingleSendSmsRequest();
           //初始化ascClient,暂时不支持多region（请勿修改）
           DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
           IAcsClient acsClient = new DefaultAcsClient(profile);
           SendSmsRequest request = new SendSmsRequest();
           try
           {
               //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式，发送国际/港澳台消息时，接收号码格式为00+国际区号+号码，如“0085200000000”
               request.PhoneNumbers = msg.tel;
               //必填:短信签名-可在短信控制台中找到
               request.SignName = "共友智慧工地平台";
               //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
               request.TemplateCode = "SMS_144455109";
               //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
               //ALIYUNMsg model = new ALIYUNMsg();
               //model.name = "孙先生";
               //model.projectName = "通州新光大中心";
               //model.craneNo = "612222";
               //model.recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
               //model.@out = 1;
               string json = JsonConvert.SerializeObject(msg);
               request.TemplateParam = json;// "{\"name\":\"Tom\", \"projectName\":\"123\",\"craneNo\":\"1111\",\"recordTime\":\"2018-02-10\",\"out\":\"1\"}";
               //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
               request.OutId = "yourOutId";
               //请求失败这里会抛ClientException异常
               SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
              // System.Console.WriteLine(sendSmsResponse.Message);
               return true;
           }
           catch (ServerException ex)
           {
               System.Console.WriteLine("Hello World!");
           }
           catch (ClientException ex)
           {
               System.Console.WriteLine("Hello World!");
           }
           return false;
       }
    }
}
[DataContract(Namespace = "http://coderzh.cnblogs.com")]
public class ALIYUNMsg
{
    [DataMember(Order = 0)]
    public string name { get; set; }
    [DataMember(Order = 1)]
    public string projectName { get; set; }
    [DataMember(Order = 2)]
    public string craneNo { get; set; }
    [DataMember(Order = 3)]
    public string recordTime { get; set; }
    [DataMember(Order = 4)]
    public string @out { get; set; }
    [DataMember(Order = 5)]
    public string tel { get; set; }
}