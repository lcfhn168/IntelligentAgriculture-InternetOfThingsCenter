using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace yeetong_ProtocolAnalysis
{
    class StringByDouble
    {
        /// <summary>
        /// 把一个数转化为double类型，并缩小对应的倍数
        /// </summary>
        /// <param name="obj">数</param>
        /// <param name="minification">缩率</param>
        /// <returns>成功返回对应的值，失败返回0</returns>
        public static double ConvertDouble(object obj, int minification)
        {
            double temp = 0d;
            if(double.TryParse(obj.ToString(),out temp))
            {
                return minification != 0 ? (double)(temp / (double)minification) : 0d;
            }
            return 0d;
        }
        /// <summary>
        /// 把一个数转化为double类型，并缩小对应的倍数
        /// </summary>
        /// <param name="obj">数</param>
        /// <param name="minification">缩率</param>
        /// <returns>成功返回对应的值de 字符串形式，失败返回0</returns>
        public static string ConvertDoubleString(object obj, int minification)
        {
            return ConvertDouble(obj, minification).ToString("0.00");
        }
    }
}
