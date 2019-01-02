using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBStorage.TowerCrane._0E
{
    public class WorkingCycle
    {
        #region 过度标识变量
        /**
         * 规则：吊重开始--下一次吊重开始（或无心跳）
         * 起始条件：大于0.2t&&（高度或幅度或回转有变化）。
         * 时间：大于等于1分钟，如果小于1分钟，则属于上一个工作循环。
         * 工作循环结束的判断条件就是：重量小于0.2或（高度或幅度或回转有变化）
         **/
        /// <summary>
        /// 重量临界值
        /// </summary>
        public double WeightCriticalValue { set; get; }
        /// <summary>
        /// 高度和幅度的长度差值
        /// </summary>
        public double HeightRangeCriticalValue { set; get; }
        /// <summary>
        /// 回转差值
        /// </summary>
        public double RotationCriticalValue { set; get; }
        /// <summary>
        /// 上一次的高度
        /// </summary>
        public string lastHeight { set; get; }
        /// <summary>
        /// 上一次的幅度
        /// </summary>
        public string lastRange { set; get; }
        /// <summary>
        /// 上一次的回转角度
        /// </summary>
        public string lastRotationAngle { set; get; }
        /// <summary>
        /// 是否是一次新的工作循环
        /// </summary>
        public bool isNewWorkingCycle { set; get; }
        /// <summary>
        /// 工序哦循环开始的时间
        /// </summary>
        public DateTime StartTime { set; get; }
        /// <summary>
        /// 是否是重新连接的第一次实时数据处理
        /// </summary>
        public bool isFirstCurrent { set; get; }
        /// <summary>
        /// 是否是重新连接的第一次的工作循环吗？
        /// 为1时是第一次的工作循环
        /// 为2时是第一次连接时的那个就存在的工作循环
        /// 为0时不是第一次的工作循环也不是第一次连接时的那个就存在的工作循环
        /// </summary>
        public byte isFirstWC { set; get; }
        /// <summary>
        /// 当前工作循环序列号
        /// </summary>
        public string WCID { set; get; }
        #endregion
        #region 方法
        public WorkingCycle()
        {
            WeightCriticalValue = 0.2d;//0.2吨
            HeightRangeCriticalValue = 0.5d;//0.5米
            RotationCriticalValue = 2d;//2度
            lastHeight = "0";
            lastRange = "0";
            lastRotationAngle = "";
            isNewWorkingCycle = true;
            StartTime = Convert.ToDateTime(null);
            isFirstCurrent = true;//重新连接的第一次实时数据处理
            isFirstWC = 1;
            WCID = "";
        }
        #endregion
    }
}
