using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Common
{
    public class LoggerHelper
    {
        /// <summary>
        /// 导入数据时记录有问题的数据
        /// </summary>
        /// <param name="sourceTableName">目标表名</param>
        /// <param name="keyName">目标表主键列名</param>
        /// <param name="keyValue">出错行主键值</param>
        /// <param name="badData">出错的列及错误值、替代值</param>
        public static void WriteWarnInfo(string sourceTableName,string keyName,string keyValue,List<Tuple<string,string,string>> badData)
        {
            var log = log4net.LogManager.GetLogger("logger");
            log.Warn(string.Format("警告：导入了问题数据。sourceTableName:{0},keyName:{1},keyValue:{2}。badData:{3}",
                sourceTableName,keyName,keyValue,string.Join(",", badData.Select(x => "(" + x.Item1 + "，" + x.Item2+","+x.Item3+")"))));
        }
    }
}
