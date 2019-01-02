using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace yeetong_ProtocolAnalysis
{
    public class HexStringToDouble
    {
        public static float HexStringToDoubleFun(string HexString)
        {
            try
            {
                uint num = uint.Parse(HexString, System.Globalization.NumberStyles.AllowHexSpecifier);
                byte[] floatValues = BitConverter.GetBytes(num);
                float f = BitConverter.ToSingle(floatValues, 0);
                return f;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
