using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPAPI;

namespace GOYO_Architecture
{
    public class ExternalClass : IExternalInterface
    {
        #region 实现接口
        public Object External { set; get; }
        public event Func<byte[], TcpSocketClient, Object, Object> InitEvent;  
        //为了给你的外部绑定类进行初始化
        public void ExternalInit(byte[] obj, TcpSocketClient tcpClientTemp)
        {
            
        }
        #endregion

        public ExternalClass()
        {
            External = new TcpClientBindingExternalClass();
            InitEvent += (byte[] obj, TcpSocketClient client, Object ob) => { return ""; };
        }


    }
    public class TcpClientBindingExternalClass
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string TVersion { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string EquipmentID { get; set; }
        public TcpClientBindingExternalClass()
        {
            TVersion = "";//版本号初始化
            EquipmentID = "";//设备编号初始化
        }
    }
}
