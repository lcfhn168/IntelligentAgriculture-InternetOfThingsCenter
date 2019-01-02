using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DBToOssServer
{
    public class CraneDeleteFile
    {
        /// <summary>
        /// 删除生成的txt文件
        /// </summary>
        /// <param name="path"></param>
       public static void DeleteFiles()
        {
            try
            {
                string root = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string path = root.Remove(root.LastIndexOf('\\') + 1) + "Cranetxt";
                DirectoryInfo directory = new DirectoryInfo(path);
                if (directory.Exists)//存在
                {
                    System.IO.Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlCrane_Oss.DeleteFile异常", ex.Message);
            }
        }
    }
}
